using Zenject;

namespace TelephoneBooth.Game
{
  public class InputServiceInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      Container.BindInterfacesAndSelfTo<InputService>().AsSingle();
    }
  }
}