using System;
using UniRx;

namespace TelephoneBooth.Core.Services
{
  public class GameStateService : IGameStateService
  {
    private ReactiveProperty<GameStateType> _gameState = new ReactiveProperty<GameStateType>();
    public ReadOnlyReactiveProperty<GameStateType> GameState => _gameState.ToReadOnlyReactiveProperty();

    public event Action<GameStateType> GameStateStarted;
    public event Action<GameStateType> GameStateFinished;

    public void SetGameState(GameStateType gameState)
    {
      GameStateFinished?.Invoke(_gameState.Value);
      _gameState.Value = gameState;
      GameStateStarted?.Invoke(_gameState.Value);
    }
  }
}