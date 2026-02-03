using UniRx;

namespace TelephoneBooth.Game.ElectricalSystem.Services
{
  public interface IGeneratorStateService
  {
    ReadOnlyReactiveProperty<GeneratorStateType> CurrentGeneratorState { get; }
    void SetGeneratorState(GeneratorStateType generatorState);
  }
}