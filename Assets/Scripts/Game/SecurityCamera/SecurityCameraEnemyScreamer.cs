using System;
using Cysharp.Threading.Tasks;
using TelephoneBooth.Enemy;
using TelephoneBooth.Enemy.EnemyScreamer.Factory;
using TelephoneBooth.Enemy.Factory;
using TelephoneBooth.Game.SecurityCamera.Services;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Game.SecurityCamera
{
  public class SecurityCameraEnemyScreamer : MonoBehaviour
  {
    [Inject] private readonly IEnemyFactory _enemyFactory;
    [Inject] private readonly IEnemyScreamerFactory _enemyScreamerFactory;
    [Inject] private readonly IEnemyVisibleService _enemyVisibleService;
    [Inject] private readonly ISecurityCameraService _securityCameraService;
    
    [SerializeField] private GameObject _chair;
    [SerializeField] private Transform _spawnPoint;

    private BaseEnemy _enemy;
    
    private async void Start()
    {
      _enemy = await _enemyFactory.GetEnemyAsync();
      _enemyVisibleService.DangerousTimeOvered += DangerousTimeOvered;
    }

    private async void DangerousTimeOvered()
    {
      _enemy.SetIdle();
      await _enemy.RotateTo(_securityCameraService.CurrentCamera.transform.position, withHead: true);
      await UniTask.Delay(TimeSpan.FromSeconds(0.35f));
      var enemy = await _enemyScreamerFactory.CreateEnemyScreamer(_spawnPoint.position, _spawnPoint.eulerAngles);
      _chair.SetActive(false);
      enemy.Hit();
      _enemy.gameObject.SetActive(false);
    }
    
    private void OnDestroy()
    {
      _enemyVisibleService.DangerousTimeOvered -= DangerousTimeOvered;
    }
  }
}