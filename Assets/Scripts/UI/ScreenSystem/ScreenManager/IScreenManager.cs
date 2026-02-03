namespace TelephoneBooth.UI.ScreenSystem
{
  public interface IScreenManager
  {
    bool HasActiveScreen<T>() where T : Screen;
    T ShowScreen<T>() where T : Screen;
    void HideScreen<T>() where T : Screen;
    void DestroyScreen<T>() where T : Screen;
  }
}