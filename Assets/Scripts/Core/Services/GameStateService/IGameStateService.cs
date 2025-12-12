using System;
using UniRx;

namespace TelephoneBooth.Core.Services
{
  public interface IGameStateService
  {
    event Action<GameStateType> GameStateStarted;
    event Action<GameStateType> GameStateFinished;
    ReadOnlyReactiveProperty<GameStateType> GameState { get; }
    void SetGameState(GameStateType gameState);
  }
}