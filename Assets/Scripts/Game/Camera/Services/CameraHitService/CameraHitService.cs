using System;
using DG.Tweening;
using TelephoneBooth.Enemy;
using TelephoneBooth.Game.Configs;
using TelephoneBooth.Player.Factory;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game
{
  public class CameraHitService : ICameraHitService, IInitializable
  {
    private readonly IPlayerCameraProvider _playerCameraProvider;
    private readonly IPlayerFactory _playerFactory;
    private readonly CameraHitConfig _cameraHitConfig;

    private Transform _cameraTransform;
    private Transform _playerTransform;
    
    private Sequence _sequence;

    [Inject]
    public CameraHitService(IPlayerCameraProvider playerCameraProvider, IPlayerFactory playerFactory, CameraHitConfig cameraHitConfig)
    {
      _playerCameraProvider = playerCameraProvider;
      _playerFactory = playerFactory;
      _cameraHitConfig = cameraHitConfig;
    }

    public async void Initialize()
    {
      _cameraTransform = (await _playerCameraProvider.GetCameraAsync()).transform;
      _playerTransform = (await _playerFactory.GetPlayerAsync()).transform;
    }
    
    public void Hit(EnemyAnimationType enemyAnimationType, Action callback)
    {
      var settings = _cameraHitConfig.GetCameraHitSettings(enemyAnimationType);
      
      var startPosition = _cameraTransform.localPosition;
      var startRotation = _cameraTransform.localRotation;
      
      _sequence = DOTween.Sequence();

      _sequence.AppendInterval(settings.DelayToPush);
      _sequence.Append(
        _cameraTransform
          .DOLocalMove(startPosition + new Vector3(settings.HorizontalPush, settings.VerticalPush, 0), settings.PushDuration)
          .SetEase(Ease.OutQuad)
      );
        
      _sequence.Join(
        _cameraTransform
          .DOLocalRotate(new Vector3(settings.TiltAngle, settings.TiltAngle, -settings.TiltAngle), settings.PushDuration)
          .SetEase(Ease.OutQuad)
      );
        
      _sequence.Join(
        _cameraTransform
          .DOShakePosition(0.15f, new Vector3(settings.ShakeStrength * 0.5f, settings.ShakeStrength, 0), 20)
      );
        
      _sequence.Append(
        _cameraTransform
          .DOMoveY(_playerTransform.position.y + 0.5f, settings.FallDuration)
          .SetEase(Ease.InQuint)
      );
        
      _sequence.Join(
        _cameraTransform
          .DOLocalRotate(new Vector3(90f, startRotation.y, startRotation.z), settings.FallDuration)
          .SetEase(Ease.InQuint)
      );
      
      _sequence.OnComplete(() => callback?.Invoke());
    }
  }
}