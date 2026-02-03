using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TelephoneBooth.Core.Services;
using UniRx;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace TelephoneBooth.Game
{
  public class CameraMovementService : ICameraMovementService, IInitializable, ILateDisposable
  {
    private readonly IPlayerCameraProvider _playerCameraProvider;
    private readonly IGameStateService _gameStateService;

    private Transform _cameraRootTransform;
    private Transform _cameraStartPoint;
    
    private Sequence _sequence;
    private IDisposable _disposable;

    [Inject]
    public CameraMovementService(IPlayerCameraProvider playerCameraProvider, IGameStateService gameStateService)
    {
      _playerCameraProvider = playerCameraProvider;
      _gameStateService = gameStateService;
    }

    public async void Initialize()
    {
      await UniTask.WaitWhile(() => _playerCameraProvider.CameraRootTransform == null);
      _cameraRootTransform = _playerCameraProvider.CameraRootTransform;

      _disposable = _gameStateService.GameState.Subscribe(value =>
      {
        if (value != GameStateType.DEATH) return;
        _sequence?.Kill();
      });
    }

    public void SetCameraPoint(Transform target, float duration = 0.5f, Action callback = null)
    {
      CreateStartPoint();
      
      MoveCamera(_cameraRootTransform.position, target.position, target.rotation.eulerAngles,
        duration, 0f, callback);
    }

    public void SetCameraPointWithCurve(Transform target, float duration = 0.5f, float centerOffset = -0.5f,
      Action callback = null)
    {
      CreateStartPoint();

      MoveCamera(_cameraRootTransform.position, target.position, target.rotation.eulerAngles, duration,
        centerOffset, callback);
    }

    public void RollbackCamera(float duration = 0.5f, Action callback = null)
    {
      var startPoint = GetStartPositionAndEulerAngles();
      
      MoveCamera(_cameraRootTransform.position, startPoint.Item1,startPoint.Item2,
        duration, 0f, callback);
    }

    public void RollbackCameraWithCurve(float duration = 0.5f, float centerOffset = -0.5f, Action callback = null)
    {
      var startPoint = GetStartPositionAndEulerAngles();
      
      MoveCamera(_cameraRootTransform.position, startPoint.Item1, startPoint.Item2,
        duration, centerOffset, callback);
    }

    private void MoveCamera(Vector3 startPos, Vector3 endPos, Vector3 endRot,
      float duration, float centerOffset, Action callback)
    {
      _sequence?.Kill();

      _sequence = DOTween.Sequence();

      if (Mathf.Abs(centerOffset) > 0.001f)
      {
        Vector3 mid = (startPos + endPos) * 0.5f + Vector3.up * Vector3.Distance(startPos, endPos) * centerOffset;
        float t = 0f;
        _sequence.Append(DOTween.To(() => t, x =>
        {
          t = x;
          Vector3 a = Vector3.Lerp(startPos, mid, t);
          Vector3 b = Vector3.Lerp(mid, endPos, t);
          _cameraRootTransform.position = Vector3.Lerp(a, b, t);
        }, 1f, duration).SetEase(Ease.InOutQuad));
      }
      else
      {
        _sequence.Append(_cameraRootTransform.DOMove(endPos, duration).SetEase(Ease.InOutQuad));
      }

      _sequence.Join(_cameraRootTransform.DORotate(endRot, duration).SetEase(Ease.InOutQuad));
      _sequence.AppendCallback(() => _cameraRootTransform.position = endPos);
      _sequence.AppendInterval(Time.deltaTime);
      _sequence.OnComplete(() => callback?.Invoke());
    }

    private void CreateStartPoint()
    {
      _cameraStartPoint = new GameObject("CameraStartPointTemp").transform;
      _cameraStartPoint.parent = _cameraRootTransform.parent;
      _cameraStartPoint.position = _cameraRootTransform.position;
      _cameraStartPoint.rotation = _cameraRootTransform.rotation;

      _cameraRootTransform.parent = null;
    }

    private (Vector3, Vector3) GetStartPositionAndEulerAngles()
    {
      _cameraRootTransform.parent = _cameraStartPoint.parent;
      
      var result = (_cameraStartPoint.position, _cameraStartPoint.eulerAngles);
      Object.Destroy(_cameraStartPoint.gameObject);

      return result;
    }

    public void LateDispose()
    {
      _sequence?.Kill();
      _disposable?.Dispose();
    }
  }
}