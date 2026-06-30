using TelephoneBooth.Localization.Configs;
using TelephoneBooth.Localization.Services;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TelephoneBooth.Localization.UI
{
  public class SwitchLocalizationPanel : MonoBehaviour
  {
    [Inject] private readonly ILocalizationService _localizationService;
    [Inject] private readonly LanguagesSettingsConfig _data;
    
    [SerializeField] private Text _currentLocalizationText;
    [SerializeField] private Image _currentLocalizationFlag;
    [SerializeField] private Button _previousButton;
    [SerializeField] private Button _nextButton;
    
    private CompositeDisposable _disposables = new CompositeDisposable();

    private void Start()
    {
      SetCurrentLanguageSettings();
      
      _previousButton
        .OnClickAsObservable().Subscribe(_ => _localizationService.SetPreviousLanguage())
        .AddTo(_disposables);
      
      _nextButton
        .OnClickAsObservable().Subscribe(_ => _localizationService.SetNextLanguage())
        .AddTo(_disposables);
      
      _localizationService.LanguageChanged += SetCurrentLanguageSettings;
    }

    private void SetCurrentLanguageSettings()
    {
      _currentLocalizationText.text = _data.GetCorrectLanguageText(_localizationService.CurrentLanguage);
      _currentLocalizationFlag.sprite = _data.GetLanguageFlagSprite(_localizationService.CurrentLanguage);
    }

    private void OnDestroy()
    {
      _disposables?.Clear();
      _localizationService.LanguageChanged -= SetCurrentLanguageSettings;
    }
  }
}