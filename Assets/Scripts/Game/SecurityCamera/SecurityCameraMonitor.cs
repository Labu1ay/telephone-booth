using TelephoneBooth.Game.SecurityCamera.Data;
using TelephoneBooth.Game.SecurityCamera.Services;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game.SecurityCamera
{
  public class SecurityCameraMonitor : MonoBehaviour
  {
    [Inject] private readonly ISecurityCameraService _securityCameraService;
    
    [SerializeField] private CameraSecurity[] _securityCameras;
    [SerializeField] private MeshRenderer _monitorScreenRenderer;
    [SerializeField] private CanvasGroup _fadeMonitorGroup;

    [Space]
    [SerializeField] private GameObject _monitor;
    [SerializeField] private GameObject _brokenMonitor;

    private void Start()
    {
      _securityCameraService.Init(new SecurityCameraData(_securityCameras, _monitorScreenRenderer, _fadeMonitorGroup));

      _securityCameraService.CameraApplied += CameraApplied;
    }

    private void CameraApplied(bool isAvailable)
    {
      _monitor.SetActive(isAvailable);
      _brokenMonitor.SetActive(!isAvailable);
    }

    private void OnDestroy()
    {
      _securityCameraService.CameraApplied -= CameraApplied;
    }
  }
}