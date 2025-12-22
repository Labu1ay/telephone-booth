using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TelephoneBooth.UI.Screens;
using TelephoneBooth.UI.ScreenSystem;
using Zenject;

namespace TelephoneBooth.Game.Interactable.Services
{
  public class InteractableTooltipService : IInteractableTooltipService, ILateDisposable
  {
    private const float DELAY_TO_SHOW_TOOLTIP = 1.5f;
    
    [Inject] private readonly IScreenFactory _screenFactory;
    
    private GameScreen _gameScreen;
    private CancellationTokenSource _cts;

    [Inject]
    public InteractableTooltipService(IScreenFactory screenFactory)
    {
      _screenFactory = screenFactory;
    }

    public async UniTask TryShowTooltip(string tooltipText)
    {
      GetGameScreen();
      
      _cts = new CancellationTokenSource();
      
      var cancellationHandler = await UniTask
        .Delay(TimeSpan.FromSeconds(DELAY_TO_SHOW_TOOLTIP), cancellationToken:  _cts.Token)
        .SuppressCancellationThrow();
      
      if(cancellationHandler) return;
      
      _gameScreen.ShowTooltip(tooltipText);
    }

    public void HideTooltip()
    {
      TokenCancel();
      _gameScreen.HideTooltip();
    }
    
    private void GetGameScreen()
    {
      if (_gameScreen != null) return;
      _gameScreen = _screenFactory.Get<GameScreen>() as GameScreen;
    }

    private void TokenCancel()
    {
      _cts?.Cancel();
      _cts?.Dispose();
      _cts = null;
    }

    public void LateDispose()
    {
      TokenCancel();
    }
  }
}