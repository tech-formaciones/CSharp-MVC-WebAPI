using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace Demos.CSharp.WebApi1.AttributesFilters
{
    /// <summary>
    ///  Implementa un filtro de acción personalizado que hereda de ActionFilterAttribute. 
    ///  Se utiliza para realizar validaciones de autorización antes de que se ejecute una acción en un controlador.
    /// </summary>
    public class Autorizacion : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Obtiene la instancia de configuración de la aplicación usando IConfiguration,
            // que permite acceder a los valores configurados de  appsettings.json.
            IConfiguration configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();

            // Obtiene una clave secreta Clave de la configuración y la compara con un valor
            // de encabezado de la solicitud llamado APIKey.
            // Si la clave no coincide con el valor del encabezado APIKey, se establece una
            // respuesta UnauthorizedObjectResult .
            try
            {
                string clave = configuration.GetValue<string>("Clave");
                context.HttpContext.Request.Headers.TryGetValue("APIKey", out var apikey);

                if (clave != apikey)
                {
                    context.Result = new UnauthorizedObjectResult(JsonConvert.SerializeObject(
                        new
                        {
                            Error = HttpStatusCode.Unauthorized.ToString(),
                            Message = "El APIKey no es valida."
                        }
                    ));
                }
                else 
                {
                    await next();
                    // Código que se ejecuta después de la acción
                }
            }
            catch (Exception e)
            {
                // Opcion A

                //context.Result = new BadRequestObjectResult(JsonConvert.SerializeObject(
                //    new
                //    {
                //        Error = HttpStatusCode.BadRequest.ToString(),
                //        Message = e.Message
                //    }
                //));


                // Opcion B

                context.HttpContext.Response.Headers.Clear();
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    Error = HttpStatusCode.InternalServerError.ToString(),
                    Message = e.Message
                });
            }
        }
    }

    /// <summary>
    ///  Implementa un filtro de acción personalizado que hereda de Attribute e implementa IAsyncActionFilter. 
    ///  Se utiliza para realizar validaciones de autorización antes de que se ejecute una acción en un controlador.
    ///  Attribute, permite su uso con controladores y métodos de los controladores
    /// </summary>
    public class Autorizacion2 : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            IConfiguration configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();

            try
            {
                string clave = configuration.GetValue<string>("Clave");
                context.HttpContext.Request.Headers.TryGetValue("APIKey", out var apikey);

                if (clave != apikey)
                {
                    context.Result = new UnauthorizedObjectResult(JsonConvert.SerializeObject(
                        new
                        {
                            Error = HttpStatusCode.Unauthorized.ToString(),
                            Message = "El APIKey no es valida."
                        }
                    ));
                }
                else
                {
                    await next();
                    // Código que se ejecuta después de la acción
                }
            }
            catch (Exception e)
            {
                context.HttpContext.Response.Headers.Clear();
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    Error = HttpStatusCode.InternalServerError.ToString(),
                    Message = e.Message
                });
            }
        }
    }

    /// <summary>
    ///  Implementa un filtro de acción personalizado que hereda de Attribute e implementa IActionFilter. 
    ///  Se utiliza para realizar validaciones de autorización antes de que se ejecute una acción en un controlador.
    ///  Attribute, permite su uso con controladores y métodos de los controladores
    /// </summary>
    public class Autorizacion3 : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            IConfiguration configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();

            try
            {
                string clave = configuration.GetValue<string>("Clave");
                context.HttpContext.Request.Headers.TryGetValue("APIKey", out var apikey);

                if (clave != apikey)
                {
                    context.Result = new UnauthorizedObjectResult(JsonConvert.SerializeObject(
                        new
                        {
                            Error = HttpStatusCode.Unauthorized.ToString(),
                            Message = "El APIKey no es valida."
                        }
                    ));
                }
            }
            catch (Exception e)
            {
                context.HttpContext.Response.Headers.Clear();
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    Error = HttpStatusCode.InternalServerError.ToString(),
                    Message = e.Message
                });
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Código que se ejecuta después de la acción
        }
    }
}
