using UnityEngine;
using UnityEngine.Localization.Settings;
using System.Collections;

public class LanguageSwitcher : MonoBehaviour
{
    public static LanguageSwitcher Instance;
    void Start()
    {
        // Set default language to Nepali
        StartCoroutine(SetLocaleAtStart("ne-NP"));
    }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    // Coroutine ensures localization system is initialized before setting locale
    IEnumerator SetLocaleAtStart(string code)
    {
        yield return LocalizationSettings.InitializationOperation;

        var locales = LocalizationSettings.AvailableLocales.Locales;
        foreach (var locale in locales)
        {
            if (locale.Identifier.Code == code)
            {
                LocalizationSettings.SelectedLocale = locale;
                Debug.Log("Default Locale set to: " + code);
                yield break;
            }
        }

        Debug.LogWarning("Locale with code '" + code + "' not found at startup.");
    }

    public void ToggleLanguage()
    {
        //if (ModelPlacer.Instance != null)
        //{
        //    ModelPlacer.Instance.UpdateModelText();
        //}
        // Get current locale code
        string currentCode = LocalizationSettings.SelectedLocale.Identifier.Code;

        // Decide which code to switch to
        string newCode = currentCode == "ne-NP" ? "en" : "ne-NP";

        SetLocaleByCode(newCode);
    }

    private void SetLocaleByCode(string code)
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;
        foreach (var locale in locales)
        {
            if (locale.Identifier.Code == code)
            {
                LocalizationSettings.SelectedLocale = locale;
                Debug.Log("Locale set to: " + code);
                return;
            }
        }

        Debug.LogWarning("Locale with code '" + code + "' not found.");
    }

    public bool IsNepali()
    {
        string currentCode = LocalizationSettings.SelectedLocale?.Identifier.Code ?? "en";
        return currentCode == "ne-NP";
    }

}
