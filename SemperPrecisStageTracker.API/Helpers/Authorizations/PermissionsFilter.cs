using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SemperPrecisStageTracker.API.Helpers
{
    public class PermissionsFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var filterDescriptors = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            var customAuthorizationFilter = (ApiAuthorizationFilter)filterDescriptors
                .Select(filterInfo => filterInfo.Filter)
                .FirstOrDefault(filter => filter is ApiAuthorizationFilter);
            if (customAuthorizationFilter != null)
            {
                operation.Description += $"Permissions: {customAuthorizationFilter.PermissionsString}";
            }
        }
    }
}