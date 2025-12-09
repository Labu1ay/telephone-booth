namespace TelephoneBooth.Core.Services
{
  public interface ISaveLoadService {
    void Save(string key, string value);
    void Save<T>(string key, T obj);
    string Load(string key);
    T Load<T>(string key);
    bool HasKey(string key);
  }
}