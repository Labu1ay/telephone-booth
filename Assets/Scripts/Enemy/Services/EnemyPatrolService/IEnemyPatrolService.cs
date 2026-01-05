using UnityEngine;

namespace TelephoneBooth.Enemy.Services
{
  public interface IEnemyPatrolService
  {
    Vector3 GetRandomPatrolPosition();
  }
}