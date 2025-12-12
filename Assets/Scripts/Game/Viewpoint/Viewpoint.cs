using System;
using TelephoneBooth.Core.Services;
using TelephoneBooth.Player.Factory;
using UniRx;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game
{
  public class Viewpoint : MonoBehaviour
  {
    private const string VIEWPOINT_PATH = "ViewpointIcon";

    [Inject] private readonly IAssetService _assetService;
    [Inject] private readonly IPlayerCameraProvider _playerCameraProvider;
    [Inject] private readonly IPlayerFactory _playerFactory;
    [Inject] private readonly DiContainer _diContainer;

    [SerializeField] private string _text = "Press E";

    [Space] 
    [SerializeField, Range(0.1f, 20)] private float _maxViewRange = 5;
    [SerializeField, Range(0.1f, 20)] private float _maxTextViewRange = 2;

    private Camera _camera;
    private GameObject _player;
    private float _distance;
    private ViewpointIcon _viewpointIcon;

    private IDisposable _disposable;

    private async void Start()
    {
      _player = await _playerFactory.GetPlayerAsync();
      _camera = await _playerCameraProvider.GetCameraAsync();

      _viewpointIcon =
        _assetService.Instantiate<ViewpointIcon>(VIEWPOINT_PATH, _diContainer, FindObjectOfType<Canvas>().transform);
      _viewpointIcon.Init(_text);

      _disposable = Observable.EveryUpdate().Subscribe(_ => EveryUpdate());
    }

    private void EveryUpdate()
    {
      _viewpointIcon.transform.position =
        _camera.WorldToScreenPoint(CalculateWorldPosition(transform.position, _camera));
      _distance = Vector3.Distance(_player.transform.position, transform.position);

      if (_distance < _maxTextViewRange)
        _viewpointIcon.ShowText();
      else
        _viewpointIcon.HideText();

      if (_distance < _maxViewRange)
        _viewpointIcon.ShowImage();
      else
        _viewpointIcon.HideImage();
    }

    private Vector3 CalculateWorldPosition(Vector3 position, Camera camera)
    {
      Vector3 cameraNormal = camera.transform.forward;
      Vector3 vectorFromCamera = position - camera.transform.position;
      float camNormDot = Vector3.Dot(cameraNormal, vectorFromCamera.normalized);
      if (camNormDot <= 0f)
      {
        float camDot = Vector3.Dot(cameraNormal, vectorFromCamera);
        Vector3 proj = (cameraNormal * camDot * 1.01f);
        position = camera.transform.position + (vectorFromCamera - proj);
      }

      return position;
    }


    private void OnDestroy()
    {
      _disposable?.Dispose();
    }
  }
}