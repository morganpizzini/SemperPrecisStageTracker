using System.Threading.Tasks;
using SemperPrecisStageTracker.Blazor.Services.Models;

namespace SemperPrecisStageTracker.Blazor.Services
{
    public interface IHttpService
    {
        Task<ApiResponse<T>?> Get<T>(string uri);
        Task<ApiResponse<T>?> Post<T>(string uri);
        Task<ApiResponse<T>?> Post<T>(string uri, object? value);
    }
}