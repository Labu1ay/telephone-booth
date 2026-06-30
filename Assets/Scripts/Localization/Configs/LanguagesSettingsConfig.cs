using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TelephoneBooth.Localization.Data;
using UnityEngine;

namespace TelephoneBooth.Localization.Configs
{
  [CreateAssetMenu(fileName = "LanguagesSettingsConfig", menuName = "configs/Localization/LanguagesSettingsConfig", order = 0)]
  public class LanguagesSettingsConfig : SerializedScriptableObject
  {
    [OdinSerialize] private Dictionary<string, LanguageSettings> _languageSettings = new Dictionary<string, LanguageSettings>();

    public string GetCorrectLanguageText(string language) => _languageSettings[language].LanguageText;
    
    public Sprite GetLanguageFlagSprite(string language)  => _languageSettings[language].LanguageFlagSprite;
  }
}