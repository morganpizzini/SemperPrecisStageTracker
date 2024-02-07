﻿using System.Threading.Tasks;
using Blazorise;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using SemperPrecisStageTracker.Blazor.Services;

namespace SemperPrecisStageTracker.Blazor.Pages;

public class SemperPrecisBasePresentationalValidationComponent<T> : SemperPrecisBasePresentationalComponent where T : new()
{
    protected Validations validations = default!;

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

public class SemperPrecisBasePresentationalComponent : FluxorComponent
{
    [Inject]
    protected IAuthenticationService AuthService { get; set; } = default!;
        
    [Inject]
    protected PresentationalServiceLayer PresentationalService{ get; set; } = default!;

    [Parameter]
    public virtual bool ApiLoading { get; set; }

    protected override void Dispose(bool disposed)
    {
      base.Dispose(disposed);
    }

    protected Task ShowNotificationSuccess(string message, string title = "", NotificationType notificationType = NotificationType.Info) =>
        ShowNotification(message,title,NotificationType.Success);

    protected Task ShowNotificationError(string message, string title = "", NotificationType notificationType = NotificationType.Info) =>
        ShowNotification(message,title,NotificationType.Error);

    protected Task ShowNotification(string message, string title = "", NotificationType notificationType = NotificationType.Info) =>
        PresentationalService.ShowNotification(message,title,notificationType);
}