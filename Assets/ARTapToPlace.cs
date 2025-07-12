using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))] 
public class ARTapToPlace : MonoBehaviour
{
    /// <summary>
    /// The placement indicator prefab.
    /// </summary>
    [SerializeField]
    [Tooltip("The placement indicator showing where the object can be placed.")]
    GameObject placementIndicator;
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject placedPrefab;
    /// <summary>
    /// The instantiated object.
    /// </summary>
    GameObject spawnedObject;

    /// <summary>
    /// The input touch control.
    /// </summary>
    TouchInput controls;

    private ARRaycastManager aRRaycastManager;
    private XROrigin arOrigin;
    private Camera arCamera;
    private Pose placementPose;
    private bool placementPoseIsValid = false;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake()
    {
        // Initialize components
        aRRaycastManager = GetComponent<ARRaycastManager>();
        arOrigin = GetComponent<XROrigin>();
        arCamera = arOrigin != null ? arOrigin.Camera : Camera.main;

        controls = new TouchInput();
        controls.control.touch.performed += ctx =>
        {
            if (ctx.control.device is Pointer device)
            {
                OnPress(device.position.ReadValue());
            }
        };

        // Validate setup
        if (aRRaycastManager == null)
        {
            Debug.LogError("ARRaycastManager not found! Make sure it's added to your AR Session Origin.");
        }

        if (placedPrefab == null)
        {
            Debug.LogError("Object to Place is not assigned!");
        }

        if (placementIndicator == null)
        {
            Debug.LogError("Placement Indicator is not assigned!");
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void OnEnable()
    {
        controls.control.Enable();
    }

    private void OnDisable()
    {
        controls.control.Disable();
    }

    private void Update()
    {
        UpdatePlacementIndicator();
    }

    void OnPress(Vector3 position)
    {
        // Check if the raycast hit any trackables
        if (aRRaycastManager.Raycast(position, hits, TrackableType.PlaneWithinPolygon))
        {
            // Raycast hits are sorted by distance, so the first hit means the closest
            var hitPose = hits[0].pose;

            // Instantiate the prefab
            spawnedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation);
            Debug.Log($"✅ Object placed successfully at: {hitPose.position}");

            // Make the spawned object face the camera
            if (arCamera != null)
            {
                Vector3 lookPos = arCamera.transform.position - spawnedObject.transform.position;
                lookPos.y = 0;
                spawnedObject.transform.rotation = Quaternion.LookRotation(lookPos);
            }
        }
        else
        {
            Debug.LogWarning("Cannot place object: No valid plane detected at touch position!");
        }
    }

    private void UpdatePlacementIndicator()
    {
        if (placementIndicator != null)
        {
            // Use screen center for raycast
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
            hits.Clear();

            // Perform raycast
            if (aRRaycastManager != null && aRRaycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
            {
                if (hits.Count > 0)
                {
                    placementPoseIsValid = true;
                    placementPose = hits[0].pose;

                    // Make the placement indicator face the camera
                    if (arCamera != null)
                    {
                        Vector3 cameraForward = arCamera.transform.forward;
                        Vector3 cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
                        if (cameraBearing != Vector3.zero)
                        {
                            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
                        }
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
            else
            {
                placementPoseIsValid = false;
                placementIndicator.SetActive(false);
            }
        }
    }
}