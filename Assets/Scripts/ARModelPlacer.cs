using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class ARModelPlacer : MonoBehaviour
{
    [Header("AR Components")]
    public ARRaycastManager raycastManager;
    public GameObject placementIndicator;

    [Header("UI Elements")]
    public Button leftButton;
    public Button rightButton;
    public Button closeButton;
    public Button resetButton;
    public Button soundButton;
    public TextMeshProUGUI labelText;

    [Header("Models & Sounds")]
    public List<GameObject> models;
    public List<AudioSource> sounds;
    public List<string> labels;

    private GameObject currentModel;
    private int currentIndex = 0;

    private Pose placementPose;
    private bool placementPoseIsValid = false;
    private bool modelPlaced = false;
    private Vector3 currentScale = Vector3.one;
    private Quaternion currentRotation = Quaternion.identity;

    void Start()
    {
        if (leftButton) leftButton.onClick.AddListener(OnPreviousModel);
        if (rightButton) rightButton.onClick.AddListener(OnNextModel);
        if (closeButton) closeButton.onClick.AddListener(RemoveModel);
        if (resetButton) resetButton.onClick.AddListener(ResetTransform);
        if (soundButton) soundButton.onClick.AddListener(PlaySound);

        placementIndicator.SetActive(false);
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        // Place model when screen tapped (center placement)
        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceModel();
        }

        // Rotate model (two-finger twist) and scale (pinch)
        if (modelPlaced && currentModel != null)
        {
            HandleTouchGestures();
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            // Align indicator with plane surface
            var cameraForward = Camera.main.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid && !modelPlaced)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void PlaceModel()
    {
        // If a model already exists, move it instead of creating new one
        if (currentModel != null)
        {
            currentModel.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            currentModel = Instantiate(models[currentIndex], placementPose.position, placementPose.rotation);
        }

        modelPlaced = true;
        currentModel.transform.localScale = currentScale;
        currentModel.transform.rotation = currentRotation;

        UpdateUI();
    }

    private void RemoveModel()
    {
        if (currentModel != null)
        {
            Destroy(currentModel);
            currentModel = null;
            modelPlaced = false;
        }

        placementIndicator.SetActive(true);
        UpdateUI();
    }

    private void OnNextModel()
    {
        if (models.Count == 0) return;
        currentIndex = (currentIndex + 1) % models.Count;
        ReplaceModel();
    }

    private void OnPreviousModel()
    {
        if (models.Count == 0) return;
        currentIndex = (currentIndex - 1 + models.Count) % models.Count;
        ReplaceModel();
    }

    private void ReplaceModel()
    {
        if (currentModel != null)
        {
            Destroy(currentModel);
        }

        currentModel = Instantiate(models[currentIndex], placementPose.position, placementPose.rotation);
        currentModel.transform.localScale = Vector3.one;
        currentScale = Vector3.one;
        currentRotation = Quaternion.identity;

        UpdateUI();
    }

    private void PlaySound()
    {
        if (sounds.Count > currentIndex && sounds[currentIndex] != null)
        {
            sounds[currentIndex].Play();
        }
    }

    private void ResetTransform()
    {
        if (currentModel != null)
        {
            currentModel.transform.localScale = Vector3.one;
            currentScale = Vector3.one;
            currentModel.transform.rotation = Quaternion.identity;
            currentRotation = Quaternion.identity;
        }
    }

    private void UpdateUI()
    {
        if (labels.Count > currentIndex)
            labelText.text = labels[currentIndex];
    }

    private void HandleTouchGestures()
    {
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // Scale gesture
            Vector2 prevTouch0Pos = touch0.position - touch0.deltaPosition;
            Vector2 prevTouch1Pos = touch1.position - touch1.deltaPosition;
            float prevMagnitude = (prevTouch0Pos - prevTouch1Pos).magnitude;
            float currentMagnitude = (touch0.position - touch1.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;
            float scaleFactor = 1 + difference * 0.001f;
            currentScale *= scaleFactor;
            currentModel.transform.localScale = currentScale;

            // Rotate gesture
            Vector2 prevDir = prevTouch1Pos - prevTouch0Pos;
            Vector2 currDir = touch1.position - touch0.position;
            float angle = Vector2.SignedAngle(prevDir, currDir);
            currentModel.transform.Rotate(Vector3.up, -angle);
            currentRotation = currentModel.transform.rotation;
        }
    }
}
