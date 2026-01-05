using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TelephoneBooth.Enemy;
using UnityEngine;

namespace TelephoneBooth.Game.Configs
{
  [CreateAssetMenu(fileName = "CameraHitConfig", menuName = "configs/CameraHitConfig", order = 0)]
  public class CameraHitConfig : SerializedScriptableObject
  {
    [OdinSerialize]
    private Dictionary<EnemyAnimationType, CameraHitSettings> _cameraHitsSettings = new ();

    public CameraHitSettings GetCameraHitSettings(EnemyAnimationType enemyAnimationType) =>
      _cameraHitsSettings.ContainsKey(enemyAnimationType) ? _cameraHitsSettings[enemyAnimationType] : default;
  }
}