﻿using Microsoft.AspNetCore.Authorization;
using SemperPrecisStageTracker.Contracts.Requests;
using SemperPrecisStageTracker.Blazor.Models;
using Microsoft.AspNetCore.Components;

namespace SemperPrecisStageTracker.Blazor.Pages;


public class SemperPrecisComponent : SemperPrecisBasePresentationalComponent
{
    
    //[Inject]
    //protected IHttpService Service { get; set; } = default!;

    protected async Task<BaseResponse<T>> Call<T>(RequestType requestType, string uri, Dictionary<string, string>? queryParameters = null, object? body = null) where T : new()
    {
        ApiLoading = true;
        var result = await PresentationalService.CallRestfull<T>(requestType, uri, queryParameters, body);
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

    protected override async Task<string> Post1(string uri, object value)
    {
        ApiLoading = true;
        
        (Model,var err) = await PresentationalService.Sample<T>(uri, value);
        
        ApiLoading = false;

        return err;
    }
}