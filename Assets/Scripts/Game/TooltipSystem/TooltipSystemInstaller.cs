using TelephoneBooth.Game.TooltipSystem.Services;
using Zenject;

namespace TelephoneBooth.Game.TooltipSystem
{
  public class TooltipSystemInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      Container.BindInterfacesAndSelfTo<TooltipService>().AsSingle();
    }
  }
}