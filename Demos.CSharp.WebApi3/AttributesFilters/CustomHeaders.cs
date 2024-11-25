using Microsoft.AspNetCore.Mvc.Filters;

namespace Demos.CSharp.WebApi3.AttributesFilters
{
    public class CustomHeaders : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            // Antes 

            var result = await next(context);

            // Después
            context.HttpContext.Response.Headers.Append("X-Server-Name", Environment.MachineName);
            context.HttpContext.Response.Headers.Append("X-Server-OSVersion", Environment.OSVersion.ToString());
            context.HttpContext.Response.Headers.Append("X-Application-Name", "Demo Curso");

            return result;
        }
    }
}
