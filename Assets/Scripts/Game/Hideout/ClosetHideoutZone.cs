using Cysharp.Threading.Tasks;
using DG.Tweening;
using TelephoneBooth.Core.Services;
using TelephoneBooth.Player.Services;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game.Hideout
{
  public class ClosetHideoutZone : HideoutZone
  {
    [Inject] private readonly IPlayerMovementService _playerMovementService;
    
    [SerializeField] private Transform _closestDoorTransform;
    [SerializeField] private Vector3 _openEulerAngles;

    [SerializeField] private Transform _playerPoint;
    
    public override async void Interact()
    {
      base.Interact();

      await _playerMovementService.MoveToPosition(_playerPoint);
      
      await _closestDoorTransform.DOLocalRotate(_openEulerAngles, 0.4f).ToUniTask();
      
      _cameraMovementService.SetCameraPoint(_cameraPoint, callback: async () =>
      {
        await _closestDoorTransform.DOLocalRotate(Vector3.zero, 0.4f).ToUniTask();
        _inputService.InteractHandler += InteractHandler;
        _interactiveCameraService.AddHandleCamera(40f, 20f);
      });
    }

    private async void InteractHandler()
    {
      _inputService.InteractHandler -= InteractHandler;
      _interactiveCameraService.RemoveHandleCamera();
      
      await _closestDoorTransform.DOLocalRotate(_openEulerAngles, 0.4f).ToUniTask();
      
      _cameraMovementService.RollbackCamera(callback: async () =>
      {
        await _closestDoorTransform.DOLocalRotate(Vector3.zero, 0.4f).ToUniTask();
        _gameStateService.SetGameState(GameStateType.GAME);
      });
    }
  }
}