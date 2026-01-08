using System;
using TelephoneBooth.Enemy.Factory;
using UniRx;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game.SecurityCamera.Services
{
  public class EnemyVisibleService : IEnemyVisibleService, IInitializable, ILateDisposable
  {
    private const float DANGER_TIME = 2f;
    
    private readonly IEnemyFactory _enemyFactory;
    
    private Transform _enemyTransform;
    private float _timer;
    
    private IDisposable _disposable;
    
    public event Action DangerousTimeOvered;

    [Inject]
    public EnemyVisibleService(IEnemyFactory enemyFactory)
    {
      _enemyFactory = enemyFactory;
    }

    public async void Initialize()
    {
      _enemyTransform = (await _enemyFactory.GetEnemyAsync()).transform;
    }
    
    public void InitCamera(Camera camera)
    {
      _disposable?.Dispose();

      _disposable = Observable.EveryUpdate().Subscribe(_ =>
      {
        if(CanCurrentlySeeEnemy(camera))
        {
          _timer += Time.deltaTime;

          if (_timer >= DANGER_TIME) 
            DangerousTimeOvered?.Invoke();
        }
      });
    }

    public void DisposeCamera()
    {
      _disposable?.Dispose();
      _timer = 0f;
    }
    
    
    private bool CanCurrentlySeeEnemy(Camera camera)
    {
      var cameraTransform = camera.transform;
      
      var toEnemy = _enemyTransform.position - cameraTransform.position;
    
      var angle = Vector3.Angle(cameraTransform.forward, toEnemy);
      if (angle > camera.fieldOfView * 0.5f)
        return false;
    
      if (Physics.Raycast(cameraTransform.position, toEnemy.normalized, out RaycastHit hit))
        return hit.collider.gameObject.layer == LayerMask.NameToLayer(Constants.ENEMY_LAYER);
    
      return false;
    }

    public void LateDispose()
    {
      _disposable?.Dispose();
    }
  }
}