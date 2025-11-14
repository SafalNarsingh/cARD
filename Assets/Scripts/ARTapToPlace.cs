using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ARTapToPlaceObject : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public GameObject objectToPlace;
    private GameObject placedObject;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Update()
    {
        if (objectToPlace == null) return;
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;

                if (placedObject == null)
                {
                    placedObject = Instantiate(objectToPlace, hitPose.position, hitPose.rotation);
                }
                else
                {
                    placedObject.transform.position = hitPose.position;
                }
            }
        }
    }

    // This method can be called when a new model is loaded
    public void SetObjectToPlace(GameObject newObj)
    {
        objectToPlace = newObj;
    }
}
