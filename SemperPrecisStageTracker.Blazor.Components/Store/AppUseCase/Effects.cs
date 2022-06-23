using Fluxor;
using SemperPrecisStageTracker.Blazor.Services;

namespace SemperPrecisStageTracker.Blazor.Store.AppUseCase;

public partial class Effects
{
    
    [EffectMethod]
    public Task HandleChangeThemeAction(ChangeThemeAction action, IDispatcher _)
    {
        _localStorage.SetItem("theme", action.Theme);
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
