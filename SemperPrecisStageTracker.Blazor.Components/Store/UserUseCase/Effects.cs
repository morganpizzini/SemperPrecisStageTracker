using Fluxor;
using SemperPrecisStageTracker.Blazor.Components.Utils;
using SemperPrecisStageTracker.Blazor.Services;
using SemperPrecisStageTracker.Contracts.Requests;

namespace SemperPrecisStageTracker.Blazor.Store.AppUseCase;

public partial class Effects
{
    private readonly ILocalStorageService _localStorage;
    private readonly IHttpService _httpService;
    public Effects(ILocalStorageService localStorage, IHttpService httpService)
    {
        _httpService = httpService;
        _localStorage = localStorage;
    }

    [EffectMethod]
    public Task HandleSetUserAction(SetUserAction action, IDispatcher dispatcher)
    {
        return DoWork(action.User,dispatcher);
    }
    
    [EffectMethod]
    public Task HandleSetUserAndPermissionAction(SetUserAndPermissionAction action, IDispatcher dispatcher)
    {
        _localStorage.SetItem(CommonVariables.PermissionKey, action.Permissions);
        return DoWork(action.User,dispatcher);
    }

    private async Task DoWork(Contracts.ShooterContract user, IDispatcher dispatcher)
    {
         if (user == null)
        { 
            _localStorage.RemoveItem(CommonVariables.UserKey);
            _localStorage.RemoveItem(CommonVariables.PermissionKey);
            _localStorage.RemoveItem(CommonVariables.AuthCode);
            return;
        }

        _localStorage.SetItem(CommonVariables.UserKey, user );
        await _localStorage.EncodeSecret(user.Username, CommonVariables.AuthCode, user.AuthData);
        var shooterInfo = await _httpService.Post<ShooterInformationResponse>(
                "api/Aggregation/FetchShooterInformation", new ShooterRequest
                {
                    ShooterId = user.ShooterId
                });
        if (shooterInfo is { WentWell: true })
        {
            dispatcher.Dispatch(new SetUserInformationAction(shooterInfo.Result));
        }
    }
}
