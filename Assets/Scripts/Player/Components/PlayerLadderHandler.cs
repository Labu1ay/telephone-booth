using System;
using Player.Configs;
using TelephoneBooth.Game;
using TelephoneBooth.Player.Hands;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Player.Components
{
  public class PlayerLadderHandler : MonoBehaviour
  {
    private const string LADDER_TAG = "Ladder";

    [Inject] private readonly IInputService _inputService;
    [Inject] private readonly PlayerControllerConfig _setup;
    [Inject] private readonly Transform _cameraTransform;
    [Inject] private readonly PlayerController _playerController;
    [Inject] private readonly HandAnimator _handAnimator;

    [SerializeField] private Collider _collider;
    
    private CompositeDisposable _disposables = new CompositeDisposable();
    private IDisposable _disposable;

    private void Start()
    {
      TriggerEnterSubscribe();
      TriggerStaySubscribe();
      TriggerExitSubscribe();
    }

    private void TriggerExitSubscribe()
    {
      _collider.OnTriggerExitAsObservable().Subscribe(other =>
      {
        if (!other.CompareTag(LADDER_TAG) || !_setup.CanClimbing) return;

        _playerController.SetRunningAvailable(true);
        _playerController.SetClimbingStatus(false);
        _playerController.SetWalkingValue(_playerController.WalkingValue * 2);
      
        _handAnimator.SetAnimation(HandAnimationType.Hide, false);
        
      }).AddTo(_disposables);
    }

    private void TriggerStaySubscribe()
    {
      _collider.OnTriggerStayAsObservable().Subscribe(other =>
      {
        if (!other.CompareTag(LADDER_TAG) || !_setup.CanClimbing) return;

        _playerController.SetMoveDirection(new Vector3(
          0,
          _inputService.Axis.y * _setup.Speed * (-_cameraTransform.localRotation.x / 1.7f),
          0));
        
      }).AddTo(_disposables);
    }

    private void TriggerEnterSubscribe()
    {
      _collider.OnTriggerEnterAsObservable().Subscribe(other =>
      {
        if (!other.CompareTag(LADDER_TAG) || !_setup.CanClimbing) return;

        _playerController.SetRunningAvailable(false);
        _playerController.SetClimbingStatus(true);
        _playerController.SetWalkingValue(_playerController.WalkingValue / 2);
      
        _handAnimator.SetAnimation(HandAnimationType.Hide, true);
        
      }).AddTo(_disposables);
    }

    private void OnDestroy()
    {
      _disposables?.Clear();
    }
  }
}