using System;

namespace TelephoneBooth.Localization.Services
{
  public interface ILocalizationService
  {
    event Action LanguageChanged;
    string CurrentLanguage { get; }
    string GetTranslation(string term);
    void SetPreviousLanguage();
    void SetNextLanguage();
  }
}