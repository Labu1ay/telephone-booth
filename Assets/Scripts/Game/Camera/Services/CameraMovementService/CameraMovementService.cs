using System;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game
{
  public class CameraMovementService : IInitializable, ILateDisposable, ICameraMovementService
  {
    private readonly IPlayerCameraProvider _playerCameraProvider;

    private Camera _camera;
    private Vector3 _startPosition;
    private Vector3 _startEulerAngles;
    private Sequence _sequence;

    [Inject]
    public CameraMovementService(IPlayerCameraProvider playerCameraProvider)
    {
      _playerCameraProvider = playerCameraProvider;
    }

    public async void Initialize()
    {
      _camera = await _playerCameraProvider.GetCameraAsync();
    }

    public void SetCameraPoint(Transform target, float duration = 0.5f, Action callback = null) =>
      MoveCamera(_camera.transform.position, target.position,
        _camera.transform.eulerAngles, target.rotation.eulerAngles,
        duration, 0f, callback);

    public void SetCameraPointWithCurve(Transform target, float duration = 0.5f, float centerOffset = -0.5f,
      Action callback = null)
    {
      _startPosition = _camera.transform.position;
      _startEulerAngles = _camera.transform.eulerAngles;

      MoveCamera(_startPosition, target.position, _startEulerAngles, target.rotation.eulerAngles, duration,
        centerOffset, callback);
    }

    public void RollbackCamera(float duration = 0.5f, Action callback = null) =>
      MoveCamera(_camera.transform.position, _startPosition,
        _camera.transform.eulerAngles, _startEulerAngles,
        duration, 0f, callback);

    public void RollbackCameraWithCurve(float duration = 0.5f, float centerOffset = -0.5f, Action callback = null) =>
      MoveCamera(_camera.transform.position, _startPosition, 
        _camera.transform.eulerAngles, _startEulerAngles,
        duration, centerOffset, callback);

    private void MoveCamera(Vector3 startPos, Vector3 endPos, Vector3 startRot, Vector3 endRot,
      float duration, float centerOffset, Action callback)
    {
      _sequence?.Kill();

      _startPosition = startPos;
      _startEulerAngles = startRot;

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
          _camera.transform.position = Vector3.Lerp(a, b, t);
        }, 1f, duration).SetEase(Ease.InOutQuad));
      }
      else
      {
        _sequence.Append(_camera.transform.DOMove(endPos, duration).SetEase(Ease.InOutQuad));
      }

      _sequence.Join(_camera.transform.DORotate(endRot, duration).SetEase(Ease.InOutQuad));
      _sequence.OnComplete(() => callback?.Invoke());
    }

    public void LateDispose()
    {
      _sequence?.Kill();
    }
  }
}