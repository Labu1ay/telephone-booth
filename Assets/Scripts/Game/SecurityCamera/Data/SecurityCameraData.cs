using UnityEngine;

namespace TelephoneBooth.Game.SecurityCamera.Data
{
  public struct SecurityCameraData
  {
    public Camera[] SecurityCameras;
    public MeshRenderer MonitorScreenRenderer;
    public CanvasGroup FadeMonitorGroup;

    public SecurityCameraData(Camera[] securityCameras, MeshRenderer monitorScreenRenderer, CanvasGroup fadeMonitorGroup)
    {
      SecurityCameras = securityCameras;
      MonitorScreenRenderer = monitorScreenRenderer;
      FadeMonitorGroup = fadeMonitorGroup;
    }
  }
}