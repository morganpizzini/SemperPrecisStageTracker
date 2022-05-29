using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SemperPrecisStageTracker.Blazor.Helpers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SemperPrecisStageTracker.Blazor.Pages.App;
using SemperPrecisStageTracker.Blazor.Utils;
using SemperPrecisStageTracker.Shared.Permissions;

namespace SemperPrecisStageTracker.Blazor.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IHttpService _httpService;
        private readonly NavigationManager _navigationManager;
        private readonly ILocalStorageService _localStorageService;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        //public ShooterContract User { get; private set; }
        //public PermissionsResponse Permissions { get; private set; }
        private readonly StateService _stateService;

        private string userKey => nameof(_stateService.User);
        private string permissionKey => nameof(_stateService.Permissions);
        private string authCode => nameof(authCode);


        private CustomAuthStateProvider _customAuthenticationStateProvider => _authenticationStateProvider as CustomAuthStateProvider;

        public AuthenticationService(
            IHttpService httpService,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService,
            AuthenticationStateProvider authenticationStateProvider,
            StateService stateService
        )
        {
            _httpService = httpService;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
            _authenticationStateProvider = authenticationStateProvider;
            _stateService = stateService;
        }

        public async Task Initialize()
        {
            var loggedUser = _localStorageService.GetItem<ShooterContract>(userKey);
            if (loggedUser == null)
                return;
            var secret = await _localStorageService.DecodeSecret(loggedUser.Username, authCode);
            if (string.IsNullOrEmpty(secret))
                return;
            var userParams = secret.DecodeBase64().Split(":");

            await Login(userParams[0], userParams[1]);
        }

        public async Task<bool> Login(string username, string password)
        {
            var response = await _httpService.Post<SignInResponse>("/api/Authorization/SignIn", new SignInRequest { Username = username, Password = password });

            if (response is not { WentWell: true })
                return false;

            _stateService.User = response.Result.Shooter;
            // save user
            _localStorageService.SetItem(userKey, _stateService.User);
            // update only on runtime the auth data
            _stateService.User.AuthData = $"{username}:{password}".EncodeBase64();

            // store authData in secret for silent login
            await _localStorageService.EncodeSecret(_stateService.User.Username, authCode, _stateService.User.AuthData);

            _stateService.Permissions = response.Result.Permissions;
            _localStorageService.SetItem(permissionKey, response.Result.Permissions);

            _customAuthenticationStateProvider.LoginNotify(_stateService.User);

            return true;
        }

        public void UpdateLogin(ShooterContract user)
        {
            // update username
            var userParams = _stateService.User.AuthData.DecodeBase64().Split(":");
            userParams[0] = user.Username;
            user.AuthData = string.Join(":", userParams).EncodeBase64();

            //override
            _stateService.User = user;
            _localStorageService.SetItem(userKey, _stateService.User);
            _customAuthenticationStateProvider.LoginNotify(_stateService.User);
        }

        public void Logout()
        {
            _stateService.User = null;
            _localStorageService.RemoveItem(userKey);
            _localStorageService.RemoveItem(permissionKey);
            _localStorageService.RemoveItem(authCode);
            _customAuthenticationStateProvider.LogoutNotify();
            _navigationManager.NavigateTo(RouteHelper.GetUrl<Login>());
        }
        

        public bool CheckPermissions(Permissions permission, string entityId = "") =>
            CheckPermissions(new List<Permissions> { permission }, entityId);

        public bool CheckPermissions(IList<Permissions> permissions, string entityId = "")
        {
            if (permissions == null || permissions.Count==0)
                return false;

            if (_stateService.Permissions.GenericPermissions.Any(permissions.Contains))
            {
                return true;
            }
            return !string.IsNullOrEmpty(entityId) && _stateService.Permissions.EntityPermissions.Count > 0
                        && _stateService.Permissions.EntityPermissions.Any(x=>x.EntityId == entityId && x.Permissions.Any(permissions.Contains));
        }
    }
}