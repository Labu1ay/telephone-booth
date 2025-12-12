using TelephoneBooth.Core.Services;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Core
{
  public class Bootstrapper : MonoBehaviour {
        [Inject] private readonly ISceneLoaderService _sceneLoaderService;
        [Inject] private readonly IGameStateService _gameStateService;
        
        private void Start() {
            _gameStateService.SetGameState(GameStateType.INITIAL);
            _sceneLoaderService.Load(Constants.GAME_SCENE_NAME);
        }
    }
}