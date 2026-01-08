using Cysharp.Threading.Tasks;
using TelephoneBooth.Enemy.Services;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Enemy.Components
{
  public class EnemyCatcher : MonoBehaviour
  {
    [Inject] private readonly IEnemyStateService _enemyStateService;
    [Inject] private readonly IEnemyHitService _enemyHitService;
    
    private void Start()
    {
      _enemyStateService.EnemyStateStarted += EnemyStateStarted;
    }

    private void EnemyStateStarted(EnemyStateType stateType)
    {
      if(stateType != EnemyStateType.CatchPlayer) return;
      _enemyHitService.Hit().Forget();
    }

    private void OnDestroy()
    {
      _enemyStateService.EnemyStateStarted -= EnemyStateStarted;
    }
  }
}