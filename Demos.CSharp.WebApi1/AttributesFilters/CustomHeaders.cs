using Demos.CSharp.WebApi1.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Demos.CSharp.WebApi1.AttributesFilters
{
    public class CustomHeaders : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);

            IOperationSingleton singleton = context.HttpContext.RequestServices.GetService<IOperationSingleton>();

            context.HttpContext.Request.RouteValues.TryGetValue("controller", out var controllerName);
            context.HttpContext.Request.RouteValues.TryGetValue("action", out var actionName);

            context.HttpContext.Response.Headers.Add("X-Controller", $"{controllerName}");
            context.HttpContext.Response.Headers.Add("X-Action", $"{actionName}");

            context.HttpContext.Response.Headers.Add("X-Server-Name", Environment.MachineName);
            context.HttpContext.Response.Headers.Add("X-Server-OSVersion", Environment.OSVersion.ToString());
            context.HttpContext.Response.Headers.Add("X-Application-Name", "Demo Curso");
            context.HttpContext.Response.Headers.Add("X-Singleton-Id", singleton.OperationId);
        }
    }
}
