using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace TelephoneBooth.Core.Services
{
  public interface IAssetService {
    GameObject Instantiate(string path, DiContainer diContainer);
    GameObject Instantiate(string path, DiContainer diContainer, Transform parent);
    T Instantiate<T>(string path, DiContainer diContainer);
    T Instantiate<T>(string path, DiContainer diContainer, Transform parent);
    T Load<T>(string path);
    public UniTaskVoid UnloadUnusedAssets();
  }
}