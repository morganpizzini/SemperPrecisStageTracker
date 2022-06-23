using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SemperPrecisStageTracker.Blazor.Helpers;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Blazor.Pages;
using SemperPrecisStageTracker.Blazor.Utils;
using SemperPrecisStageTracker.Shared.Permissions;
using Fluxor;
using SemperPrecisStageTracker.Blazor.Store.AppUseCase;
using SemperPrecisStageTracker.Blazor.Store;
using SemperPrecisStageTracker.Blazor.Components.Utils;

namespace SemperPrecisStageTracker.Blazor.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IHttpService _httpService;
        private readonly NavigationManager _navigationManager;
        private readonly ILocalStorageService _localStorageService;
        //private readonly AuthenticationStateProvider _authenticationStateProvider;
        //private readonly StateService _stateService;
        private readonly IState<UserState> _userState;
        private readonly IDispatcher _dispatcher;
        //private string userKey => nameof(_userState.Value.User);
        //private string permissionKey => nameof(_userState.Value.Permissions);
        //private string authCode => nameof(authCode);


        private CustomAuthStateProvider _customAuthenticationStateProvider; //=> _authenticationStateProvider as CustomAuthStateProvider;

        public AuthenticationService(
            IHttpService httpService,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService,
            AuthenticationStateProvider authenticationStateProvider,
            IState<UserState> UserState,
            IDispatcher Dispatcher
        )
        {
            _dispatcher = Dispatcher;
            _httpService = httpService;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
            _customAuthenticationStateProvider = authenticationStateProvider as CustomAuthStateProvider ?? new CustomAuthStateProvider();
            //_authenticationStateProvider = authenticationStateProvider;
            _userState = UserState;
        }

        public async Task Initialize()
        {
            var loggedUser = _localStorageService.GetItem<ShooterContract>(CommonVariables.UserKey);
            if (loggedUser == null)
            {
                _dispatcher.Dispatch(new UserSetInitializedAction());
                return;
            }
            
            var secret = await _localStorageService.DecodeSecret(loggedUser.Username, CommonVariables.AuthCode);
            if (string.IsNullOrEmpty(secret))
            {
                _dispatcher.Dispatch(new UserSetInitializedAction());
                return;
            }

            var userParams = secret.DecodeBase64().Split(":");

            await Login(userParams[0], userParams[1]);

        }

        public async Task<bool> Login(string username, string password)
        {
            var response = await _httpService.Post<SignInResponse>("/api/Authorization/LogIn", new LogInRequest { Username = username, Password = password });

            if (response is not { WentWell: true })
                return false;

            await SetUser(response.Result,password);
            return true;
            
        }
        public async Task<bool> SignIn(SignInRequest request)
        {
            var response = await _httpService.Post<SignInResponse>("/api/Authorization/SignIn", request);

            if (response is not { WentWell: true })
                return false;

            await SetUser(response.Result, request.Password);
            return true;
        }

        private async Task SetUser(SignInResponse response,string password)
        {
            // set auth data
            response.Shooter.AuthData = $"{response.Shooter.Username}:{password}".EncodeBase64();

            _dispatcher.Dispatch(new SetUserAndPermissionAction(response.Shooter,response.Permissions));
            
            _customAuthenticationStateProvider.LoginNotify(response.Shooter);
        }

        public void UpdateLogin(ShooterContract user)
        {
            // update username
            var userParams = _userState.Value.User.AuthData.DecodeBase64().Split(":");
            userParams[0] = user.Username;
            user.AuthData = string.Join(":", userParams).EncodeBase64();

            //override
            _dispatcher.Dispatch(new SetUserAction(user));
            _customAuthenticationStateProvider.LoginNotify(user);
        }

        public void Logout()
        {
            _dispatcher.Dispatch(new SetUserAction(null));
            _customAuthenticationStateProvider.LogoutNotify();
            _navigationManager.NavigateTo(RouteHelper.GetUrl<Login>());
        }


        public bool CheckPermissions(IPermissionInterface permission, string entityId = "") =>
            CheckPermissions(permission.List, entityId);

        public bool CheckPermissions(Permissions permission, string entityId = "") =>
            CheckPermissions(new List<Permissions> { permission }, entityId);

        public bool CheckPermissions(IList<Permissions> permissions, string entityId = "")
        {
            if (permissions.Count==0)
                return false;

            // AuthenticationServiceLayer.ValidateUserPermissions
            // se ho permessi generici
            return _userState.Value.Permissions.GenericPermissions.Any(permissions.Contains) ||
                   // permessi sull'entità, ma solo nel caso in cui non ho specificato l'id
                   _userState.Value.Permissions.EntityPermissions.Any(x =>
                       (string.IsNullOrEmpty(entityId) || x.EntityId == entityId) &&
                       permissions.Any(p => x.Permissions.Contains(p)));
        }
    }
}