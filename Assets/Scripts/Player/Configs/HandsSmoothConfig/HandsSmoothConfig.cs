using UnityEngine;

namespace TelephoneBooth.Player.Configs
{
  [CreateAssetMenu(fileName = "HandsSmoothConfig", menuName = "configs/Player/HandsSmoothConfig", order = 0)]
  public class HandsSmoothConfig : ScriptableObject
  {
    [field: Header("HandsSmooth")]
    [field: SerializeField, Range(1, 10)] public float Smooth { get; private set; } = 4f;
    [field: SerializeField, Range(0.001f, 1)] public float Amount { get; private set; } = 0.077f;
    [field: SerializeField, Range(0.001f, 1)] public float MaxAmount { get; private set; } = 0.1f;
        
    [field: Header("Rotation")]
    [field: SerializeField, Range(1, 10)] public float RotationSmooth { get; private set; } = 4f;
    [field: SerializeField, Range(0.1f, 10)] public float RotationAmount { get; private set; } = 4.31f;
    [field: SerializeField, Range(0.1f, 10)] public float MaxRotationAmount { get; private set; } = 5f;
    [field: SerializeField, Range(0.1f, 10)] public float RotationMovementMultiplier { get; private set; } = 3f;

    [field: Header("CroughRotation")]
    [field: SerializeField] public bool EnabledCroughRotation { get; private set; } = true;
    [field: SerializeField, Range(0.1f, 20)] public float RotationCroughSmooth { get; private set; } = 15.63f;
    [field: SerializeField, Range(5f, 50)] public float RotationCroughMultiplier { get; private set; } = 13.1f;
  }
}