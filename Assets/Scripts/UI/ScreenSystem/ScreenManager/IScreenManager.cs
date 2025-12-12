namespace TelephoneBooth.UI.ScreenSystem
{
  public interface IScreenManager
  {
    void ShowScreen<T>() where T : Screen;
    void HideScreen<T>() where T : Screen;
    void DestroyScreen<T>() where T : Screen;
  }
}