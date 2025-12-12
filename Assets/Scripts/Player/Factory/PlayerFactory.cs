using Cysharp.Threading.Tasks;
using TelephoneBooth.Core.Services;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Player.Factory
{
  public class PlayerFactory : IPlayerFactory
  {
    private const string PLAYER_PATH = "Player";
    
    [Inject] private readonly IAssetService _assetService;
    [Inject] private readonly DiContainer _diContainer;

    public GameObject Player { get; private set; }
    
    public void CreatePlayer(Vector3 position, Quaternion rotation)
    {
      Player = _assetService.Instantiate(PLAYER_PATH, _diContainer, position, rotation);
    }

    public async UniTask<GameObject> GetPlayerAsync()
    {
      await UniTask.WaitWhile(() => Player == null);
      return Player;
    }
  }
}