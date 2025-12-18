using System;
using TelephoneBooth.Core.Services;
using UniRx;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game.Interactable
{
  public class InteractableFinder : MonoBehaviour
  {
    private const float INTERACT_DISTANCE = 1.5f;
    
    [Inject] private readonly IGameStateService _gameStateService;
    [Inject] private readonly IPlayerCameraProvider _playerCameraProvider;
    [Inject] private readonly IInputService _inputService;

    private IInteractable _interactable;
    
    private Camera _camera;
    private Ray _ray;

    private IDisposable _disposable;

    private async void Start()
    {
      _camera = await _playerCameraProvider.GetCameraAsync();
      _disposable = Observable.EveryUpdate().Subscribe(_ => EveryUpdate());

      _inputService.InteractHandler += TryInteract;
    }

    private void EveryUpdate()
    {
      if (_gameStateService.GameState.Value != GameStateType.GAME)
      {
        ClearInteractable();
        return;
      }
      
      _ray = new Ray(_camera.transform.position, _camera.transform.forward);
        
      if (Physics.Raycast(_ray, out RaycastHit hit, Mathf.Infinity)) {
        float distance = Vector3.Distance(hit.collider.ClosestPointOnBounds(_camera.transform.position), _camera.transform.position);
        
        if (hit.collider.TryGetComponent(out IInteractable interactable) && distance < INTERACT_DISTANCE)
        {
          if(_interactable == interactable)
            return;
          
          _interactable?.Outline.HideOutline();
          _interactable = interactable;
          _interactable.Outline.ShowOutline();
        }
        else
        {
          ClearInteractable();
        }
      }
      else
      {
        ClearInteractable();
      }
    }
    
    private void ClearInteractable()
    {
      if(_interactable == null)
        return;
      
      _interactable?.Outline.HideOutline();
      _interactable = null;
    }

    private void TryInteract() => _interactable?.Interact();
    
    private void OnDestroy()
    {
      _disposable?.Dispose();
      _inputService.InteractHandler -= TryInteract;
    }
  }
}