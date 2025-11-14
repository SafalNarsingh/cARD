using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ARUIManager : MonoBehaviour
{
    [Header("References")]
    public ARModelManager modelManager;

    [Header("UI Prefabs")]
    public GameObject modelButtonPrefab;
    public Transform buttonContainer;

    [Header("Control Buttons")]
    public Button soundButton;
    public Button deleteButton;
    public Button resetButton;

    [Header("Info Text")]
    public TextMeshProUGUI infoText;

    void Start()
    {
        CreateModelButtons();
        SetupControlButtons();
        UpdateInfoText("Point camera at a surface to place models");
    }

    void CreateModelButtons()
    {
        for (int i = 0; i < modelManager.models.Count; i++)
        {
            int index = i; // Capture for closure
            GameObject buttonObj = Instantiate(modelButtonPrefab, buttonContainer);

            Button btn = buttonObj.GetComponent<Button>();
            TextMeshProUGUI btnText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (btnText != null)
            {
                btnText.text = modelManager.models[i].modelName;
            }

            if (btn != null)
            {
                btn.onClick.AddListener(() => OnModelButtonClick(index));
            }
        }
    }

    void SetupControlButtons()
    {
        if (soundButton != null)
        {
            soundButton.onClick.AddListener(OnSoundButtonClick);
        }

        if (deleteButton != null)
        {
            deleteButton.onClick.AddListener(OnDeleteButtonClick);
        }

        if (resetButton != null)
        {
            resetButton.onClick.AddListener(OnResetButtonClick);
        }
    }

    void OnModelButtonClick(int index)
    {
        modelManager.SelectModel(index);
        UpdateInfoText($"Selected: {modelManager.models[index].modelName}\nTap on a surface to place");
    }

    void OnSoundButtonClick()
    {
        modelManager.PlaySound();
        UpdateInfoText("Playing sound...");
    }

    void OnDeleteButtonClick()
    {
        modelManager.DeleteCurrentObject();
        UpdateInfoText("Object deleted. Select a model to place a new one");
    }

    void OnResetButtonClick()
    {
        if (modelManager != null)
        {
            GameObject placedObj = GameObject.FindGameObjectWithTag("ARObject");
            if (placedObj != null)
            {
                ARObjectInteraction interaction = placedObj.GetComponent<ARObjectInteraction>();
                if (interaction != null)
                {
                    interaction.ResetTransform();
                    UpdateInfoText("Object reset to original transform");
                }
            }
        }
    }

    void UpdateInfoText(string message)
    {
        if (infoText != null)
        {
            infoText.text = message;
        }
    }
}