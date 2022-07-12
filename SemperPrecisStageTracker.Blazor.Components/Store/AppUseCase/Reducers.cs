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
    public static SettingsState ReduceChangeIsDeviceAction(SettingsState state, ChangeIsDeviceAction action) =>
          state with
          {
              IsDevice = action.IsDevice
          };

    [ReducerMethod]
    public static SettingsState ReduceSetHasNetworkAction(SettingsState state, SetHasNetworkAction action) =>
        state with
        {
            HasNetwork = action.HasNetwork
        };

    [ReducerMethod]
    public static SettingsState ReduceSetOfflineAction(SettingsState state, SetOfflineAction action) =>
        state with
        {
            Offline = action.Offline
        };

    [ReducerMethod(typeof(SettingsSetInitializedAction))]
    public static SettingsState ReduceUserSetInitializedAction(SettingsState state) =>
        state with
            {
                Initialize= true
            };

    [ReducerMethod(typeof(SettingsSetReadyAction))]
    public static SettingsState ReduceSettingsSetReadyAction(SettingsState state) =>
        state with
            {
                Ready= true
            };
}

