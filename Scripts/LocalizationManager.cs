using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LocalizationManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown languageDropdown;

    private List<Locale> availableLocales;
    private bool isInitialized;



    // 1. Subscribes to Localization events and initializes the dropdown if Localization is already ready.
    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;
        LocalizationSettings.InitializationOperation.Completed += OnLocalizationInitialized;

        if (LocalizationSettings.InitializationOperation.IsDone)
        {
            InitializeDropdown();
        }
    }

    // 2. Unsubscribes from Localization events to prevent unnecessary callbacks.
    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;
        LocalizationSettings.InitializationOperation.Completed -= OnLocalizationInitialized;
    }

    // 3. Removes the dropdown listener when this component is destroyed.
    private void OnDestroy()
    {
        languageDropdown.onValueChanged.RemoveListener(OnLanguageChanged);
    }

    // 4. Receives the Localization initialization result and initializes the dropdown.
    private void OnLocalizationInitialized(
        AsyncOperationHandle<LocalizationSettings> operation)
    {
        InitializeDropdown();
    }


    // 5. Retrieves available locales from LocalizationSettings, populates the dropdown with their names, and registers the selection listener.
    private void InitializeDropdown()
    {
        if (!isInitialized)
        {
            availableLocales = LocalizationSettings.AvailableLocales.Locales;

            languageDropdown.ClearOptions();

            List<string> languageNames = new List<string>();

            foreach (Locale locale in availableLocales)
            {
                languageNames.Add(locale.LocaleName);
            }

            languageDropdown.AddOptions(languageNames);
            languageDropdown.onValueChanged.AddListener(OnLanguageChanged);

            isInitialized = true;
        }

        UpdateDropdownSelection();
    }

    // 6. Applies the selected locale using the index provided by the dropdown.
    private void OnLanguageChanged(int index)
    {
        LocalizationSettings.SelectedLocale = availableLocales[index];
    }

    // 7. Detects locale changes from LocalizationSettings and synchronizes the dropdown selection.
    private void OnSelectedLocaleChanged(Locale locale)
    {
        UpdateDropdownSelection();
    }

    // 8. Finds the index of the currently selected locale and updates the dropdown without triggering another selection event.
    private void UpdateDropdownSelection()
    {
        if (isInitialized)
        {
            int selectedIndex = availableLocales.IndexOf(
                LocalizationSettings.SelectedLocale);

            if (selectedIndex >= 0)
            {
                languageDropdown.SetValueWithoutNotify(selectedIndex);
                languageDropdown.RefreshShownValue();
            }
        }
    }
}
