//using System.Collections;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class ModelPlacer : MonoBehaviour
//{
//    [Header("Model Sections")]
//    public ModelSection[] sections = new ModelSection[4];

//    [Header("Section Canvas UI (Objects/Animals/Vehicles/Plants selection)")]
//    public Button[] sectionButtons = new Button[4]; // Objects, Animals, Vehicles, Plants

//    [Header("AR Canvas UI (Model placement and navigation)")]
//    public Button leftButton;           // Navigate to previous model
//    public Button rightButton;         // Navigate to next model
//    public Button unloadButton;        // Remove placed models
//    public Button backButton;          // Return to section selection
//    public TextMeshProUGUI modelNameText; // Shows current model name

//    [Header("Canvas Management")]
//    public GameObject sectionCanvas;    // Section selector canvas
//    public GameObject arCanvas;         // AR placer canvas

//    [Header("AR Components")]
//    public Transform placementIndicator; // Placement indicator prefab reference
//    public ARTapToPlace arTapToPlace;   // AR placement controller

//    [Header("Current State")]
//    [SerializeField] private int currentSectionIndex = -1;
//    [SerializeField] private int currentModelIndex = 0;
//    private GameObject currentPlacedModel;
//    private ModelData currentModelData;

//    public static ModelPlacer Instance;

//    private void Awake()
//    {
//        Instance = this;
//        InitializeSections();
//    }

//    private void Start()
//    {
//        SetupUI();
//        SetInitialCanvasState();
//        UpdateUI();
//    }

//    private void InitializeSections()
//    {
//        // Initialize section names
//        if (sections.Length >= 4)
//        {
//            sections[0].sectionName = "Objects";
//            sections[1].sectionName = "Animals";
//            sections[2].sectionName = "Vehicles";
//            sections[3].sectionName = "Plants";
//        }
//    }

//    private void SetupUI()
//    {
//        // Setup section buttons (from section canvas)
//        for (int i = 0; i < sectionButtons.Length; i++)
//        {
//            int sectionIndex = i; // Capture for closure
//            if (sectionButtons[i] != null)
//            {
//                sectionButtons[i].onClick.AddListener(() => SelectSection(sectionIndex));
//            }
//        }

//        // Setup AR canvas navigation buttons
//        if (leftButton != null)
//            leftButton.onClick.AddListener(PreviousModel);
//        if (rightButton != null)
//            rightButton.onClick.AddListener(NextModel);
//        if (unloadButton != null)
//            unloadButton.onClick.AddListener(UnloadModel);
//        if (backButton != null)
//            backButton.onClick.AddListener(SwitchToSectionCanvas);
//    }

//    private void SetInitialCanvasState()
//    {
//        // Start with section canvas active, AR canvas inactive
//        if (sectionCanvas != null)
//            sectionCanvas.SetActive(true);
//        if (arCanvas != null)
//            arCanvas.SetActive(false);
//    }

//    public void SelectSection(int sectionIndex)
//    {
//        if (sectionIndex < 0 || sectionIndex >= sections.Length)
//        {
//            Debug.LogError($"Invalid section index: {sectionIndex}");
//            return;
//        }

//        currentSectionIndex = sectionIndex;
//        currentModelIndex = 0;

//        Debug.Log($"Selected section: {sections[sectionIndex].sectionName}");

//        // Switch to AR Canvas
//        SwitchToARCanvas();

//        // Load first model of selected section
//        LoadCurrentModel();
//        UpdateUI();
//    }

//    public void NextModel()
//    {
//        if (currentSectionIndex == -1)
//        {
//            Debug.LogWarning("No section selected!");
//            return;
//        }

//        currentModelIndex = (currentModelIndex + 1) % sections[currentSectionIndex].models.Length;
//        LoadCurrentModel();
//        UpdateUI();

//        Debug.Log($"Next model: {currentModelData?.englishName}");
//    }

//    public void PreviousModel()
//    {
//        if (currentSectionIndex == -1)
//        {
//            Debug.LogWarning("No section selected!");
//            return;
//        }

//        currentModelIndex--;
//        if (currentModelIndex < 0)
//            currentModelIndex = sections[currentSectionIndex].models.Length - 1;

//        LoadCurrentModel();
//        UpdateUI();

//        Debug.Log($"Previous model: {currentModelData?.englishName}");
//    }

//    private void LoadCurrentModel()
//    {
//        if (currentSectionIndex == -1) return;

//        var currentSection = sections[currentSectionIndex];
//        if (currentModelIndex < currentSection.models.Length)
//        {
//            currentModelData = currentSection.models[currentModelIndex];

//            if (currentModelData != null && currentModelData.modelPrefab != null)
//            {
//                // Update the ARTapToPlace script with the new prefab
//                if (arTapToPlace != null)
//                {
//                    arTapToPlace.placedPrefab = currentModelData.modelPrefab;
//                }

//                Debug.Log($"Loaded model: {currentModelData.englishName} / {currentModelData.nepaliName}");
//            }
//            else
//            {
//                Debug.LogWarning($"Model data or prefab missing at section {currentSectionIndex}, model {currentModelIndex}");
//            }
//        }
//    }

