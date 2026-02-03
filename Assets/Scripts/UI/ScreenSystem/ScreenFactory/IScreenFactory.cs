namespace TelephoneBooth.UI.ScreenSystem
{
  public interface IScreenFactory
  {
    bool Has<T>() where T : Screen;
    Screen GetOrCreate<T>() where T : Screen;
    Screen Get<T>() where T : Screen;
    void Destroy<T>() where T : Screen;
  }
}