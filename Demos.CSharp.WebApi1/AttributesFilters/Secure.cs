using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace Demos.CSharp.WebApi1.AttributesFilters
{

    public class Autorizacion : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            IConfiguration configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();

            try
            {
                var a = 0;
                var b = 10 / a;

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
                //context.Result = new BadRequestObjectResult(JsonConvert.SerializeObject(
                //    new
                //    {
                //        Error = HttpStatusCode.BadRequest.ToString(),
                //        Message = e.Message
                //    }
                //));


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


    public class AAAutorizacionFilter : IAsyncActionFilter
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
            }
            catch (Exception e)
            {
                //context.Result = new BadRequestObjectResult(JsonConvert.SerializeObject(
                //    new
                //    {
                //        Error = HttpStatusCode.BadRequest.ToString(),
                //        Message = e.Message
                //    }
                //));


                context.HttpContext.Response.Headers.Clear();
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    Error = HttpStatusCode.InternalServerError.ToString(),
                    Message = e.Message
                });
            }

            await next();   

        }
    }

    public class AAAutorizacion2Filter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            throw new NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
