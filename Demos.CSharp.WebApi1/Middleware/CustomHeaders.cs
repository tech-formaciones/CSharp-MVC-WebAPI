using Demos.CSharp.WebApi1.Services;

namespace Demos.CSharp.WebApi1.Middleware
{
    public class CustomHeaders
    {
        private readonly RequestDelegate _next;
        private readonly IOperationSingleton _singleton;
        private readonly string _message;

        public Task Invoke(HttpContext context)
        {
            context.Response.Headers.Add("X-Server-Name", Environment.MachineName);
            context.Response.Headers.Add("X-Server-OSVersion", Environment.OSVersion.ToString());
            context.Response.Headers.Add("X-Application-Name", "Demo Curso");
            context.Response.Headers.Add("X-Singleton-Id", _singleton.OperationId);
            context.Response.Headers.Add("X-Message", _message);

            return _next(context);
        }

        public CustomHeaders(RequestDelegate next, IOperationSingleton singleton, string message)
        { 
            _next = next;
            _singleton = singleton;
            _message = message;
        }
    }

    public static class CustomHeadersExtensions
    {
        public static IApplicationBuilder UseCustomHeaders(this IApplicationBuilder builder, string message)
        { 
            return builder.UseMiddleware<CustomHeaders>(message);
        }
    }

}
