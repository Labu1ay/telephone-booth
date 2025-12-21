namespace TelephoneBooth.Core.Services
{
  public interface ISavingService
  {
    SaveContainer<T> GetPackage<T>(string key, T defaultValue = default);
    bool RemovePackage(string key);
    void Save();
    void SaveKey(string key);
  }
}