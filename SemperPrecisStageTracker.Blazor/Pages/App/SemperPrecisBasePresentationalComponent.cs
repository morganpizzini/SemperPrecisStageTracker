using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using SemperPrecisStageTracker.Blazor.Services;

namespace SemperPrecisStageTracker.Blazor.Pages;

[Authorize]
public class SemperPrecisBasePresentationalComponent : ComponentBase
{
    [Inject]
    protected IAuthenticationService AuthService { get; set; }
        
    [Inject]
    private INotificationService NotificationService { get; set; }

    //public bool PageLoading { get; set; } = true;
    [Parameter]
    public virtual bool ApiLoading { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        //PageLoading = false;
    }

    protected Task ShowNotification(string message, string title = "", NotificationType notificationType = NotificationType.Info)
    {
        if (string.IsNullOrEmpty(message))
        {
            return Task.CompletedTask;
        }
        switch (notificationType)
        {
            case NotificationType.Warning:
                return NotificationService.Warning(message, title);
            case NotificationType.Error:
                return NotificationService.Error(message, title);
            case NotificationType.Success:
                return NotificationService.Success(message, title);
            case NotificationType.Info:
            default:
                return NotificationService.Warning(message, title);
        }
    }
}