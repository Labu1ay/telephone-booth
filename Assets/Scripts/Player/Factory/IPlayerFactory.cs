using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TelephoneBooth.Player.Factory
{
  public interface IPlayerFactory
  {
    GameObject Player { get; }
    CharacterController CharacterController { get; }
    void CreatePlayer(Vector3 position, Quaternion rotation);
    UniTask<GameObject> GetPlayerAsync();
  }
}