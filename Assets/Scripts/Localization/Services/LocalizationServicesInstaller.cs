using TelephoneBooth.Localization.Configs;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Localization.Services
{
  public class LocalizationServicesInstaller : MonoInstaller
  {
    [SerializeField] private LanguagesSettingsConfig _languagesSettingsConfig;
    
    public override void InstallBindings()
    {
      Container.BindInterfacesAndSelfTo<LocalizationService>().AsSingle();
      Container.Bind<LanguagesSettingsConfig>().FromInstance(_languagesSettingsConfig).AsSingle();
    }
  }
}