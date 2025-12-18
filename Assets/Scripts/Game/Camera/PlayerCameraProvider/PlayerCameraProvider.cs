using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game
{
  public class PlayerCameraProvider : IPlayerCameraProvider, ILateDisposable
  {
    public Camera Camera { get; private set; }
    
    private Vector3 _startPosition;
    private Vector3 _startEulerAngles;

    private Sequence _sequence;
    
    public void SetCamera(Camera camera) => Camera = camera;

    public async UniTask<Camera> GetCameraAsync()
    {
      await UniTask.WaitWhile(() => Camera == null);
      return Camera;
    }

    public void SetCameraPoint(Transform point, float duration = 0.5f, Action callback = null)
    {
      _startPosition = Camera.transform.position;
      _startEulerAngles = Camera.transform.eulerAngles;
      
      _sequence?.Kill();
      _sequence = DOTween.Sequence()
        .Append(Camera.transform.DOMove(point.position, duration))
        .Join(Camera.transform.DORotate(point.rotation.eulerAngles, duration))
        .OnComplete(() => callback?.Invoke());
    }

    public void RollbackCamera(float duration = 0.5f, Action callback = null)
    {
      
      _sequence?.Kill();
      _sequence = DOTween.Sequence()
        .Append(Camera.transform.DOMove(_startPosition, duration))
        .Join(Camera.transform.DORotate(_startEulerAngles, duration))
        .OnComplete(() => callback?.Invoke());
    }

    public void LateDispose()
    {
      _sequence?.Kill();
    }
  }
}