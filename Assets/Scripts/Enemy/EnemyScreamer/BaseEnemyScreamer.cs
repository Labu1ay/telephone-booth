using Cysharp.Threading.Tasks;
using TelephoneBooth.Enemy.Services;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Enemy.EnemyScreamer
{
  public class BaseEnemyScreamer : MonoBehaviour
  {
    [Inject] private readonly IEnemyHitService _enemyHitService;

    public void Hit() => _enemyHitService.Hit().Forget();
  }
}