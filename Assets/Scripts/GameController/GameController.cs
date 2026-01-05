using TelephoneBooth.Core.Services;
using TelephoneBooth.UI.Screens;
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
    
    private GameStateType _previousGameState;

    [Inject]
    public GameController(IInputService inputService, IGameStateService gameStateService, IScreenManager screenManager)
    {
      _inputService = inputService;
      _gameStateService = gameStateService;
      _screenManager = screenManager;
    }

    public void Initialize()
    {
      _inputService.PausedHandler += PauseHandler;
      _inputService.InventoryHandler += InventoryHandler;
      _gameStateService.GameStateStarted += GameStateStarted;
      _gameStateService.GameStateFinished += GameStateFinished;
      
      ResumeGame();
    }

    private void GameStateStarted(GameStateType state)
    {
      switch (state)
      {
        case GameStateType.GAME: 
          _screenManager.ShowScreen<GameScreen>(); 
          HideCursor();
          break;
        case GameStateType.PAUSE: _screenManager.ShowScreen<PauseScreen>(); break;
        case GameStateType.INTERACTIVE: _screenManager.ShowScreen<InteractiveScreen>(); break;
        case GameStateType.INVENTORY: _screenManager.ShowScreen<InventoryScreen>(); break;
        case GameStateType.DEATH: HideCursor(); break;
      }
    }
    
    private void GameStateFinished(GameStateType state)
    {
      switch (state)
      {
        case GameStateType.GAME: _screenManager.HideScreen<GameScreen>(); break;
        case GameStateType.PAUSE: _screenManager.DestroyScreen<PauseScreen>(); break;
        case GameStateType.INTERACTIVE: _screenManager.DestroyScreen<InteractiveScreen>(); break;
        case GameStateType.INVENTORY: _screenManager.DestroyScreen<InventoryScreen>(); break;
      }
    }

    private void PauseHandler()
    {
      if(_gameStateService.GameState.Value == GameStateType.DEATH) return;
      
      if (_gameStateService.GameState.Value == GameStateType.INVENTORY)
      {
        _gameStateService.SetGameState(GameStateType.GAME);
        HideCursor();
        return;
      }
      
      if (_gameStateService.GameState.Value != GameStateType.PAUSE)
      {
        _previousGameState = _gameStateService.GameState.Value;
        _gameStateService.SetGameState(GameStateType.PAUSE);
        PauseGame();
      }
      else
      {
        _gameStateService.SetGameState(_previousGameState);
        _previousGameState = default;
        ResumeGame();
      }
    }
    
    private void InventoryHandler()
    {
      if (_gameStateService.GameState.Value == GameStateType.GAME)
      {
        _gameStateService.SetGameState(GameStateType.INVENTORY);
        ShowCursor();
      }
      else if (_gameStateService.GameState.Value == GameStateType.INVENTORY)
      {
        _gameStateService.SetGameState(GameStateType.GAME);
        HideCursor();
      }
    }

    private void ResumeGame()
    {
      HideCursor();
      Time.timeScale = 1.0f;
    }

    private void PauseGame()
    {
      ShowCursor();
      Time.timeScale = 0.0f;
    }

    private static void ShowCursor()
    {
      Cursor.visible = true;
      Cursor.lockState = CursorLockMode.None;
    }
    
    private static void HideCursor()
    {
      Cursor.visible = false;
      Cursor.lockState = CursorLockMode.Locked;
    }

    public void LateDispose()
    {
      _inputService.PausedHandler -= PauseHandler;
      _inputService.InventoryHandler -= InventoryHandler;
      _gameStateService.GameStateStarted -= GameStateStarted;
      _gameStateService.GameStateFinished -= GameStateFinished;
    }
  }
}