using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TelephoneBooth.UI.Screens;
using TelephoneBooth.UI.ScreenSystem;
using Zenject;

namespace TelephoneBooth.Game.TooltipSystem.Services
{
  public class TooltipService : ITooltipService, ILateDisposable
  {
    [Inject] private readonly IScreenFactory _screenFactory;
    
    private GameScreen _gameScreen;
    private CancellationTokenSource _cts;

    [Inject]
    public TooltipService(IScreenFactory screenFactory)
    {
      _screenFactory = screenFactory;
    }

    public async UniTask TryShowTooltip(string tooltipText, float delaySeconds = 5f)
    {
      if(string.IsNullOrEmpty(tooltipText)) return;
      
      HideTooltip();
      GetGameScreen();
      
      _cts ??= new CancellationTokenSource();
      
      var cancellationHandler = await UniTask
        .Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken:  _cts.Token)
        .SuppressCancellationThrow();
      
      if(cancellationHandler) return;
      
      _gameScreen.ShowTooltip(tooltipText);
    }

    public async UniTask TryShowTemporaryTooltip(string tooltipText, float delaySeconds = 0f, float durationSeconds = 1f)
    {
      if(string.IsNullOrEmpty(tooltipText)) return;
      
      await TryShowTooltip(tooltipText, delaySeconds);
      
      _cts ??= new CancellationTokenSource();
      
      var cancellationHandler = await UniTask
        .Delay(TimeSpan.FromSeconds(durationSeconds), cancellationToken:  _cts.Token)
        .SuppressCancellationThrow();
      
      if(cancellationHandler) return;
      
      HideTooltip();
    }

    public void HideTooltip()
    {
      TokenCancel();
      _gameScreen?.HideTooltip();
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