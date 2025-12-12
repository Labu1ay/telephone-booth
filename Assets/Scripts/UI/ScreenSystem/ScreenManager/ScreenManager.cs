using Zenject;

namespace TelephoneBooth.UI.ScreenSystem
{
  public class ScreenManager : IScreenManager
  {
    private readonly IScreenFactory _screenFactory;

    [Inject]
    public ScreenManager(IScreenFactory screenFactory)
    {
      _screenFactory = screenFactory;
    }

    public void ShowScreen<T>() where T : Screen
    {
      var screen = _screenFactory.GetOrCreate<T>();
      screen.gameObject.SetActive(true);
    }

    public void HideScreen<T>() where T : Screen
    {
      var screen = _screenFactory.GetOrCreate<T>();
      screen.gameObject.SetActive(false);
    }

    public void DestroyScreen<T>() where T : Screen => 
      _screenFactory.Destroy<T>();
  }
}