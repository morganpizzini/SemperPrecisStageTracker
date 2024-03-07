using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authorization;
using SemperPrecisStageTracker.Blazor.BackOffice.Services;

namespace SemperPrecisStageTracker.Blazor.Pages;

[Authorize]
public class SemperPrecisBaseMainComponent : SemperPrecisBaseComponent
{
    [Inject]
    protected MainServiceLayer MainServiceLayer { get; set; } = default!;
}