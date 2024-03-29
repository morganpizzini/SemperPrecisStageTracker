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
using SemperPrecisStageTracker.Blazor.Components;

namespace SemperPrecisStageTracker.Blazor.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IHttpService _httpService;
        private readonly NavigationManager _navigationManager;
        private readonly IState<UserState> _userState;
        private readonly IDispatcher _dispatcher;

        private CustomAuthStateProvider _customAuthenticationStateProvider;

        public AuthenticationService(
            IHttpService httpService,
            NavigationManager navigationManager,
            AuthenticationStateProvider authenticationStateProvider,
            IState<UserState> UserState,
            IDispatcher Dispatcher
        )
        {
            _dispatcher = Dispatcher;
            _httpService = httpService;
            _navigationManager = navigationManager;
            _customAuthenticationStateProvider = authenticationStateProvider as CustomAuthStateProvider ?? new CustomAuthStateProvider();
            _userState = UserState;
        }

        public void Login(string username, string password)
        {
            _dispatcher.Dispatch(new TryLoginAction
            {
                Username = username,
                Password = password,
                ReturnUrl = _navigationManager.QueryString("returnUrl") ?? RouteHelper.GetUrl<Home>()
            });
        }

        public async Task<bool> SignIn(SignInRequest request)
        {
            var response = await _httpService.Post<SignInResponse>("/api/Authorization/SignIn", request);

            if (response is not { WentWell: true })
                return false;

            _dispatcher.Dispatch(new SetUserWithoutLoginAction()
            {
                Password = request.Password,
                UserData = response.Result
            });

            return true;
        }

        public void UpdateLogin(ShooterContract user)
        {
            // update username
            var userParams = _userState.Value.User?.AuthData.DecodeBase64().Split(":");
            if(userParams == null)
                return;

            userParams[0] = user.Username;
            user.AuthData = string.Join(":", userParams).EncodeBase64();

            //override
            _dispatcher.Dispatch(new SetUserAction(user));
            _customAuthenticationStateProvider.LoginNotify(user);
        }

        public void Logout()
        {
            _dispatcher.Dispatch(new SetUserAction(null));
            _dispatcher.Dispatch(new SetOfflineAction(false,string.Empty));
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
                   // permessi sull'entit�, ma solo nel caso in cui non ho specificato l'id
                   _userState.Value.Permissions.EntityPermissions.Any(x =>
                       (string.IsNullOrEmpty(entityId) || x.EntityId == entityId) &&
                       permissions.Any(p => x.Permissions.Contains(p)));
        }
    }
}