namespace TelephoneBooth.Game
{
  public interface IInteractiveCameraService
  {
    void AddHandleCamera(float lookXLimit = 10f, float lookYLimit = 25f);
    void RemoveHandleCamera();
  }
}