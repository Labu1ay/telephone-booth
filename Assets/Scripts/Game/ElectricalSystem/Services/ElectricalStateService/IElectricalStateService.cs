using UniRx;

namespace TelephoneBooth.Game.ElectricalSystem.Services
{
  public interface IElectricalStateService
  {
    ReadOnlyReactiveProperty<ElectricalStateType> CurrentElectricalState { get; }
    void SetElectricalState(ElectricalStateType electricalState);
  }
}