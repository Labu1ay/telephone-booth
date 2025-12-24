using Zenject;

namespace TelephoneBooth.Player.Services
{
  public class PlayerServicesInstaller : MonoInstaller  
  {
    public override void InstallBindings()
    {
      Container.BindInterfacesAndSelfTo<PlayerMovementService>().AsSingle();
    }
  }
}