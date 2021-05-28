using Microsoft.AspNetCore.Components;
using SemperPrecisStageTracker.Blazor.Helpers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using System.Threading.Tasks;

namespace SemperPrecisStageTracker.Blazor.Services
{
    public interface IAuthenticationService
    {
        ShooterContract User { get; }
        bool IsAuth {get;}
        Task Initialize();
        Task Login(string username, string password);
        Task Logout();
    }

    public class AuthenticationService : IAuthenticationService
    {
        private IHttpService _httpService;
        private NavigationManager _navigationManager;
        private ILocalStorageService _localStorageService;

        public ShooterContract User { get; private set; }

        public bool IsAuth => User!= null;

        public AuthenticationService(
            IHttpService httpService,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService
        ) {
            _httpService = httpService;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
        }

        public async Task Initialize()
        {
            User = await _localStorageService.GetItem<ShooterContract>("user");
        }

        public async Task Login(string username, string password)
        {
            User = await _httpService.Post<ShooterContract>("/api/Authorization/SignIn", new SignInRequest{ Username = username, Password = password });
            User.AuthData = $"{username}:{password}".EncodeBase64();
            await _localStorageService.SetItem("user", User);
        }

        public async Task Logout()
        {
            User = null;
            await _localStorageService.RemoveItem("user");
            _navigationManager.NavigateTo("login");
        }
    }
}