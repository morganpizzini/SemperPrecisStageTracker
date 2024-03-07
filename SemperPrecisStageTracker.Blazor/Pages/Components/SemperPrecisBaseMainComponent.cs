using Microsoft.AspNetCore.Components;
using SemperPrecisStageTracker.Blazor.Services;
using Microsoft.AspNetCore.Authorization;

namespace SemperPrecisStageTracker.Blazor.Pages;

[Authorize]
public class SemperPrecisBaseMainComponent : SemperPrecisBaseComponent
{
    [Inject]
    protected MainServiceLayer MainServiceLayer { get; set; } = default!;
}