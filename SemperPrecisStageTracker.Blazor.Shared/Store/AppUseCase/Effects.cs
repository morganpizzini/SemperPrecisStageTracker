using Blazorise;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using SemperPrecisStageTracker.Blazor.Components;
using SemperPrecisStageTracker.Blazor.Components.Utils;
using SemperPrecisStageTracker.Blazor.Helpers;
using SemperPrecisStageTracker.Blazor.Models;
using SemperPrecisStageTracker.Blazor.Services;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Contracts.Utilities;

namespace SemperPrecisStageTracker.Blazor.Store.AppUseCase;

public partial class AppUseCaseEffects
{
    private readonly ILocalStorageService _localStorage;
    private readonly IHttpService _httpService;
    private readonly IState<SettingsState> _settingsState;
    private CustomAuthStateProvider _customAuthenticationStateProvider;
    private INotificationService _notificationService;
    private readonly NavigationManager _navigationManager;
    private readonly IJSInProcessRuntime _jsRuntime;

    public AppUseCaseEffects(ILocalStorageService localStorage, IHttpService httpService, IState<SettingsState> settingsState, AuthenticationStateProvider authenticationStateProvider,
        INotificationService notificationService, NavigationManager navigationManager, IJSRuntime jSRuntime)
    {
        _httpService = httpService;
        _localStorage = localStorage;
        _settingsState = settingsState;
        _notificationService = notificationService;
        _navigationManager = navigationManager;
        _jsRuntime = (IJSInProcessRuntime)jSRuntime;
        _customAuthenticationStateProvider = authenticationStateProvider as CustomAuthStateProvider ?? new CustomAuthStateProvider();
    }

    [EffectMethod(typeof(SettingsSetInitializedAction))]
    public async Task HandleSettingsSetInitializedAction(IDispatcher dispatcher)
    {
        // load Theme and general acknoledge
         dispatcher.Dispatch(new ChangeThemeAction(_localStorage.GetItem<string>("theme") ?? "light"));
        
        dispatcher.Dispatch(new ChangeIsDeviceAction(_jsRuntime.Invoke<bool>("customFunctions.isDevice")));

        // load user
        var loggedUser = _localStorage.GetItem<UserContract>(CommonVariables.UserKey);
        
        if (loggedUser == null)
        {
            dispatcher.Dispatch(new SettingsSetReadyAction());
            return;
        }

        var (username, password) = await GetLoggedUserInfo(loggedUser.Username);
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            dispatcher.Dispatch(new SettingsSetReadyAction());
            return;
        }

        if (!_settingsState.Value.Offline)
        {
            // login with API
            await Login(username, password, dispatcher);
        }
        else
        {
            // offline login
            var userPermissions = _localStorage.GetItem<UserPermissionContract>(CommonVariables.PermissionKey);
            SetUser(new SignInResponse
            {
                Permissions = userPermissions,
                User = loggedUser
            }, password, dispatcher);
        }
        
        dispatcher.Dispatch(new SettingsSetReadyAction());
    }
    [EffectMethod]
    public async Task HandleSettingsSetTryLoginAction(TryLoginAction action, IDispatcher dispatcher)
    {
        try
        {
            var result = await Login(action.Username, action.Password, dispatcher);
            if (!result)
            {
                await _notificationService.Error("Error", "LoginFailed");
            }
            else
            {
                _jsRuntime.Invoke<NotificationSubscriptionCreateRequest>("BlazorPWA.requestSubscription");
                _navigationManager.NavigateTo(action.ReturnUrl);
            }
        }
        catch (Exception ex)
        {
            await _notificationService.Error("Error", ex.Message);
        }
    }

    [EffectMethod]
    public Task HandleSetUserWithoutLoginAction(SetUserWithoutLoginAction action, IDispatcher dispatcher)
    {
        SetUser(action.UserData,action.Password,dispatcher);
        return Task.CompletedTask;
    }

    private async Task<(string username, string password)> GetLoggedUserInfo(string secretKey)
    {
        var secret = await _localStorage.DecodeSecret(secretKey, CommonVariables.AuthCode);
        if (string.IsNullOrEmpty(secret))
        {
            return default;
        }
        var userParams = secret.DecodeBase64().Split(":");
        return (userParams[0], userParams[1]);
    }
    private async Task<bool> Login(string username, string password, IDispatcher dispatcher)
    {
        var response = await _httpService.Post<SignInResponse>("/api/Authorization/LogIn", new LogInRequest { Username = username, Password = password });

        if (response is not { WentWell: true })
            return false;

        SetUser(response.Result, password, dispatcher);
        return true;
    }

    private void SetUser(SignInResponse response, string password, IDispatcher dispatcher)
    {
        // set auth data
        response.User.AuthData = $"{response.User.Username}:{password}".EncodeBase64();

        dispatcher.Dispatch(new SetUserAndPermissionAction(response.User, response.Permissions));

        _customAuthenticationStateProvider.LoginNotify(response.User);
    }

    [EffectMethod]
    public Task HandleChangeThemeAction(ChangeThemeAction action, IDispatcher _)
    {
        _localStorage.SetItem("theme", action.Theme);
        return Task.CompletedTask;
    }
    [EffectMethod]
    public Task HandleSetOfflineAction(SetOfflineAction action, IDispatcher dispatcher)
    {
        var config = _localStorage.GetItem<ClientSetting>(CommonVariables.ClientSettingsKey) ?? new ClientSetting();
        config.OfflineMode = action.Offline;
        config.MatchId = action.MatchId;

        _localStorage.SetItem(CommonVariables.ClientSettingsKey, config);

        dispatcher.Dispatch(new SettingsSetInitializedAction());

        return Task.CompletedTask;
    }
    //  [EffectMethod]
    //public async Task HandleFetchDataAction(ChangeThemeAction action, IDispatcher dispatcher)
    //{
    //      LocalStorage.SetItem("theme",action.Theme);
    //  var forecasts = await WeatherForecastService.GetForecastAsync(DateTime.Now);
    //  dispatcher.Dispatch(new FetchDataResultAction(forecasts));
    //}
}
