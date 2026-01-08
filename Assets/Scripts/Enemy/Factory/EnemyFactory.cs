using Cysharp.Threading.Tasks;
using TelephoneBooth.Core.Services;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Enemy.Factory
{
  public class EnemyFactory : IEnemyFactory
  {
    private const string ENEMY_PATH = "Enemy";
    
    [Inject] private readonly IAssetService _assetService;
    [Inject] private readonly DiContainer _diContainer;

    public BaseEnemy Enemy { get; private set; }
    
    public void CreateEnemy(Vector3 position, Quaternion rotation)
    {
      Enemy = _assetService.Instantiate<BaseEnemy>(ENEMY_PATH, _diContainer, position, rotation);
    }

    public async UniTask<BaseEnemy> GetEnemyAsync()
    {
      await UniTask.WaitWhile(() => Enemy == null);
      return Enemy;
    }
  }
}