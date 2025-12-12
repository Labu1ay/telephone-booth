using UnityEngine;

namespace TelephoneBooth.Player.Configs
{
  [CreateAssetMenu(fileName = "PlayerCameraConfig", menuName = "configs/Player/PlayerCameraConfig", order = 0)]
  public class PlayerCameraConfig : ScriptableObject
  {
    [field: Header("HeadBob Effect")]
    [field: SerializeField] public bool Enabled { get; private set; } = true;
    
    [field: Space]
    [field: SerializeField, Range(0.001f, 0.01f)] public float Amount { get; private set; } = 0.01f;
    [field: SerializeField, Range(10f, 30f)] public float Frequency { get; private set; } = 13f;
    [field: SerializeField, Range(100f, 10f)] public float Smooth { get; private set; } = 44.7f;
    
    [field: Space]
    [field: SerializeField] public bool EnabledRotationMovement { get; private set; } = true;
    [field: SerializeField, Range(40f, 4f)] public float RotationMovementSmooth { get; private set; } = 6.7f;
    [field: SerializeField, Range(1f, 10f)] public float RotationMovementAmount { get; private set; } = 4f;
    
    [field: Header("Movement Tilt")]
    [field: SerializeField, Range(0.05f, 2)] public float RotationAmount { get; private set; } = 0.2f;
    [field: SerializeField, Range(1f, 20)] public float RotationSmooth { get; private set; } = 6f;
    [field: SerializeField] public bool CanMovementFX { get; private set; } = true;
    [field: SerializeField, Range(0.1f, 2)] public float MovementAmount { get; private set; } = 0.6f;
  }
}