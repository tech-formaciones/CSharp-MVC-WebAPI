using Demos.CSharp.WebApi1.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Demos.CSharp.WebApi1.AttributesFilters
{
    /// <summary>
    ///  Filtro de acción personalizado llamado CustomHeaders que extiende de ActionFilterAttribute. 
    ///  Este filtro agrega encabezados personalizados a la respuesta HTTP después de que la acción del controlador ha sido ejecutada.
    /// </summary>
    public class CustomHeaders : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);

            // Obtiene una instancia del servicio IOperationSingleton del contenedor de dependencias. 
            IOperationSingleton singleton = context.HttpContext.RequestServices.GetService<IOperationSingleton>();

            // Extrae los valores de controller y action de las rutas de la solicitud HTTP.
            context.HttpContext.Request.RouteValues.TryGetValue("controller", out var controllerName);
            context.HttpContext.Request.RouteValues.TryGetValue("action", out var actionName);

            // Agrega varios encabezados personalizados a la respuesta HTTP.
            context.HttpContext.Response.Headers.Append("X-Controller", $"{controllerName}");
            context.HttpContext.Response.Headers.Append("X-Action", $"{actionName}");
            context.HttpContext.Response.Headers.Append("X-Server-Name", Environment.MachineName);
            context.HttpContext.Response.Headers.Append("X-Server-OSVersion", Environment.OSVersion.ToString());
            context.HttpContext.Response.Headers.Append("X-Application-Name", "Demo Curso");
            context.HttpContext.Response.Headers.Append("X-Singleton-Id", singleton.OperationId);
        }
    }
}
