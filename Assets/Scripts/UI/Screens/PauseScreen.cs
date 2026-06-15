using System;
using TelephoneBooth.Core.Services;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Screen = TelephoneBooth.UI.ScreenSystem.Screen;

namespace TelephoneBooth.UI.Screens
{
  public class PauseScreen : Screen
  {
    [Inject] private readonly ISavingService _savingService;
    
    [SerializeField] private Button _saveButton;

    private IDisposable _disposable;

    private void Start()
    {
      _disposable = _saveButton.OnClickAsObservable().Subscribe(_ => _savingService.Save());
    }

    private void OnDestroy()
    {
      _disposable?.Dispose();
    }
  }
}