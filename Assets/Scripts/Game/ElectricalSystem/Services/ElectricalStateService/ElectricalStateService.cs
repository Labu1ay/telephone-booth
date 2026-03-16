using TelephoneBooth.Core.Services;
using UniRx;
using Zenject;

namespace TelephoneBooth.Game.ElectricalSystem.Services
{
  public class ElectricalStateService : IElectricalStateService, IInitializable
  {
    private const string ELECTRICAL_STATES_SAVING_KEY = "CurrentElectricalState";
    
    private readonly ISavingService _savingService;
    
    private SaveContainer<ElectricalStateType> _electricalStateSaveContainer;

    private ReactiveProperty<ElectricalStateType> _currentElectricalState = new ReactiveProperty<ElectricalStateType>();
    public ReadOnlyReactiveProperty<ElectricalStateType> CurrentElectricalState => _currentElectricalState.ToReadOnlyReactiveProperty();
    
    [Inject]
    public ElectricalStateService(ISavingService savingService)
    {
      _savingService = savingService;
    }

    public void Initialize()
    {
      _electricalStateSaveContainer = _savingService.GetPackage(ELECTRICAL_STATES_SAVING_KEY, ElectricalStateType.PowerIsOff);
      SetElectricalState(_electricalStateSaveContainer.Item);
    }

    public void SetElectricalState(ElectricalStateType electricalState)
    {
      _currentElectricalState.Value = electricalState;
      _electricalStateSaveContainer.Item = electricalState;
    }
  }
}