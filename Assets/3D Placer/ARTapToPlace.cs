using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ARModelPlacement : MonoBehaviour
{
    [Header("AR Components")]
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARPlaneManager planeManager;

    [Header("Placement Settings")]
    [SerializeField] private GameObject placementIndicator;
    [SerializeField] private GameObject modelPrefab;

    [Header("Indicator Settings")]
    [SerializeField] private float indicatorRotationSpeed = 50f;
    [SerializeField] private float indicatorHoverHeight = 0.01f;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private Pose placementPose;
    private bool placementPoseIsValid = false;
    private Camera arCamera;
    private GameObject spawnedModel;
    private bool modelPlaced = false;

    void Awake()
    {
        // Get AR components
        if (raycastManager == null)
            raycastManager = GetComponent<ARRaycastManager>();

        if (planeManager == null)
            planeManager = GetComponent<ARPlaneManager>();

        arCamera = Camera.main;
    }

    void Start()
    {
        // Ensure placement indicator is initially hidden
        if (placementIndicator != null)
            placementIndicator.SetActive(false);

        // Validate required components
        if (modelPrefab == null)
            Debug.LogError("Model Prefab is not assigned!");

        if (placementIndicator == null)
            Debug.LogError("Placement Indicator is not assigned!");
    }

    void Update()
    {
        // Only update placement pose if model hasn't been placed yet
        if (!modelPlaced)
        {
            UpdatePlacementPose();
            UpdatePlacementIndicator();
        }

        // Handle touch input for model placement
        if (placementPoseIsValid && Input.touchCount > 0 && !modelPlaced)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                PlaceModel();
            }
        }

        // Optional: Also support mouse click for Unity Editor testing
#if UNITY_EDITOR
        if (placementPoseIsValid && Input.GetMouseButtonDown(0) && !modelPlaced)
        {
            PlaceModel();
        }
#endif
    }

    private void UpdatePlacementPose()
    {
        // Get the center of the screen
        Vector3 screenCenter = arCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));

        // Perform raycast from screen center
        if (raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
        {
            placementPoseIsValid = true;
            placementPose = hits[0].pose;

            // Adjust the pose to align with camera direction (optional)
            Vector3 cameraForward = arCamera.transform.forward;
            Vector3 cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
        else
        {
            placementPoseIsValid = false;
        }
    }

    private void UpdatePlacementIndicator()
    {
        if (placementIndicator == null)
            return;

        if (placementPoseIsValid)
        {
            // Show and position the indicator
            placementIndicator.SetActive(true);

            // Add slight hover effect
            Vector3 indicatorPosition = placementPose.position;
            indicatorPosition.y += indicatorHoverHeight;
            placementIndicator.transform.position = indicatorPosition;

            placementIndicator.transform.rotation = placementPose.rotation;

            // Optional: Add rotation animation to indicator
            placementIndicator.transform.Rotate(Vector3.up, indicatorRotationSpeed * Time.deltaTime);
        }
        else
        {
            // Hide indicator if no valid surface
            placementIndicator.SetActive(false);
        }
    }

    private void PlaceModel()
    {
        if (modelPrefab == null || !placementPoseIsValid)
            return;

        // Instantiate the model at the placement pose
        spawnedModel = Instantiate(modelPrefab, placementPose.position, placementPose.rotation);

        // Hide the placement indicator
        if (placementIndicator != null)
            placementIndicator.SetActive(false);

        // Mark model as placed
        modelPlaced = true;

        // Optional: Disable plane detection after placement to save resources
        if (planeManager != null)
            planeManager.enabled = false;

        Debug.Log("Model placed successfully!");
    }

    // Public method to reset placement (useful for UI button)
    public void ResetPlacement()
    {
        if (spawnedModel != null)
        {
            Destroy(spawnedModel);
            spawnedModel = null;
        }

        modelPlaced = false;

        // Re-enable plane detection
        if (planeManager != null)
            planeManager.enabled = true;

        // Show indicator again
        if (placementIndicator != null)
            placementIndicator.SetActive(true);
    }

    // Optional: Method to place multiple models
    public void EnableMultiplePlacements()
    {
        modelPlaced = false;

        if (placementIndicator != null)
            placementIndicator.SetActive(true);
    }
}