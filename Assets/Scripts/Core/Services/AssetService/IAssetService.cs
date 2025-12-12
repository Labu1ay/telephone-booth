using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Core.Services
{
  public interface IAssetService {
    GameObject Instantiate(string path, DiContainer diContainer, Transform parent = null);
    GameObject Instantiate(string path, DiContainer diContainer, Vector3 position, Quaternion rotation, Transform parent = null);
    T Instantiate<T>(string path, DiContainer diContainer, Transform parent = null);
    T Instantiate<T>(string path, DiContainer diContainer,  Vector3 position, Quaternion rotation,Transform parent = null);
    T Instantiate<T>(T prefab, DiContainer diContainer, Transform parent = null) where T : Component;
    T Load<T>(string path);
    public UniTaskVoid UnloadUnusedAssets();
  }
}