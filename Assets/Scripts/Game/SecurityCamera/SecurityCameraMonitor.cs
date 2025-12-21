using TelephoneBooth.Game.SecurityCamera.Data;
using TelephoneBooth.Game.SecurityCamera.Services;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game.SecurityCamera
{
  public class SecurityCameraMonitor : MonoBehaviour
  {
    [Inject] private readonly ISecurityCameraService _securityCameraService;
    
    [SerializeField] private Camera[] _securityCameras;
    [SerializeField] private MeshRenderer _monitorScreenRenderer;
    [SerializeField] private CanvasGroup _fadeMonitorGroup;

    private void Start()
    {
      _securityCameraService.Init(new SecurityCameraData(_securityCameras, _monitorScreenRenderer, _fadeMonitorGroup));
    }
  }
}