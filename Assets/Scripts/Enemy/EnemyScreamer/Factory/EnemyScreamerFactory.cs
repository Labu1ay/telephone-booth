using Cysharp.Threading.Tasks;
using TelephoneBooth.Core.Services;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Enemy.EnemyScreamer.Factory
{
  public class EnemyScreamerFactory : IEnemyScreamerFactory
  {
    private const string ENEMY_PATH = "EnemyScreamer";
    
    [Inject] private readonly IAssetService _assetService;
    [Inject] private readonly DiContainer _diContainer;

    public BaseEnemyScreamer BaseEnemyScreamer { get; private set; }
    
    public async UniTask<BaseEnemyScreamer> CreateEnemyScreamer(Vector3 position, Vector3 eulerAngles)
    {
      BaseEnemyScreamer = await _assetService.InstantiateAsync<BaseEnemyScreamer>(ENEMY_PATH, _diContainer, position, Quaternion.Euler(eulerAngles));
      return BaseEnemyScreamer;
    }

    public async UniTask<BaseEnemyScreamer> GetEnemyScreamerAsync()
    {
      await UniTask.WaitWhile(() => BaseEnemyScreamer == null);
      return BaseEnemyScreamer;
    }
  }
}