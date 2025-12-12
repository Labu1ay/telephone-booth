using System.Collections.Generic;
using UnityEngine;

namespace TelephoneBooth.Player.Hands
{
  [RequireComponent(typeof(Animator))]
  public class HandAnimator : MonoBehaviour
  {
    [SerializeField] private Animator _animator;

    private Dictionary<HandAnimationType, int> _animations = new()
    {
      { HandAnimationType.Hide , Animator.StringToHash("Hide")}
    };

    private void OnValidate()
    {
      _animator ??= GetComponent<Animator>();
    }

    public void SetAnimation(HandAnimationType animation, bool value) => 
      _animator.SetBool(_animations[animation], value);
  }
}