using UniRx;

namespace TelephoneBooth.Player.Services
{
  public class PlayerHiddenService : IPlayerHiddenService
  {
    private ReactiveProperty<bool> _isHidden = new ReactiveProperty<bool>();
    public ReadOnlyReactiveProperty<bool> IsHidden => _isHidden.ToReadOnlyReactiveProperty();
    
    public void SetHiddenStatus(bool isHidden) => _isHidden.Value = isHidden;
  }
}