using UniRx;

namespace TelephoneBooth.Game.ElectricalSystem.Services
{
  public interface ITumblerStateService
  {
    ReadOnlyReactiveProperty<TumblerStateType> CurrentTumblerState { get; }
    void SetTumblerState(TumblerStateType tumblerState);
  }
}