using Demos.CSharp.WebApi1.Services;

namespace Demos.CSharp.WebApi1.Middleware
{
    /// <summary>
    /// Canalización personalizada de seguridad de la API. Añade HEADERS de control en todas las respuestas.
    /// </summary>
    public class CustomHeaders
    {
        private readonly RequestDelegate _next;
        private readonly IOperationSingleton _singleton;
        private readonly string _message;

        public Task Invoke(HttpContext context)
        {
            context.Response.Headers.Append("X-Server-Name", Environment.MachineName);
            context.Response.Headers.Append("X-Server-OSVersion", Environment.OSVersion.ToString());
            context.Response.Headers.Append("X-Application-Name", "Demo Curso");
            context.Response.Headers.Append("X-Singleton-Id", _singleton.OperationId);
            context.Response.Headers.Append("X-Message", _message);

            return _next(context);
        }

        public CustomHeaders(RequestDelegate next, IOperationSingleton singleton, string message)
        { 
            _next = next;
            _singleton = singleton;
            _message = message;
        }
    }

    /// <summary>
    /// Una extensión para el tipo IApplicationBuilder.
    /// Permite agregar un middleware personalizado llamado CustomHeaders al pipeline de la aplicación.
    /// </summary>
    public static class CustomHeadersExtensions
    {
        public static IApplicationBuilder UseCustomHeaders(this IApplicationBuilder builder, string message)
        { 
            return builder.UseMiddleware<CustomHeaders>(message);
        }
    }

}
