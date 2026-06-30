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
    private CancellationTokenSource _ctsTemporary;

    private bool _temporaryTooltipShowed;

    [Inject]
    public TooltipService(IScreenFactory screenFactory)
    {
      _screenFactory = screenFactory;
    }

    public async UniTask TryShowTooltip(string tooltipText, float delaySeconds = 5f)
    {
      HideTooltip();
      
      _cts ??= new CancellationTokenSource();
      var cancellationHandler = await UniTask.WaitWhile(() => _temporaryTooltipShowed, cancellationToken: _cts.Token)
        .SuppressCancellationThrow();
      
      if(cancellationHandler) return;
      await ShowTooltip(tooltipText, delaySeconds, _cts.Token);
    }

    public async UniTask TryShowTemporaryTooltip(string tooltipText, float delaySeconds = 0f, float durationSeconds = 1f)
    {
      if(string.IsNullOrEmpty(tooltipText)) return;
      
      _temporaryTooltipShowed = true;
      
      TokenCancel();
      if(_gameScreen != null) await _gameScreen.ForceHideTooltip();
      
      TemporaryTokenCancel();
      _ctsTemporary ??= new CancellationTokenSource();
      await ShowTooltip(tooltipText, delaySeconds, cancellationToken: _ctsTemporary.Token);
      
      var cancellationHandler = await UniTask.Delay(TimeSpan.FromSeconds(durationSeconds), cancellationToken: _ctsTemporary.Token)
        .SuppressCancellationThrow();
      
      if(cancellationHandler) return;

      _temporaryTooltipShowed = false;
      _gameScreen.HideTooltip();
    }

    private async UniTask ShowTooltip(string tooltipText, float delaySeconds, CancellationToken cancellationToken)
    {
      if(string.IsNullOrEmpty(tooltipText)) return;
      
      GetGameScreen();
      
      var cancellationHandler = await UniTask
        .Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken:  cancellationToken)
        .SuppressCancellationThrow();
      
      if(cancellationHandler) return;
      
      _gameScreen.ShowTooltip(tooltipText);
    }

   

    public void HideTooltip()
    {
      TokenCancel();
      
      if(_temporaryTooltipShowed) return;
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
    
    private void TemporaryTokenCancel()
    {
      _ctsTemporary?.Cancel();
      _ctsTemporary?.Dispose();
      _ctsTemporary = null;
    }

    public void LateDispose()
    {
      TokenCancel();
      TemporaryTokenCancel();
    }
  }
}