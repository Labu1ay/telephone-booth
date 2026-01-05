using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TelephoneBooth.Enemy.Factory
{
  public interface IEnemyFactory
  {
    GameObject Enemy { get; }
    void CreateEnemy(Vector3 position, Quaternion rotation);
    UniTask<GameObject> GetEnemyAsync();
  }
}