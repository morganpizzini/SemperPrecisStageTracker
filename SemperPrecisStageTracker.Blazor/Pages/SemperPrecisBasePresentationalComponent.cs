using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using SemperPrecisStageTracker.Blazor.Services;

namespace SemperPrecisStageTracker.Blazor.Pages;

public class SemperPrecisBasePresentationalValidationComponent<T> : SemperPrecisBasePresentationalComponent where T : new()
{
    protected Validations validations;

    [Parameter,EditorRequired]
    public virtual T Model { get; set; } = new();

    [Parameter,EditorRequired]
    public EventCallback SubmitCallback { get; set; }

    protected async Task Submit()
    {
        if (!(await validations.ValidateAll()))
            return;

        await validations.ClearAll();

        await SubmitCallback.InvokeAsync();
    }
}

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

    protected Task ShowNotificationSuccess(string message, string title = "", NotificationType notificationType = NotificationType.Info) => ShowNotification(message,title,NotificationType.Success);
    protected Task ShowNotificationError(string message, string title = "", NotificationType notificationType = NotificationType.Info) => ShowNotification(message,title,NotificationType.Error);
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