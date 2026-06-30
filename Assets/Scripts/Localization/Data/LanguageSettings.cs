using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TelephoneBooth.Localization.Data
{
  [Serializable]
  public struct LanguageSettings
  {
    public string LanguageText;
    [PreviewField] public Sprite LanguageFlagSprite;
  }
}