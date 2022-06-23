using Fluxor;

namespace SemperPrecisStageTracker.Blazor.Store.AppUseCase;

//[FeatureState]
public record SettingsState
{
  public string Theme { get; init; }
  public bool HasNetwork {get; init;}
  public bool Offline {get; init;}
}

public class SettingsFeature : Feature<SettingsState>
{
    public override string GetName() => "Settings";

    protected override SettingsState GetInitialState() =>
        new SettingsState 
        {
            Theme = string.Empty,
            HasNetwork = true,
            Offline = false
        };
    
}