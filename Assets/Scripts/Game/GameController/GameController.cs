using TelephoneBooth.Core.Services;
using TelephoneBooth.UI;
using TelephoneBooth.UI.ScreenSystem;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game.GameController
{
  public class GameController : IInitializable, ILateDisposable
  {
    private readonly IInputService _inputService;
    private readonly IGameStateService _gameStateService;
    private readonly IScreenManager _screenManager;

    public GameController(IInputService inputService, IGameStateService gameStateService, IScreenManager screenManager)
    {
      _inputService = inputService;
      _gameStateService = gameStateService;
      _screenManager = screenManager;
    }

    public void Initialize()
    {
      _inputService.PausedHandler += PauseHandler;
      _gameStateService.GameStateStarted += GameStateStarted;
    }

    private void GameStateStarted(GameStateType state)
    {
      switch (state)
      {
        case GameStateType.GAME: ResumeGame(); break;
        case GameStateType.PAUSE: PauseGame(); break;
      }
    }

    private void PauseHandler()
    {
      switch (_gameStateService.GameState.Value)
      {
        case GameStateType.GAME: _gameStateService.SetGameState(GameStateType.PAUSE); break;
        case GameStateType.PAUSE: _gameStateService.SetGameState(GameStateType.GAME); break;
      }
    }

    private void ResumeGame()
    {
      Cursor.visible = false;
      Cursor.lockState = CursorLockMode.Locked;
      
      Time.timeScale = 1.0f;
      
      _screenManager.DestroyScreen<PauseScreen>();
      _screenManager.ShowScreen<GameScreen>();
    }

    private void PauseGame()
    {
      
      Cursor.visible = true;
      Cursor.lockState = CursorLockMode.None;
      
      Time.timeScale = 0.0f;
      
      _screenManager.HideScreen<GameScreen>();
      _screenManager.ShowScreen<PauseScreen>();
    }

    public void LateDispose()
    {
      _inputService.PausedHandler -= PauseHandler;
      _gameStateService.GameStateStarted -= GameStateStarted;
    }
  }
}