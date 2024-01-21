using Fluxor;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;

namespace SemperPrecisStageTracker.Blazor.Store.AppUseCase;

//[FeatureState]
public record UserState
{
    //public bool Initialize { get; init; }// = new ShooterContract();
    public UserContract? User { get; init; }// = new ShooterContract();
    public ShooterInformationResponse Info { get; init; } = new ShooterInformationResponse();
    public UserPermissionContract Permissions { get; init; } = new UserPermissionContract();

    //private UserState() { } // Required for creating initial state

    //public UserState(ShooterContract user, ShooterInformationResponse info, UserPermissionContract permissions)
    //{
    //    User = user;
    //    Info = info;
    //    Permissions = permissions;
    //}
}

public class UserFeature : Feature<UserState>
{
    public override string GetName() => "User";

    protected override UserState GetInitialState()
    {
        return new UserState 
        {
            //Initialize = false,
            User = null,
            Info = new ShooterInformationResponse(),
            Permissions = new UserPermissionContract(),
        };
    }
}
