using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ARTapToPlace : MonoBehaviour
{
    [Header("Placement Settings")]
    [SerializeField]
    [Tooltip("The placement indicator showing where the object can be placed.")]
    GameObject placementIndicator;

    [HideInInspector]
    public GameObject placedPrefab; // This will be set by ModelPlacer

    [HideInInspector]
    public GameObject spawnedObject; // Made public for ModelPlacer access

    TouchInput controls;
    private ARRaycastManager aRRaycastManager;
    private XROrigin arOrigin;
    private Camera arCamera;
    private Pose placementPose;
    private bool placementPoseIsValid = false;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
        arOrigin = GetComponent<XROrigin>();
        arCamera = arOrigin != null ? arOrigin.Camera : Camera.main;

        controls = new TouchInput();
        controls.control.touch.performed += ctx =>
        {
            if (ctx.control.device is Pointer device)
            {
                OnPress();
            }
        };

        ValidateSetup();
    }

    private void ValidateSetup()
    {
        if (aRRaycastManager == null)
            Debug.LogError("ARRaycastManager not found!");
        if (placementIndicator == null)
            Debug.LogError("Placement Indicator is not assigned!");
        else
            placementIndicator.SetActive(false);
    }

    private void OnEnable() => controls.control.Enable();
    private void OnDisable() => controls.control.Disable();
    private void Update() => UpdatePlacementIndicator();

    void OnPress()
    {
        if (placementPoseIsValid && placedPrefab != null)
        {
            // Remove previous model
            if (spawnedObject != null)
                Destroy(spawnedObject);

            // Place new model
            spawnedObject = Instantiate(placedPrefab, placementPose.position, placementPose.rotation);
            Debug.Log($"✅ Model placed: {placedPrefab.name}");
        }
        else
        {
            if (placedPrefab == null)
                Debug.LogWarning("No model selected to place!");
            else
                Debug.LogWarning("No valid placement position!");
        }
    }

    private void UpdatePlacementIndicator()
    {
        if (placementIndicator == null) return;

        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        hits.Clear();

        if (aRRaycastManager?.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon) == true && hits.Count > 0)
        {
            placementPoseIsValid = true;
            placementPose = hits[0].pose;

            if (arCamera != null)
            {
                Vector3 cameraForward = arCamera.transform.forward;
                Vector3 cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
                if (cameraBearing != Vector3.zero)
                    placementPose.rotation = Quaternion.LookRotation(cameraBearing);
            }

            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementPoseIsValid = false;
            placementIndicator.SetActive(false);
        }
    }
}