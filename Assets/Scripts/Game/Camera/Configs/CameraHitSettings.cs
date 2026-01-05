using System;

namespace TelephoneBooth.Game.Configs
{
  [Serializable]
  public struct CameraHitSettings
  {
    public float HorizontalPush; 
    public float VerticalPush;   
    public float TiltAngle;       
    public float ShakeStrength;

    public float DelayToPush;
    public float PushDuration;
    public float FallDuration;
  }
}