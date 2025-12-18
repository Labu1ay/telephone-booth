using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TelephoneBooth.Game
{
  public interface IPlayerCameraProvider
  {
    Camera Camera { get; }
    void SetCamera(Camera camera);
    UniTask<Camera> GetCameraAsync();
    void SetCameraPoint(Transform point, float duration = 0.5f, Action callback = null);
    void RollbackCamera(float duration = 0.5f, Action callback = null);
  }
}