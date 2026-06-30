using System;
using I2.Loc;
using TelephoneBooth.Core.Services;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Localization.Services
{
  public class LocalizationService : ILocalizationService, IInitializable, ILateDisposable
  {
    private const string CURRENT_LANGUAGE_SAVING_KEY = "CurrentLanguage";
    
    private readonly ISavingService _savingService;

    private SaveContainer<string> _currentLanguage;
    private string[] _languages;

    public string CurrentLanguage => _currentLanguage.Item;
    
    public event Action LanguageChanged;

    [Inject]
    public LocalizationService(ISavingService savingService)
    {
      _savingService = savingService;
    }

    public void Initialize()
    {
      _languages = LocalizationManager.GetAllLanguages().ToArray();
      _currentLanguage = _savingService.GetPackage(CURRENT_LANGUAGE_SAVING_KEY, LocalizationManager.CurrentLanguage);

      LocalizationManager.CurrentLanguage = _currentLanguage.Item;
    }

    public void SetPreviousLanguage()
    {
      int index = Array.IndexOf(_languages, LocalizationManager.CurrentLanguage);
      
      if (index > 0)
        LocalizationManager.CurrentLanguage = _languages[index - 1];
      else if (index == 0) 
        LocalizationManager.CurrentLanguage = _languages[^1];

      _currentLanguage.Item = LocalizationManager.CurrentLanguage;
      LanguageChanged?.Invoke();
    }

    public void SetNextLanguage()
    {
      int index = Array.IndexOf(_languages, LocalizationManager.CurrentLanguage);
      
      if (index < _languages.Length - 1)
        LocalizationManager.CurrentLanguage = _languages[index + 1];
      else if (index == _languages.Length - 1) 
        LocalizationManager.CurrentLanguage = _languages[0];

      _currentLanguage.Item = LocalizationManager.CurrentLanguage;
      LanguageChanged?.Invoke();
    }


    public string GetTranslation(string term)
    {
      if (string.IsNullOrEmpty(term))
      {
        Debug.LogError("LocalizationService: Term is empty!");
        return "ERROR_TERM_IS_EMPTY";
      }

      string result = LocalizationManager.GetTranslation(term);

      if (!string.IsNullOrEmpty(result)) return result;

      Debug.LogError($"LocalizationService: No translation for '{term}'");
      return $"ERROR_NO_TRANSLATION_FOR_TERM_'{term}'";
    }

    public void LateDispose()
    {
      _savingService.SaveKey(CURRENT_LANGUAGE_SAVING_KEY);
    }
  }
}