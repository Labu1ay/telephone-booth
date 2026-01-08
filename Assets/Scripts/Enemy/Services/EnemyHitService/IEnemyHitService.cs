using Cysharp.Threading.Tasks;

namespace TelephoneBooth.Enemy.Services
{
  public interface IEnemyHitService
  {
    UniTask Hit();
  }
}