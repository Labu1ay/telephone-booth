using UniRx;

namespace TelephoneBooth.Player.Services
{
  public class PlayerVisibleService : IPlayerVisibleService
  {
    private ReactiveProperty<bool> _isVisible = new ReactiveProperty<bool>();
    public ReadOnlyReactiveProperty<bool> IsVisible => _isVisible.ToReadOnlyReactiveProperty();
    
    public void SetVisibleStatus(bool isVisible) => _isVisible.Value = isVisible;
  }
}