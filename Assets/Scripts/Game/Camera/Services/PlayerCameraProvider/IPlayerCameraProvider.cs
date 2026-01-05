using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TelephoneBooth.Game
{
  public interface IPlayerCameraProvider
  {
    Camera Camera { get; }
    void SetCamera(Camera camera);
    UniTask<Camera> GetCameraAsync();
  }
}