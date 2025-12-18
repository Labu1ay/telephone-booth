using UnityEngine;

namespace TelephoneBooth.Game.SecurityCamera
{
  public class SecurityCameraMonitor : MonoBehaviour
  {
    public Camera securityCamera;

    public MeshRenderer monitorScreenRenderer;

    public int textureWidth = 512;
    public int textureHeight = 288;
    public int depth = 16;

    private RenderTexture _renderTexture;

    void Start()
    {
      _renderTexture = new RenderTexture(textureWidth, textureHeight, depth);
      _renderTexture.name = "SecurityCameraRT_" + securityCamera.name;
      _renderTexture.Create();

      securityCamera.targetTexture = _renderTexture;
      monitorScreenRenderer.material.mainTexture = _renderTexture;

      securityCamera.aspect = (float)textureWidth / textureHeight;
    }

    void OnDestroy()
    {
      if (securityCamera != null) securityCamera.targetTexture = null;

      if (_renderTexture != null)
      {
        _renderTexture.Release();
        Destroy(_renderTexture);
      }
    }
  }
}