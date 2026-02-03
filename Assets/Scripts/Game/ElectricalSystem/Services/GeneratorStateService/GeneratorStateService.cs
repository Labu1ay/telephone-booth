using TelephoneBooth.Core.Services;
using UniRx;
using Zenject;

namespace TelephoneBooth.Game.ElectricalSystem.Services
{
  public class GeneratorStateService : IGeneratorStateService, IInitializable
  {
    private const string GENERATOR_STATES_SAVING_KEY = "CurrentGeneratorState";
    
    private readonly ISavingService _savingService;
    
    private SaveContainer<GeneratorStateType> _generatorStateSaveContainer;

    private ReactiveProperty<GeneratorStateType> _currentGeneratorState = new ReactiveProperty<GeneratorStateType>();
    public ReadOnlyReactiveProperty<GeneratorStateType> CurrentGeneratorState => _currentGeneratorState.ToReadOnlyReactiveProperty();
    
    [Inject]
    public GeneratorStateService(ISavingService savingService)
    {
      _savingService = savingService;
    }

    public void Initialize()
    {
      _generatorStateSaveContainer = _savingService.GetPackage(GENERATOR_STATES_SAVING_KEY, GeneratorStateType.NoFuel);
      SetGeneratorState(_generatorStateSaveContainer.Item);
    }

    public void SetGeneratorState(GeneratorStateType generatorState)
    {
      _currentGeneratorState.Value = generatorState;
      _generatorStateSaveContainer.Item = generatorState;
    }
  }
}