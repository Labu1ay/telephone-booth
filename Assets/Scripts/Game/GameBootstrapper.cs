using TelephoneBooth.Core.Services;
using TelephoneBooth.Enemy.Factory;
using TelephoneBooth.Player.Factory;
using TelephoneBooth.UI.Screens;
using TelephoneBooth.UI.ScreenSystem;
using Unity.AI.Navigation;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game
{
  public class GameBootstrapper : MonoBehaviour
  {
    [Inject] private readonly IPlayerFactory _playerFactory;
    [Inject] private readonly IEnemyFactory _enemyFactory;
    [Inject] private readonly IGameStateService _gameStateService;
    [Inject] private readonly IScreenManager _screenManager;
    [Inject] private readonly ISavingService _savingService;

    [SerializeField] private NavMeshSurface _navMeshSurface;
    
    private void Start()
    {
      _navMeshSurface.BuildNavMesh();
      
      _gameStateService.SetGameState(GameStateType.GAME);
      _playerFactory.CreatePlayer(Vector3.zero, Quaternion.identity);
      //_enemyFactory.CreateEnemy(new Vector3(0f, 0f, 4f), Quaternion.identity);
      _screenManager.ShowScreen<GameScreen>();
    }

    private void OnDestroy()
    {
      _savingService.ClearCache();
    }
  }
}