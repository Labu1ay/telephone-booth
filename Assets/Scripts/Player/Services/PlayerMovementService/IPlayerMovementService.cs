using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TelephoneBooth.Player.Services
{
  public interface IPlayerMovementService
  {
    UniTask MoveToPosition(Transform needPoint, float duration = 0.5f);
  }
}