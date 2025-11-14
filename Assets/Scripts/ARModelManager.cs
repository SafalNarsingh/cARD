using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ARModelManager : MonoBehaviour
{
    [System.Serializable]
    public class ModelData
    {
        public string modelName;
        public GameObject modelPrefab;
        public AudioClip modelSound;
    }

    [Header("AR Components")]
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;

    [Header("Model Data")]
    public List<ModelData> models = new List<ModelData>();

    [Header("Interaction Settings")]
    public float minScale = 0.1f;
    public float maxScale = 5f;
    public float rotationSpeed = 100f;

    private GameObject currentSelectedModel;
    private int currentModelIndex = 0;
    private GameObject placedObject;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    // Touch interaction variables
    private Vector2 previousTouchPos;
    private float previousPinchDistance;
    private bool isRotating = false;
    private bool isScaling = false;

    void Update()
    {
        // Handle model placement
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (!IsPointerOverUIObject())
            {
                TryPlaceModel(Input.GetTouch(0).position);
            }
        }

        // Handle interactions if object is placed
        if (placedObject != null)
        {
            HandleObjectInteraction();
        }
    }

    void TryPlaceModel(Vector2 touchPosition)
    {
        if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            if (placedObject == null && currentModelIndex < models.Count)
            {
                placedObject = Instantiate(models[currentModelIndex].modelPrefab, hitPose.position, hitPose.rotation);

                // Add interaction component
                ARObjectInteraction interaction = placedObject.AddComponent<ARObjectInteraction>();
                interaction.Initialize(minScale, maxScale, rotationSpeed);

                // Add audio source if sound exists
                if (models[currentModelIndex].modelSound != null)
                {
                    AudioSource audioSource = placedObject.AddComponent<AudioSource>();
                    audioSource.clip = models[currentModelIndex].modelSound;
                    audioSource.playOnAwake = false;
                }
            }
        }
    }

    void HandleObjectInteraction()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                previousTouchPos = touch.position;
                isRotating = true;
            }
            else if (touch.phase == TouchPhase.Moved && isRotating)
            {
                Vector2 delta = touch.position - previousTouchPos;
                placedObject.transform.Rotate(Vector3.up, -delta.x * rotationSpeed * Time.deltaTime, Space.World);
                previousTouchPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isRotating = false;
            }
        }
        else if (Input.touchCount == 2)
        {
            isRotating = false;
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
            {
                previousPinchDistance = Vector2.Distance(touch0.position, touch1.position);
                isScaling = true;
            }
            else if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
            {
                float currentPinchDistance = Vector2.Distance(touch0.position, touch1.position);
                float scaleFactor = currentPinchDistance / previousPinchDistance;

                Vector3 newScale = placedObject.transform.localScale * scaleFactor;
                newScale = Vector3.Max(Vector3.one * minScale, Vector3.Min(newScale, Vector3.one * maxScale));
                placedObject.transform.localScale = newScale;

                previousPinchDistance = currentPinchDistance;
            }
            else if (touch0.phase == TouchPhase.Ended || touch1.phase == TouchPhase.Ended)
            {
                isScaling = false;
            }
        }
    }

    public void SelectModel(int index)
    {
        if (index >= 0 && index < models.Count)
        {
            currentModelIndex = index;
            Debug.Log($"Selected model: {models[index].modelName}");
        }
    }

    public void PlaySound()
    {
        if (placedObject != null)
        {
            AudioSource audioSource = placedObject.GetComponent<AudioSource>();
            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.Play();
            }
        }
    }

    public void DeleteCurrentObject()
    {
        if (placedObject != null)
        {
            Destroy(placedObject);
            placedObject = null;
        }
    }

    bool IsPointerOverUIObject()
    {
        if (Input.touchCount > 0)
        {
            return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }
        return false;
    }
}