using UnityEngine;

namespace Player.Configs
{
  [CreateAssetMenu(fileName = "PlayerControllerConfig", menuName = "configs/Player/PlayerControllerConfig", order = 0)]
  public class PlayerControllerConfig : ScriptableObject
  {
    [field: Header("Movement Settings")]
    [field: SerializeField, Range(1, 10)] public float WalkingSpeed { get; private set; } = 3f;
    [field: SerializeField, Range(2, 20)] public float RunningSpeed { get; private set; } = 4f;
    [field: SerializeField, Range(0, 20)] public float JumpSpeed { get; private set; } = 6f;
    [field: SerializeField] public float TimeToRunning { get; private set; } = 2f;
    [field: SerializeField] public float Gravity { get; private set; } = 20f;

    [field: Header("Look Settings")]
    [field: SerializeField, Range(0.5f, 10)] public float LookSpeed { get; private set; } = 2f;
    [field: SerializeField, Range(10, 120)] public float LookXLimit { get; private set; } = 80f;
    [field: SerializeField] public float RunningFOV { get; private set; } = 70f;
    [field: SerializeField] public float SpeedToFOV { get; private set; } = 4f;

    [field: Header("Crouch Settings")]
    [field: SerializeField] public float CroughHeight { get; private set; } = 1f;
    [field: SerializeField, Range(0.1f, 5)] public float CroughSpeed { get; private set; } = 1f;
    
    [field: Header("ClimbSettings")] 
    [field: SerializeField] public bool CanClimbing { get; private set; } = true;
    [field: SerializeField, Range(1, 25)] public float Speed { get; private set; } = 12.11f;
    
    [field: Header("WallHiderSettings")] 
    [field: SerializeField] public bool CanHideDistanceWall { get; private set; } = true;
    [field: SerializeField, Range(0.1f, 5)] public float HideDistance { get; private set; } = 1.5f;
    
  }
}