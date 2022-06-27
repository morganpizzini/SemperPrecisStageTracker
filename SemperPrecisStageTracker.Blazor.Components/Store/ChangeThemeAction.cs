using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;

namespace SemperPrecisStageTracker.Blazor.Store;

public class ChangeThemeAction
{
    public string Theme { get; set; }
    public ChangeThemeAction(string theme)
    {
        Theme = theme;
    }
}

public class SetHasNetworkAction
{
    public bool HasNetwork { get; set; }
    public SetHasNetworkAction(bool status)
    {
        HasNetwork = status;
    }
}

public class SetOfflineAction
{
    public bool Offline { get; set; }
    public string MatchId { get; set; }
    public SetOfflineAction(bool offline,string matchId)
    {
        Offline = offline;
        MatchId = matchId;
    }
}

public class SettingsSetInitializedAction {}
public class SettingsSetReadyAction {}

public record TryLoginAction
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string ReturnUrl {get; init; } = string.Empty;
}

public record SetUserWithoutLoginAction{    
    public SignInResponse UserData { get; init; } = new SignInResponse();
    public string Password { get; init; } = string.Empty;
}

public class SetUserInformationAction
{
    public ShooterInformationResponse Info { get; set; }
    public SetUserInformationAction(ShooterInformationResponse info)
    {
        Info = info;
    }
}

//public class UserSetInitializedAction { }


public class SetUserAction
{
    public ShooterContract User { get; set; }
    public SetUserAction(ShooterContract user)
    {
        User = user;
    }
}

public class SetUserAndPermissionAction
{
    public ShooterContract User { get; set; }
    public UserPermissionContract Permissions { get; set; }
    public SetUserAndPermissionAction(ShooterContract user,UserPermissionContract permissions)
    {
        User = user;
        Permissions = permissions;
    }
}

public class SetUserPermissionAction
{
    public UserPermissionContract Permissions { get; set; }
    public SetUserPermissionAction(UserPermissionContract permissions)
    {
        Permissions = permissions;
    }
}
