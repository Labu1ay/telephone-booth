using UnityEngine;
using Zenject;

namespace TelephoneBooth.Core.Configs
{
  public class CoreConfigsInstaller : MonoInstaller
  {
    [SerializeField] private AudioClipConfig _audioClipConfig;

    public override void InstallBindings() {
      Container.Bind<AudioClipConfig>().FromInstance(_audioClipConfig).AsSingle();
    }
  }
}