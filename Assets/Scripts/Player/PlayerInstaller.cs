using TelephoneBooth.Player.Components;
using TelephoneBooth.Player.Hands;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Player
{
  public class PlayerInstaller : MonoInstaller
  {
    [SerializeField] private HandAnimator _handAnimator;
    [SerializeField] private Transform _cameraHolderTransform;
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private CharacterController _characterController;
    
    public override void InstallBindings()
    {
      Container.BindInstance(_handAnimator);
      Container.BindInstance(_characterController);
      Container.BindInstance(_playerController);
      Container.BindInstance(_cameraHolderTransform);
      Container.BindInstance(_playerCamera);
    }
  }
}