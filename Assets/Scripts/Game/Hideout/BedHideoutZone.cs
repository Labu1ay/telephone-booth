using TelephoneBooth.Core.Services;

namespace TelephoneBooth.Game.Hideout
{
  public class BedHideoutZone : HideoutZone
  {
    public override void Interact()
    {
      base.Interact();
      
      _cameraMovementService.SetCameraPointWithCurve(_cameraPoint , callback: () =>
      {
        _inputService.InteractHandler += InteractHandler;
        _interactiveCameraService.AddHandleCamera(10f, 40f);
      });
    }
    
    private void InteractHandler()
    {
      _inputService.InteractHandler -= InteractHandler;
      _interactiveCameraService.RemoveHandleCamera();
      
      _cameraMovementService.RollbackCameraWithCurve(callback: () =>
      {
        _gameStateService.SetGameState(GameStateType.GAME);
      });
    }
  }
}