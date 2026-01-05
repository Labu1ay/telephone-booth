using System;
using TelephoneBooth.Enemy;

namespace TelephoneBooth.Game
{
  public interface ICameraHitService
  {
    void Hit(EnemyAnimationType enemyAnimationType, Action callback);
  }
}