using System.Threading.Tasks;
using SemperPrecisStageTracker.Blazor.Services.Models;

namespace SemperPrecisStageTracker.Blazor.Services
{
    public interface IHttpService
    {
        Task<ApiResponse<T>> Get<T>(string uri, Dictionary<string,string>? queryParameters = null);
        Task<ApiResponse<T>> Post<T>(string uri, object? value = null);
    }
}