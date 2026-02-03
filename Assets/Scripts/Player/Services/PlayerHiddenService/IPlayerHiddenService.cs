using UniRx;

namespace TelephoneBooth.Player.Services
{
  public interface IPlayerHiddenService
  {
    ReadOnlyReactiveProperty<bool> IsHidden { get; }
    void SetHiddenStatus(bool isHidden);
  }
}