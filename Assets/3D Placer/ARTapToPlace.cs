using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARTapToPlace : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject objectToPlace;
    public GameObject placementIndicator;

    [Header("Debug")]
    public bool showDebugInfo = true;

    private XROrigin arOrigin;
    private ARRaycastManager arRaycastManager;
    private Camera arCamera;
    private Pose placementPose;
    private bool placementPoseIsValid = false;

    void Start()
    {
        // Find AR components
        arOrigin = FindFirstObjectByType<XROrigin>();
        arRaycastManager = FindFirstObjectByType<ARRaycastManager>();

        // Get the AR camera
        if (arOrigin != null)
        {
            arCamera = arOrigin.Camera;
        }
        else
        {
            arCamera = Camera.main;
        }

        // Validate setup
        if (arRaycastManager == null)
        {
            Debug.LogError("ARRaycastManager not found! Make sure it's added to your AR Session Origin.");
        }

        if (objectToPlace == null)
        {
            Debug.LogError("Object to Place is not assigned!");
        }

        if (placementIndicator == null)
        {
            Debug.LogError("Placement Indicator is not assigned!");
        }
        else
        {
            // Make sure indicator starts hidden
            placementIndicator.SetActive(false);
        }
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        // Handle touch input
        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Debug.Log("Touch detected! Attempting to place object...");
            PlaceObject();
        }

        // Debug info
        if (showDebugInfo)
        {
            if (Input.touchCount > 0)
            {
                Debug.Log($"Touch detected! Placement Valid: {placementPoseIsValid}, Touch Count: {Input.touchCount}");
            }
        }
    }

    private void PlaceObject()
    {
        if (objectToPlace == null)
        {
            Debug.LogError("Object to Place is NULL! Please assign a prefab in the inspector.");
            return;
        }

        if (!placementPoseIsValid)
        {
            Debug.LogWarning("Placement pose is not valid!");
            return;
        }

        GameObject placedObject = Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
        Debug.Log($"✅ Object placed successfully at: {placementPose.position}");

        // Optional: Make the object slightly bigger so it's visible
        placedObject.transform.localScale = objectToPlace.transform.localScale;
    }

    private void UpdatePlacementIndicator()
    {
        if (placementIndicator != null)
        {
            if (placementPoseIsValid)
            {
                placementIndicator.SetActive(true);
                placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
            }
            else
            {
                placementIndicator.SetActive(false);
            }
        }
    }

    private void UpdatePlacementPose()
    {
        // Use screen center for raycast
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        // Perform raycast
        if (arRaycastManager != null && arRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes))
        {
            if (hits.Count > 0)
            {
                placementPoseIsValid = true;
                placementPose = hits[0].pose;

                // Optional: Make object face the camera
                if (arCamera != null)
                {
                    Vector3 cameraForward = arCamera.transform.forward;
                    Vector3 cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
                    if (cameraBearing != Vector3.zero)
                    {
                        placementPose.rotation = Quaternion.LookRotation(cameraBearing);
                    }
                }

                if (showDebugInfo)
                {
                    Debug.Log($"Valid placement pose found at: {placementPose.position}");
                }
            }
            else
            {
                placementPoseIsValid = false;
            }
        }
        else
        {
            placementPoseIsValid = false;
            if (showDebugInfo && arRaycastManager == null)
            {
                Debug.LogWarning("ARRaycastManager is null!");
            }
        }
    }
}