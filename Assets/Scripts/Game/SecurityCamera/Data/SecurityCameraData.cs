using UnityEngine;

namespace TelephoneBooth.Game.SecurityCamera.Data
{
  public struct SecurityCameraData
  {
    public CameraSecurity[] SecurityCameras;
    public MeshRenderer MonitorScreenRenderer;
    public CanvasGroup FadeMonitorGroup;

    public SecurityCameraData(CameraSecurity[] securityCameras, MeshRenderer monitorScreenRenderer, CanvasGroup fadeMonitorGroup)
    {
      SecurityCameras = securityCameras;
      MonitorScreenRenderer = monitorScreenRenderer;
      FadeMonitorGroup = fadeMonitorGroup;
    }
  }
}