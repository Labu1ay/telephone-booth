using TelephoneBooth.Game.Configs;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game
{
  public class CameraInstaller : MonoInstaller
  {
    [SerializeField] private CameraHitConfig _cameraHitConfig;
    
    public override void InstallBindings()
    {
      Container.Bind<CameraHitConfig>().FromInstance(_cameraHitConfig).AsSingle();
      
      Container.BindInterfacesAndSelfTo<PlayerCameraProvider>().AsSingle();
      Container.BindInterfacesAndSelfTo<CameraMovementService>().AsSingle();
      Container.BindInterfacesAndSelfTo<InteractiveCameraService>().AsSingle();
      Container.BindInterfacesAndSelfTo<CameraHitService>().AsSingle();
    }
  }
}