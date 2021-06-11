using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SemperPrecisStageTracker.Blazor.Helpers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using System.Threading.Tasks;
using SemperPrecisStageTracker.Blazor.Pages;
using SemperPrecisStageTracker.Blazor.Utils;

namespace SemperPrecisStageTracker.Blazor.Services
{
    public interface IAuthenticationService
    {
        ShooterContract User { get; }
        bool IsAuth { get; }
        Task Initialize();
        Task<bool> Login(string username, string password);
        Task UpdateLogin(ShooterContract user);
        Task Logout();
    }

    public class AuthenticationService : IAuthenticationService
    {
        private IHttpService _httpService;
        private NavigationManager _navigationManager;
        private ILocalStorageService _localStorageService;
        private AuthenticationStateProvider _authenticationStateProvider;
        public ShooterContract User { get; private set; }

        public bool IsAuth => User != null;

        public CustomAuthStateProvider _customAuthenticationStateProvider => _authenticationStateProvider as CustomAuthStateProvider;

        public AuthenticationService(
            IHttpService httpService,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService,
            AuthenticationStateProvider authenticationStateProvider
        )
        {
            _httpService = httpService;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task Initialize()
        {
            User = await _localStorageService.GetItem<ShooterContract>("user");
            if (User != null)
            {
                _customAuthenticationStateProvider.LoginNotify(User);
            }
        }

        public async Task<bool> Login(string username, string password)
        {
            User = await _httpService.Post<ShooterContract>("/api/Authorization/SignIn", new SignInRequest { Username = username, Password = password });
            User.AuthData = $"{username}:{password}".EncodeBase64();
            await _localStorageService.SetItem("user", User);
            _customAuthenticationStateProvider.LoginNotify(User);

            return true;
        }

        public async Task UpdateLogin(ShooterContract user)
        {
            // update username
            var userParams = User.AuthData.DecodeBase64().Split(":");
            userParams[0] = user.Username;
            user.AuthData = string.Join(":", userParams).EncodeBase64();

            //override
            User = user;
            await _localStorageService.SetItem("user", User);
            _customAuthenticationStateProvider.LoginNotify(User);
        }

        public async Task Logout()
        {
            User = null;
            await _localStorageService.RemoveItem("user");
            _customAuthenticationStateProvider.LogoutNotify();
            _navigationManager.NavigateTo(RouteHelper.GetUrl<Login>());
        }

    }
}