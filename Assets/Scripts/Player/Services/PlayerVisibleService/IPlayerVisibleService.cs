using UniRx;

namespace TelephoneBooth.Player.Services
{
  public interface IPlayerVisibleService
  {
    ReadOnlyReactiveProperty<bool> IsVisible { get; }
    void SetVisibleStatus(bool isVisible);
  }
}