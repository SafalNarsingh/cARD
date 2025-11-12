using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using Vuforia;

public class ScannerUIHandler : MonoBehaviour
{
    [Header("References")]
    public GameObject modelObject;     // 3D model for this card
    public Button textButton;          // Button showing the label text
    public Button soundButton;         // Button with sound icon
    public Button closeButton;         // Button to unrender the text and model
    public AudioSource audioSource1;    // Plays the sound
    public AudioSource audioSource2;    // Two audio sources to support multiple languages
    public string displayText;         // e.g., "Dog / Kukur"
    public string displayText2;

    private AudioSource audioSource;


    private ObserverBehaviour observerBehaviour;

    void Start()
    {
        observerBehaviour = GetComponent<ObserverBehaviour>();

        if (observerBehaviour)
        {
            observerBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
        }

        // Hide everything initially
        SetActiveState(false);
    }

    private void OnDestroy()
    {
        if (observerBehaviour)
        {
            observerBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus targetStatus)
    {
        // Vuforia considers TRACKED and EXTENDED_TRACKED as "found"
        bool isTracked =
            targetStatus.Status == Status.TRACKED ||
            targetStatus.Status == Status.EXTENDED_TRACKED;

        // Anything else means "lost"
        if (isTracked)
            OnTargetFound();
        else
            OnTargetLost();
    }

    private void OnTargetFound()
    {
        SetActiveState(true);

        // Determine locale (for example: "en-US" or "ne-NP")
        string currentLocale = LocalizationSettings.SelectedLocale?.Identifier.Code ?? "en";

        // Pick appropriate text
        string chosenText = currentLocale == "ne-NP" ? displayText2 : displayText;
        textButton.GetComponentInChildren<TextMeshProUGUI>().text = chosenText;


        audioSource = currentLocale == "ne-NP" ? audioSource2 : audioSource1;


        // Add sound functionality
        soundButton.onClick.RemoveAllListeners();
        soundButton.onClick.AddListener(() =>
        {
            if (audioSource != null)
                audioSource.Play();
        });
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() =>
        {
            OnTargetLost();
        });
    }

    private void OnTargetLost()
    {
        SetActiveState(false);

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }

    private void SetActiveState(bool state)
    {
        if (modelObject) modelObject.SetActive(state);
        if (textButton) textButton.gameObject.SetActive(state);
        if (soundButton) soundButton.gameObject.SetActive(state);
        if (closeButton) closeButton.gameObject.SetActive(state);
    }
}
