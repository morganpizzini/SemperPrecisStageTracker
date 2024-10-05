using Microsoft.AspNetCore.Authorization;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Blazor.Models;
using Microsoft.AspNetCore.Components;
using SemperPrecisStageTracker.Blazor.Components.Utils;

namespace SemperPrecisStageTracker.Blazor.Pages;


public class SemperPrecisComponent : SemperPrecisBasePresentationalComponent
{

    //[Inject]
    //protected IHttpService Service { get; set; } = default!;
    protected Task Call(RequestType requestType, string uri, string positiveMessage = "", Dictionary<string, string>? queryParameters = null)
        => Call<object>(requestType, uri, queryParameters, null, positiveMessage);

    protected Task Call(RequestType requestType, string uri, Dictionary<string, string>? queryParameters = null, object? body = null, string positiveMessage = "")
        => Call<object>(requestType, uri, queryParameters, body, positiveMessage); 
    

    protected async Task<BaseResponse<T>> Call<T>(RequestType requestType, string uri, Dictionary<string, string>? queryParameters = null, object? body = null,string positiveMessage = "") where T : new()
    {
        ApiLoading = true;
        var result = await PresentationalService.CallRestfull<T>(requestType, uri, queryParameters, body, positiveMessage);
        ApiLoading = false;
        return result;
    }

    public Task<T> Post<T>(string uri) where T : new() => Post<T>(uri, new { });

    protected virtual Task<string> Post1(string uri, object value)
    {
        return Task.FromResult(string.Empty);
    }

    protected virtual async Task<T> Post<T>(string uri, object? value= null, bool pageOperation = true) where T : new()
    {
        if (pageOperation)
            ApiLoading = true;
        
        var (result,_) = await PresentationalService.Sample<T>(uri,value);

        if (pageOperation)
            ApiLoading = false;
        return result;
    }

    protected async Task<T> Post<T>(Func<Task<T>> method)
    {
        ApiLoading = true;
        var result = await method();
        ApiLoading = false;
        return result;
    }
}

[Authorize]
public class SemperPrecisBaseComponent : SemperPrecisComponent
{
}

public class SemperPrecisBaseComponent<T> : SemperPrecisBaseComponent where T : new()
{
    protected T Model = new();
    protected T CloneModel = new();
    protected bool _disableEdit = true;
    protected bool IsNewElement => Id == CommonVariables.NewUrlEndpoint;

    [Parameter]
    public string Id { get; set; } = string.Empty;

    override protected async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (string.IsNullOrEmpty(Id))
        {
            return;
        }
        await LoadModel();
    }

    protected virtual Task LoadModel()
    {
        return Task.CompletedTask;
    }

    protected override async Task<string> Post1(string uri, object value)
    {
        ApiLoading = true;
        
        (Model,var err) = await PresentationalService.Sample<T>(uri, value);
        
        ApiLoading = false;

        return err;
    }
}
