using TelephoneBooth.Core.Services;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Core
{
  public class GameBootstrapper : MonoBehaviour {
        [Inject] private readonly ISceneLoaderService _sceneLoaderService;
        
        private void Start() {
            _sceneLoaderService.Load(Constants.GAME_SCENE_NAME);
        }
    }
}