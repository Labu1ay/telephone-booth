using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TelephoneBooth.Enemy.EnemyScreamer.Factory
{
  public interface IEnemyScreamerFactory
  {
    BaseEnemyScreamer BaseEnemyScreamer { get; }
    UniTask<BaseEnemyScreamer> CreateEnemyScreamer(Vector3 position, Vector3 eulerAngles);
    UniTask<BaseEnemyScreamer> GetEnemyScreamerAsync();
  }
}