//    public void UnloadModel()
//    {
//        // Clear any models placed by ARTapToPlace
//        if (arTapToPlace != null && arTapToPlace.spawnedObject != null)
//        {
//            Destroy(arTapToPlace.spawnedObject);
//            arTapToPlace.spawnedObject = null;
//            Debug.Log("Placed model unloaded");
//        }

//        // Clear any other placed models
//        if (currentPlacedModel != null)
//        {
//            Destroy(currentPlacedModel);
//            currentPlacedModel = null;
//            Debug.Log("Current model unloaded");
//        }
//    }

//    public void SwitchToARCanvas()
//    {
//        if (sectionCanvas != null)
//            sectionCanvas.SetActive(false);
//        if (arCanvas != null)
//            arCanvas.SetActive(true);

//        Debug.Log("Switched to AR Canvas");
//    }

//    public void SwitchToSectionCanvas()
//    {
//        if (arCanvas != null)
//            arCanvas.SetActive(false);
//        if (sectionCanvas != null)
//            sectionCanvas.SetActive(true);

//        // Reset current selection
//        currentSectionIndex = -1;
//        currentModelIndex = 0;
//        currentModelData = null;

//        // Clear any placed models
//        UnloadModel();

//        // Clear AR prefab reference
//        if (arTapToPlace != null)
//            arTapToPlace.placedPrefab = null;

//        Debug.Log("Switched to Section Canvas - State reset");
//    }

//    public void UpdateModelText()
//    {
//        if (currentModelData != null && modelNameText != null)
//        {
//            bool isNepali = LanguageSwitcher.Instance != null && LanguageSwitcher.Instance.IsNepali();
//            string modelName = currentModelData.GetLocalizedName(isNepali);
//            modelNameText.text = modelName;

//            Debug.Log($"Model text updated: {modelName}");
//        }
//        else if (modelNameText != null)
//        {
//            // Default empty text when no model is loaded
//            modelNameText.text = "";
//        }
//    }

//    private void UpdateUI()
//    {
//        // Update navigation buttons (only when AR canvas is active)
//        bool hasSection = currentSectionIndex != -1;
//        bool hasModels = hasSection && sections[currentSectionIndex].models.Length > 0;
//        bool isARCanvasActive = arCanvas != null && arCanvas.activeInHierarchy;

//        if (leftButton != null)
//            leftButton.interactable = hasModels && isARCanvasActive;
//        if (rightButton != null)
//            rightButton.interactable = hasModels && isARCanvasActive;
//        if (unloadButton != null)
//            unloadButton.interactable = isARCanvasActive;
//        if (backButton != null)
//            backButton.interactable = isARCanvasActive;

//        // Update model name text (only if AR canvas is active)
//        if (modelNameText != null && isARCanvasActive)
//        {
//            UpdateModelText();
//        }

//        // Highlight selected section button (only when section canvas is active)
//        bool isSectionCanvasActive = sectionCanvas != null && sectionCanvas.activeInHierarchy;
//        if (isSectionCanvasActive)
//        {
//            for (int i = 0; i < sectionButtons.Length; i++)
//            {
//                if (sectionButtons[i] != null)
//                {
//                    var colors = sectionButtons[i].colors;
//                    colors.normalColor = (i == currentSectionIndex) ? Color.green : Color.white;
//                    sectionButtons[i].colors = colors;
//                }
//            }
//        }
//    }

//    // Public methods for external access
//    public bool IsARCanvasActive()
//    {
//        return arCanvas != null && arCanvas.activeInHierarchy;
//    }

//    public string GetCurrentModelName()
//    {
//        if (currentModelData != null)
//        {
//            bool isNepali = LanguageSwitcher.Instance != null && LanguageSwitcher.Instance.IsNepali();
//            return currentModelData.GetLocalizedName(isNepali);
//        }
//        return "";
//    }

//    public int GetCurrentSectionIndex()
//    {
//        return currentSectionIndex;
//    }

//    public int GetCurrentModelIndex()
//    {
//        return currentModelIndex;
//    }

//    // Validation method to check setup
//    private void OnValidate()
//    {
//        // Validate sections
//        if (sections.Length != 4)
//        {
//            Debug.LogWarning("ModelPlacer: Should have exactly 4 sections (Objects, Animals, Vehicles, Plants)");
//        }

//        // Validate section buttons
//        if (sectionButtons.Length != 4)
//        {
//            Debug.LogWarning("ModelPlacer: Should have exactly 4 section buttons");
//        }

//        // Check for missing references
//        if (sectionCanvas == null)
//            Debug.LogWarning("ModelPlacer: Section Canvas not assigned!");
//        if (arCanvas == null)
//            Debug.LogWarning("ModelPlacer: AR Canvas not assigned!");
//        if (arTapToPlace == null)
//            Debug.LogWarning("ModelPlacer: AR Tap To Place not assigned!");
//    }
//}
