using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARTapToPlaceObject : MonoBehaviour
{
    [Header("AR Placement References")]
    public GameObject objectToPlace;
    public GameObject placementIndicator;

    private XROrigin arOrigin;
    private ARRaycastManager raycastManager;

    private Pose placementPose;
    private bool placementPoseIsValid = false;
    private GameObject spawnedObject;

    void Start()
    {
        // ✅ Updated object finder API
        arOrigin = Object.FindFirstObjectByType<XROrigin>();
        raycastManager = Object.FindFirstObjectByType<ARRaycastManager>();

        if (placementIndicator != null)
        {
            placementIndicator.SetActive(false);

            // Ensure placement indicator has a collider
            if (placementIndicator.GetComponent<Collider>() == null)
            {
                var collider = placementIndicator.AddComponent<BoxCollider>();
                collider.isTrigger = true;
            }
        }
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (placementPoseIsValid)
        {
            HandleTouchInput();
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began) return;

        // Raycast from touch to detect if indicator was tapped
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform == placementIndicator.transform)
            {
                PlaceObject();
            }
        }
    }

    private void PlaceObject()
    {
        if (objectToPlace == null || !placementPoseIsValid) return;

        if (spawnedObject == null)
        {
            spawnedObject = Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
        }
        else
        {
            spawnedObject.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
    }

    private void UpdatePlacementIndicator()
    {
        if (placementIndicator == null) return;

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

    private void UpdatePlacementPose()
    {
        var screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        var hits = new List<ARRaycastHit>();

        if (raycastManager != null)
            placementPoseIsValid = raycastManager.Raycast(screenCenter, hits, TrackableType.Planes);
        else
            placementPoseIsValid = false;

        if (placementPoseIsValid && hits.Count > 0)
        {
            placementPose = hits[0].pose;

            var cameraForward = Camera.main.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
}
