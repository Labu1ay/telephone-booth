using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TelephoneBooth.Game.SecurityCamera.Data;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace TelephoneBooth.Game.SecurityCamera.Services
{
  public class SecurityCameraService : ISecurityCameraService, IInitializable, ILateDisposable
  {
    private const int TEXTURE_WIDTH = 512;
    private const int TEXTURE_HEIGHT = 288;
    private const int DEPTH = 16;
    private const float FADE_DURATION = 0.25f;
    
    private readonly IInputService _inputService;
    private readonly IEnemyVisibleService _enemyVisibleService;

    private RenderTexture _renderTexture;
    private int _index;

    private Camera[] _securityCameras;
    private MeshRenderer _monitorScreenRenderer;
    private CanvasGroup _fadeMonitorGroup;

    private Tween _tween;
    private IDisposable _disposable;

    public Camera CurrentCamera =>
      _securityCameras != null && _securityCameras.Length > 0 ? _securityCameras[_index] : null;

    [Inject]
    private SecurityCameraService(IInputService inputService, IEnemyVisibleService enemyVisibleService)
    {
      _inputService = inputService;
      _enemyVisibleService = enemyVisibleService;
    }
    
    public void Initialize()
    {
      _enemyVisibleService.DangerousTimeOvered += DangerousTimeOvered;
    }

    public void Init(SecurityCameraData data)
    {
      _securityCameras = data.SecurityCameras;
      _monitorScreenRenderer = data.MonitorScreenRenderer;
      _fadeMonitorGroup = data.FadeMonitorGroup;

      if (_securityCameras == null) return;

      foreach (var securityCamera in _securityCameras)
        if (securityCamera != null)
          securityCamera.enabled = false;
    }

    private void LeftHandler() => ApplyCamera(Wrap(_index - 1));
    private void RightHandler() => ApplyCamera(Wrap(_index + 1));


    public void EnableMonitor()
    {
      _renderTexture = new RenderTexture(TEXTURE_WIDTH, TEXTURE_HEIGHT, DEPTH);

      _renderTexture.Create();
      _monitorScreenRenderer.material.mainTexture = _renderTexture;

      ApplyCamera(_index);

      _fadeMonitorGroup.DOFade(0f, FADE_DURATION);
      
      _inputService.LeftHandler += LeftHandler;
      _inputService.RightHandler += RightHandler;
      
    }

    private int Wrap(int i)
    {
      int n = _securityCameras.Length;
      i %= n;
      if (i < 0) i += n;
      return i;
    }

    private void ApplyCamera(int newIndex)
    {
      var oldCamera = CurrentCamera;
      if (oldCamera != null)
      {
        oldCamera.targetTexture = null;
        oldCamera.enabled = false;
      }

      _index = newIndex;

      var newCamera = CurrentCamera;
      if (newCamera != null)
      {
        newCamera.enabled = true;
        newCamera.targetTexture = _renderTexture;
        newCamera.aspect = (float)TEXTURE_WIDTH / TEXTURE_HEIGHT;
        
        _enemyVisibleService.InitCamera(newCamera);
      }
    }

    public async UniTask DisableMonitor()
    {
      _disposable?.Dispose();
      
      _inputService.LeftHandler -= LeftHandler;
      _inputService.RightHandler -= RightHandler;
      
      _enemyVisibleService.DisposeCamera();

      _tween = _fadeMonitorGroup.DOFade(1f, FADE_DURATION);
      await _tween.ToUniTask();

      DisposeMonitorAndCameras();
    }

    private void DisposeMonitorAndCameras()
    {
      _disposable?.Dispose();

      if (_securityCameras != null)
      {
        foreach (var securityCamera in _securityCameras)
        {
          if (securityCamera == null) continue;
          securityCamera.targetTexture = null;
          securityCamera.enabled = false;
        }
      }

      if (_renderTexture != null)
      {
        _renderTexture.Release();
        Object.Destroy(_renderTexture);
        _renderTexture = null;
      }
    }
    
    private void DangerousTimeOvered()
    {
      _inputService.LeftHandler -= LeftHandler;
      _inputService.RightHandler -= RightHandler;
      
      _enemyVisibleService.DisposeCamera();
    }

    public void LateDispose()
    {
      DisposeMonitorAndCameras();
      
      _enemyVisibleService.DangerousTimeOvered -= DangerousTimeOvered;
    }
  }
}