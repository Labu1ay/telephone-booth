using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TelephoneBooth.Game
{
  public class PlayerCameraProvider : IPlayerCameraProvider
  {
    public Camera Camera { get; private set; }
    public Transform CameraRootTransform { get; private set; }
    
    public void SetCamera(Camera camera)
    {
      Camera = camera;
      CameraRootTransform = camera.transform.parent.parent;
    }

    public async UniTask<Camera> GetCameraAsync()
    {
      await UniTask.WaitWhile(() => Camera == null);
      
      return Camera;
    }
  }
}