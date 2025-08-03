using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ModelPlacer : MonoBehaviour
{
    [Header("Model Sections")]
    public ModelSection[] sections = new ModelSection[4];

    [Header("UI References")]
    public Button[] sectionButtons = new Button[4]; // Objects, Animals, Vehicles, Plants
    public Button leftButton;
    public Button rightButton;
    public Button unloadButton;
    public Text modelNameText;

    [Header("AR Components")]
    public Transform placementIndicator;
    public ARTapToPlace arTapToPlace;

    [Header("Current State")]
    public int currentSectionIndex = -1;
    public int currentModelIndex = 0;
    private GameObject currentPlacedModel;
    private ModelData currentModelData;

    public static ModelPlacer Instance;

    private void Awake()
    {
        Instance = this;
        InitializeSections();
    }

    private void Start()
    {
        SetupUI();
        UpdateUI();
    }

    private void InitializeSections()
    {
        // Initialize section names
        if (sections.Length >= 4)
        {
            sections[0].sectionName = "Objects";
            sections[1].sectionName = "Animals";
            sections[2].sectionName = "Vehicles";
            sections[3].sectionName = "Plants";
        }
    }

    private void SetupUI()
    {
        // Setup section buttons
        for (int i = 0; i < sectionButtons.Length; i++)
        {
            int sectionIndex = i; // Capture for closure
            if (sectionButtons[i] != null)
            {
                sectionButtons[i].onClick.AddListener(() => SelectSection(sectionIndex));
            }
        }

        // Setup navigation buttons
        if (leftButton != null)
            leftButton.onClick.AddListener(PreviousModel);
        if (rightButton != null)
            rightButton.onClick.AddListener(NextModel);
        if (unloadButton != null)
            unloadButton.onClick.AddListener(UnloadModel);
    }

    public void SelectSection(int sectionIndex)
    {
        if (sectionIndex < 0 || sectionIndex >= sections.Length)
            return;

        currentSectionIndex = sectionIndex;
        currentModelIndex = 0;

        Debug.Log($"Selected section: {sections[sectionIndex].sectionName}");

        LoadCurrentModel();
        UpdateUI();
    }

    public void NextModel()
    {
        if (currentSectionIndex == -1) return;

        currentModelIndex = (currentModelIndex + 1) % sections[currentSectionIndex].models.Length;
        LoadCurrentModel();
        UpdateUI();
    }

    public void PreviousModel()
    {
        if (currentSectionIndex == -1) return;

        currentModelIndex--;
        if (currentModelIndex < 0)
            currentModelIndex = sections[currentSectionIndex].models.Length - 1;

        LoadCurrentModel();
        UpdateUI();
    }

    private void LoadCurrentModel()
    {
        if (currentSectionIndex == -1) return;

        var currentSection = sections[currentSectionIndex];
        if (currentModelIndex < currentSection.models.Length)
        {
            currentModelData = currentSection.models[currentModelIndex];

            if (currentModelData != null && currentModelData.modelPrefab != null)
            {
                // Update the ARTapToPlace script with the new prefab
                if (arTapToPlace != null)
                {
                    arTapToPlace.placedPrefab = currentModelData.modelPrefab;
                }

                Debug.Log($"Loaded model: {currentModelData.englishName}");
            }
        }
    }

    public void UnloadModel()
    {
        if (currentPlacedModel != null)
        {
            Destroy(currentPlacedModel);
            currentPlacedModel = null;
            Debug.Log("Model unloaded");
        }

        // Also clear any models placed by ARTapToPlace
        if (arTapToPlace != null && arTapToPlace.spawnedObject != null)
        {
            Destroy(arTapToPlace.spawnedObject);
            arTapToPlace.spawnedObject = null;
        }
    }

    public void UpdateModelText()
    {
        if (currentModelData != null && modelNameText != null)
        {
            bool isNepali = LanguageSwitcher.Instance != null && LanguageSwitcher.Instance.IsNepali();
            modelNameText.text = currentModelData.GetLocalizedName(isNepali);
        }
    }

    private void UpdateUI()
    {
        // Update navigation buttons
        bool hasSection = currentSectionIndex != -1;
        bool hasModels = hasSection && sections[currentSectionIndex].models.Length > 0;

        if (leftButton != null)
            leftButton.interactable = hasModels;
        if (rightButton != null)
            rightButton.interactable = hasModels;
        if (unloadButton != null)
            unloadButton.interactable = true;

        // Update model name text
        if (modelNameText != null)
        {
            if (currentModelData != null)
            {
                UpdateModelText();
            }
            else
            {
                modelNameText.text = hasSection ? "Select a model" : "Choose a section";
            }
        }

        // Highlight selected section button
        for (int i = 0; i < sectionButtons.Length; i++)
        {
            if (sectionButtons[i] != null)
            {
                var colors = sectionButtons[i].colors;
                colors.normalColor = (i == currentSectionIndex) ? Color.green : Color.white;
                sectionButtons[i].colors = colors;
            }
        }
    }
}