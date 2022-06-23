using Fluxor;

namespace SemperPrecisStageTracker.Blazor.Store.AppUseCase;

public static partial class Reducers
{
    [ReducerMethod]
    public static UserState ReduceSetUserAction(UserState state, SetUserAction action) {
        if(action.User == null){
            return state with
            {
                Initialize = true,
                User = action.User,
                Info = new Contracts.Requests.ShooterInformationResponse(),
                Permissions = new Contracts.UserPermissionContract()
            };
        }
        return state with
            {
                Initialize = true,
                User = action.User
            };
    }

    [ReducerMethod]
    public static UserState ReduceSetUserAndPermissionAction(UserState state, SetUserAndPermissionAction action) {
        return state with
        {
            Initialize = true,
            User = action.User,
            Permissions = action.Permissions
        };
    }

    [ReducerMethod]
    public static UserState ReduceSetUserInformationAction(UserState state, SetUserInformationAction action) =>
        state with
            {
                Info = action.Info
            };

    [ReducerMethod]
    public static UserState ReduceSetUserPermissionsAction(UserState state, SetUserPermissionAction action) =>
        state with
            {
                Permissions= action.Permissions
            };

    [ReducerMethod(typeof(UserSetInitializedAction))]
    public static UserState ReduceSetUserPermissionsAction(UserState state) =>
        state with
            {
                Initialize= true
            };

}
