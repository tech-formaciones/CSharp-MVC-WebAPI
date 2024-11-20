using System.Net;
using Microsoft.Extensions.Primitives;

namespace Demos.CSharp.WebApi1.Middleware
{
    public class SecureAPI
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly string _message;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                string clave = _configuration.GetValue<string>("Clave");
                
                StringValues apikey;
                context.Request.Headers.TryGetValue("APIKey", out apikey);

                if (clave == apikey) await _next(context);
                else 
                {
                    context.Response.Headers.Clear();
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                    await context.Response.WriteAsJsonAsync(new
                    {
                        Error = HttpStatusCode.Unauthorized.ToString(),
                        Message = "El APIKey no es valida."
                    }); 
                }
            }
            catch (Exception e)
            {
                context.Response.Headers.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                await context.Response.WriteAsJsonAsync(new {
                    Error = HttpStatusCode.InternalServerError.ToString(),
                    Message = e.Message
                });
            }
        }

        public SecureAPI(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }
    }

    public static class SecureAPIExtensions
    {
        public static IApplicationBuilder UseSecureAPI(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SecureAPI>();
        }
    }
}
