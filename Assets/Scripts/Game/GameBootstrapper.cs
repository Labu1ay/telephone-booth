using TelephoneBooth.Core.Services;
using TelephoneBooth.Player.Factory;
using TelephoneBooth.UI.Screens;
using TelephoneBooth.UI.ScreenSystem;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game
{
  public class GameBootstrapper : MonoBehaviour
  {
    [Inject] private readonly IPlayerFactory _playerFactory;
    [Inject] private readonly IGameStateService _gameStateService;
    [Inject] private readonly IScreenManager _screenManager;
    
    private void Start()
    {
      _gameStateService.SetGameState(GameStateType.GAME);
      _playerFactory.CreatePlayer(Vector3.zero, Quaternion.identity);
      _screenManager.ShowScreen<GameScreen>();
    }
  }
}