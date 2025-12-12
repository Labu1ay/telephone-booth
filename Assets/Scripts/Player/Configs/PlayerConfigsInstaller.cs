using Player.Configs;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Player.Configs
{
  public class PlayerConfigsInstaller : MonoInstaller
  {
    [SerializeField] private PlayerControllerConfig _playerControllerConfig;
    [SerializeField] private HandsHolderConfig _handsHolderConfig;
    [SerializeField] private HandsSmoothConfig _handsSmoothConfig;
    [SerializeField] private PlayerCameraConfig _playerCameraConfig;
    
    public override void InstallBindings()
    {
      Container.Bind<PlayerControllerConfig>().FromInstance(_playerControllerConfig).AsSingle();
      Container.Bind<HandsHolderConfig>().FromInstance(_handsHolderConfig).AsSingle();
      Container.Bind<HandsSmoothConfig>().FromInstance(_handsSmoothConfig).AsSingle();
      Container.Bind<PlayerCameraConfig>().FromInstance(_playerCameraConfig).AsSingle();
    }
  }
}