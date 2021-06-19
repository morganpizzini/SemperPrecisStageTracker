using System.Threading.Tasks;

namespace SemperPrecisStageTracker.Blazor.Services
{
    public interface IHttpService
    {
        Task<T> Get<T>(string uri);
        Task<T> Post<T>(string uri);
        Task<T> Post<T>(string uri, object value);
    }
}