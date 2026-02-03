using TelephoneBooth.Game.ElectricalSystem.Configs;
using TelephoneBooth.Game.ElectricalSystem.Factory;
using TelephoneBooth.Game.ElectricalSystem.Services;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game.ElectricalSystem
{
  public class ElectricalSystemInstaller : MonoInstaller
  {
    [SerializeField] private FusePanelItemConfig _fusePanelItemConfig;
    
    public override void InstallBindings()
    {
      Container.Bind<FusePanelItemConfig>().FromInstance(_fusePanelItemConfig).AsSingle();

      Container.BindInterfacesAndSelfTo<FusesPanelItemFactory>().AsSingle();
      
      Container.BindInterfacesAndSelfTo<GeneratorStateService>().AsSingle();
      Container.BindInterfacesAndSelfTo<FusesPanelService>().AsSingle();
    }
  }
}