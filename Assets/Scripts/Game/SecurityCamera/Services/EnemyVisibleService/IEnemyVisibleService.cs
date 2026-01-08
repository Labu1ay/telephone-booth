using System;
using UnityEngine;

namespace TelephoneBooth.Game.SecurityCamera.Services
{
  public interface IEnemyVisibleService
  {
    event Action DangerousTimeOvered;
    void InitCamera(Camera camera);
    void DisposeCamera();
  }
}