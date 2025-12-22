using Zenject;

namespace TelephoneBooth.Game.Interactable.Services
{
  public class InteractableServicesInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      Container.BindInterfacesAndSelfTo<InteractableTooltipService>().AsSingle();
    }
  }
}