using TelephoneBooth.Core.Services;
using UniRx;
using Zenject;

namespace TelephoneBooth.Game.ElectricalSystem.Services
{
  public class TumblerStateService : ITumblerStateService, IInitializable
  {
    private const string TUMBLER_STATES_SAVING_KEY = "CurrentTumblerState";
    
    private readonly ISavingService _savingService;
    
    private SaveContainer<TumblerStateType> _tumblerStateSaveContainer;

    private ReactiveProperty<TumblerStateType> _currentTumblerState = new ReactiveProperty<TumblerStateType>();
    public ReadOnlyReactiveProperty<TumblerStateType> CurrentTumblerState => _currentTumblerState.ToReadOnlyReactiveProperty();

    [Inject]
    public TumblerStateService(ISavingService savingService)
    {
      _savingService = savingService;
    }

    public void Initialize()
    {
      _tumblerStateSaveContainer = _savingService.GetPackage(TUMBLER_STATES_SAVING_KEY, TumblerStateType.Inactive);
      SetTumblerState(_tumblerStateSaveContainer.Item);
    }

    public void SetTumblerState(TumblerStateType tumblerState)
    {
      _currentTumblerState.Value = tumblerState;
      _tumblerStateSaveContainer.Item = tumblerState;
    }
  }
}