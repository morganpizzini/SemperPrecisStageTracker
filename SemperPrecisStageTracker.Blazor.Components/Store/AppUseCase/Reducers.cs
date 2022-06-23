using Fluxor;

namespace SemperPrecisStageTracker.Blazor.Store.AppUseCase;

public static partial class Reducers
{
  [ReducerMethod]
  public static SettingsState ReduceChangeThemeAction(SettingsState state, ChangeThemeAction action) =>
        state with
        {
            Theme = action.Theme
        };
    [ReducerMethod]
  public static SettingsState ReduceIncrementCounterAction(SettingsState state, SetOnlineStatusAction action) =>
        state with
        {
            Online = action.Status
        };
    
  //  [ReducerMethod(typeof(ChangeThemeAction))]
  //public static SettingsState ReduceIncrementCounterAction(SettingsState state) =>
  //  new CounterState(clickCount: state.ClickCount + 1);
}
