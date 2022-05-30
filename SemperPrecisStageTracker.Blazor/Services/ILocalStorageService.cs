using System.Threading.Tasks;

namespace SemperPrecisStageTracker.Blazor.Services
{
    public interface ILocalStorageService
    {
        T GetItem<T>(string key);
        void SetItem<T>(string key, T value);
        void RemoveItem(string key);
        ValueTask EncodeSecret(string key,string name,string value);
        ValueTask<string> DecodeSecret(string key,string name);
    }
}