using System;
using Cysharp.Threading.Tasks;

namespace TelephoneBooth.Core.Services
{
  public interface ISceneLoaderService {
    string ActiveSceneName { get; }
    UniTaskVoid Load(string name, Action callback = null);
  }
}