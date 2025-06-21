using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageSwitcher : MonoBehaviour
{
    public void SetNepali()
    {
        SetLocaleByCode("ne-NP");
    }

    public void SetEnglish()
    {
        SetLocaleByCode("en"); // or "en-US" if that’s what you used
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
}
