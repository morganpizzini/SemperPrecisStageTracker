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
            _stateService.User = await _localStorageService.GetItem<ShooterContract>(userKey);
            _stateService.Permissions = await _localStorageService.GetItem<UserPermissionContract>(permissionKey);
            if (_stateService.User != null)
            {
                _customAuthenticationStateProvider.LoginNotify(_stateService.User);
            }
        }

        public async Task<bool> Login(string username, string password)
        {
            var response = await _httpService.Post<SignInResponse>("/api/Authorization/SignIn", new SignInRequest { Username = username, Password = password });

            if (response is not { WentWell: true })
                return false;

            _stateService.User = response.Result.Shooter;
            _stateService.User.AuthData = $"{username}:{password}".EncodeBase64();
            _stateService.Permissions = response.Result.Permissions;

            await _localStorageService.SetItem(userKey, _stateService.User);
            await _localStorageService.SetItem(permissionKey, response.Result.Permissions);
            _customAuthenticationStateProvider.LoginNotify(_stateService.User);

            return true;
        }

        public async Task UpdateLogin(ShooterContract user)
        {
            // update username
            var userParams = _stateService.User.AuthData.DecodeBase64().Split(":");
            userParams[0] = user.Username;
            user.AuthData = string.Join(":", userParams).EncodeBase64();

            //override
            _stateService.User = user;
            await _localStorageService.SetItem(userKey, _stateService.User);
            _customAuthenticationStateProvider.LoginNotify(_stateService.User);
        }

        public async Task Logout()
        {
            _stateService.User = null;
            await _localStorageService.RemoveItem(userKey);
            await _localStorageService.RemoveItem(permissionKey);
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

            if (string.IsNullOrEmpty(entityId) || _stateService.Permissions.EntityPermissions.Count == 0)
                return false;
            
            return _stateService.Permissions.EntityPermissions.Any(x=>x.EntityId == entityId && x.Permissions.Any(permissions.Contains));
        }
    }
}