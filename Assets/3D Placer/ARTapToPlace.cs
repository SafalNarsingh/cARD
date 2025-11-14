using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Localization.Settings;

[RequireComponent(typeof(ARRaycastManager))]
public class ARTapToPlace : MonoBehaviour
{
    [SerializeField] GameObject placementIndicator;
    [SerializeField] GameObject placedPrefab;

    [Header("Localization Text")]
    public string englishText;
    public string nepaliText;

    [Header("Localization Audio")]
    public AudioClip englishAudio;
    public AudioClip nepaliAudio;

    [Header("UI References")]
    public Button infoButton;
    public Button soundButton;
    public AudioSource audioSource;

    // ------------------ NEW: Real-time locale support ------------------
    private string _currentLocale;
    public string CurrentLocale
    {
        get => _currentLocale;
        private set
        {
            if (_currentLocale != value)
            {
                _currentLocale = value;
                UpdateLocalizedUI();
            }
        }
    }
    // -------------------------------------------------------------------

    GameObject spawnedObject;
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

        // Detect current locale at startup
        CurrentLocale = LocalizationSettings.SelectedLocale?.Identifier.Code ?? "en";

        controls = new TouchInput();
        controls.control.touch.performed += ctx =>
        {
            if (ctx.control.device is Pointer device)
            {
                OnPress(device.position.ReadValue());
            }
        };

        placementIndicator?.SetActive(false);

        if (infoButton != null) infoButton.gameObject.SetActive(false);
        if (soundButton != null) soundButton.gameObject.SetActive(false);
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

        // Detect if locale changed in real-time
        string localeCode = LocalizationSettings.SelectedLocale?.Identifier.Code ?? "en";
        if (localeCode != CurrentLocale)
        {
            CurrentLocale = localeCode;
        }
    }

    void OnPress(Vector3 position)
    {
        if (aRRaycastManager.Raycast(position, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;

            spawnedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation);
            Debug.Log($"✅ Object placed at: {hitPose.position}");

            if (arCamera != null)
            {
                Vector3 lookPos = arCamera.transform.position - spawnedObject.transform.position;
                lookPos.y = 0;
                spawnedObject.transform.rotation = Quaternion.LookRotation(lookPos);
            }

            ShowLocalizedUI();
        }
        else
        {
            Debug.LogWarning("No valid plane detected!");
        }
    }

    void ShowLocalizedUI()
    {
        if (infoButton != null)
        {
            infoButton.gameObject.SetActive(true);

            string finalText = CurrentLocale == "en" ? englishText : nepaliText;
            infoButton.GetComponentInChildren<Text>().text = finalText;
        }

        if (soundButton != null)
        {
            soundButton.gameObject.SetActive(true);

            soundButton.onClick.RemoveAllListeners();
            soundButton.onClick.AddListener(() =>
            {
                PlayLocalizedAudio();
            });
        }
    }

    void UpdateLocalizedUI()
    {
        if (infoButton != null)
        {
            string finalText = CurrentLocale == "en" ? englishText : nepaliText;
            infoButton.GetComponentInChildren<Text>().text = finalText;
        }
    }

    void PlayLocalizedAudio()
    {
        if (audioSource == null) return;

        AudioClip clipToPlay = CurrentLocale == "en" ? englishAudio : nepaliAudio;

        if (clipToPlay != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(clipToPlay);
        }
        else
        {
            Debug.LogWarning("No audio assigned for this locale!");
        }
    }

    private void UpdatePlacementIndicator()
    {
        if (placementIndicator == null) return;

        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        hits.Clear();

        if (aRRaycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
        {
            placementPoseIsValid = true;
            placementPose = hits[0].pose;

            if (arCamera != null)
            {
                Vector3 cameraForward = arCamera.transform.forward;
                Vector3 cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
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
