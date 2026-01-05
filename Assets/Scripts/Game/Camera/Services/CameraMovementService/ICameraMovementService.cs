using System;
using UnityEngine;

namespace TelephoneBooth.Game
{
  public interface ICameraMovementService
  {
    void SetCameraPoint(Transform target, float duration = 0.5f, Action callback = null);
    void SetCameraPointWithCurve(Transform target, float duration = 0.5f, float centerOffset = -0.5f, Action callback = null);
    void RollbackCamera(float duration = 0.5f, Action callback = null);
    void RollbackCameraWithCurve(float duration = 0.5f, float centerOffset = -0.5f, Action callback = null);
  }
}