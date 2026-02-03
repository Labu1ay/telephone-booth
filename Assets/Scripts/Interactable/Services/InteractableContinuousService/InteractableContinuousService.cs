using System;
using Cysharp.Threading.Tasks;
using TelephoneBooth.Core.Services;
using TelephoneBooth.UI.Gameplay;
using TelephoneBooth.UI.Screens;
using TelephoneBooth.UI.ScreenSystem;
using UniRx;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace TelephoneBooth.Game.Interactable
{
  public class InteractableContinuousService : IInteractableContinuousService, IInitializable, ILateDisposable
  {
    private const string LOADING_INTERACTABLE_PATH = "LoadingInteractable";
    private const float ANGLE_OFFSET = 80f;
    private const float DISTANCE_OFFSET = 0.5f;
    
    private readonly IInputService _inputService;
    private readonly IPlayerCameraProvider _playerCameraProvider;
    private readonly IGameStateService _gameStateService;
    private readonly IAssetService _assetService;
    private readonly IScreenFactory _screenFactory;
    private readonly DiContainer _diContainer;
    
    private LoadingInteractable _loadingInteractable;
    
    private float _timer;
    private Transform _cameraTransform;
    private Camera _camera;
    
    private IInteractable _interactable;
    private Component _interactableComponent;
    
    private IDisposable _disposable;
    private IDisposable _gameStateDisposable;

    [Inject]
    public InteractableContinuousService(
      IInputService inputService,
      IPlayerCameraProvider playerCameraProvider,
      IGameStateService gameStateService,
      IAssetService assetService, 
      IScreenFactory screenFactory, 
      DiContainer diContainer)
    {
      _inputService = inputService;
      _playerCameraProvider = playerCameraProvider;
      _gameStateService = gameStateService;
      _assetService = assetService;
      _screenFactory = screenFactory;
      _diContainer = diContainer;
    }
    
    public async void Initialize()
    {
      _camera = await _playerCameraProvider.GetCameraAsync();
      _cameraTransform = _camera.transform;

      _gameStateDisposable = _gameStateService.GameState.Subscribe(_ => Cleanup());

    }

    public async UniTaskVoid InteractContinuous(float duration, Action onFinished)
    {
      _inputService.InteractHandler += OnInteractReleased;
      
      _disposable = Observable.EveryUpdate().Subscribe(_ =>
      {
        _timer += Time.deltaTime;
        
        if (_timer >= duration)
        {
          Cleanup();
          onFinished?.Invoke();
        }
      });
    }

    public async UniTaskVoid InteractContinuous(IInteractable interactable, float duration, Action onFinished)
    {
      _interactable = interactable;
      _interactableComponent = interactable as Component;
      
      await CreateLoadingInteractable();
      _loadingInteractable.Show();
      
      var startDistance = Vector3.Distance(_interactableComponent.transform.position, _cameraTransform.position);

      _inputService.InteractHandler += OnInteractReleased;
      
      _disposable = Observable.EveryUpdate().Subscribe(_ =>
      {
        _timer += Time.deltaTime;
        _interactable.Outline.ShowOutline();
        
          _loadingInteractable.transform.position =
            _camera.WorldToScreenPoint(CalculateWorldPosition());
          
        _loadingInteractable.SetProgress(_timer, duration);

        if (CheckDistance(startDistance) || CheckAngle(_interactableComponent.transform))
          Cleanup();
        
        if (_timer >= duration)
        {
          Cleanup();
          onFinished?.Invoke();
        }
      });
    }

    private bool CheckDistance(float startDistance) => 
      Vector3.Distance(_interactableComponent.transform.position, _cameraTransform.position) >= startDistance + DISTANCE_OFFSET;
    
    private bool CheckAngle(Transform transform)
    {
      var toCamera = transform.position -  _cameraTransform.position;
      var angle = Vector3.Angle( _cameraTransform.forward, toCamera);
      
      return angle > ANGLE_OFFSET * 0.5f;
    }

    private void OnInteractReleased(bool isInteracted)
    {
      if(isInteracted) return;
      Cleanup();
    }
    
    private void Cleanup()
    {
      _disposable?.Dispose();
      _inputService.InteractHandler -= OnInteractReleased;
      _interactable?.Outline.HideOutline();
      _timer = 0f;
      
      _interactable = null;
      _interactableComponent = null;
      
      RemoveLoadingInteractable();
    }

    private async UniTask<bool> CreateLoadingInteractable()
    {
      var screen = _screenFactory.Get<GameScreen>();
      
      if (screen == null || _loadingInteractable != null)
        return false;

      _loadingInteractable = await _assetService.InstantiateAsync<LoadingInteractable>(LOADING_INTERACTABLE_PATH, _diContainer, screen.transform);

      return true;
    }
    
    private void RemoveLoadingInteractable()
    {
      if(_loadingInteractable == null) return;
      
      Object.Destroy(_loadingInteractable.gameObject);
      _loadingInteractable = null;
    }
    
    private Vector3 CalculateWorldPosition()
    {
      Vector3 position = _interactableComponent.transform.position;
      Vector3 cameraNormal = _cameraTransform.forward;
      
      Vector3 vectorFromCamera = position - _cameraTransform.position;
      float camNormDot = Vector3.Dot(cameraNormal, vectorFromCamera.normalized);
      
      if (camNormDot <= 0f)
      {
        float camDot = Vector3.Dot(cameraNormal, vectorFromCamera);
        Vector3 proj = (cameraNormal * camDot * 1.01f);
        position = _cameraTransform.position + (vectorFromCamera - proj);
      }

      return position;
    }

    public void LateDispose()
    {
      Cleanup();
      _gameStateDisposable?.Dispose();
    }
  }
}