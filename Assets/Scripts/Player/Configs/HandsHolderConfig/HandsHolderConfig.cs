using UnityEngine;

namespace TelephoneBooth.Player.Configs
{
  [CreateAssetMenu(fileName = "HandsHolderConfig", menuName = "configs/Player/HandsHolderConfig", order = 0)]
  public class HandsHolderConfig : ScriptableObject
  {
    [field: Header("HandsHolder")]
    [field: SerializeField] public bool Enabled { get; private set; } = true;

    [field: Header("Main")]
    [field: SerializeField, Range(0.0005f, 0.02f)] public float Amount { get; private set; } = 0.0076f;
    [field: SerializeField, Range(1.0f, 3.0f)] public float SprintAmount { get; private set; } = 1.4f;

    [field: SerializeField, Range(5f, 20f)] public float Frequency { get; private set; } = 13f;
    [field: SerializeField, Range(50f, 10f)] public float Smooth { get; private set; } = 15.4f;

    [field: Header("RotationMovement")]
    [field: SerializeField] public bool EnabledRotationMovement { get; private set; } = true;
    [field: SerializeField, Range(0.1f, 10.0f)] public float RotationMultiplier { get; private set; } = 6f;
  }
}