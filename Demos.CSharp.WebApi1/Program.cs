
using Demos.CSharp.WebApi1.AttributesFilters;
using Demos.CSharp.WebApi1.Middleware;
using Demos.CSharp.WebApi1.Services;
using Microsoft.OpenApi.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace Demos.CSharp.WebApi1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ////////////////////////////////////////////////////////////////////////
            // Add services to the container.
            ////////////////////////////////////////////////////////////////////////

            // Registra los servicios necesarios para usar controladores en una aplicaci�n ASP.NET Core.
            builder.Services.AddControllers();

            // Registra los servicios necesarios para usar controladores en una aplicaci�n ASP.NET Core y establece Filtros GLOBALES
            //builder.Services.AddControllers(options => {
            //    options.Filters.Add<Autorizacion2>();
            //});

            //////////////////////////////////////////////////////////////////////

            // Registra el uso de Swagger en etapa de dise�o y depuraci�n

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
                options.AddSecurityDefinition("APIKey", new OpenApiSecurityScheme { 
                    Name = "APIKey",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Description = "APIKey necesaria para acceder al servicio."
                })
            );

            //////////////////////////////////////////////////////////////////////

            // Registran servicios en el contenedor de dependencias con distintos ciclos de vida en ASP.NET Core:
            //  - AddSingleton: Una �nica instancia de Operation es creada y compartida en toda la aplicaci�n.
            //  - AddScoped: Se crea una nueva instancia de Operation para cada solicitud HTTP.
            //  - AddTransient: Se crea una nueva instancia de Operation cada vez que se solicita el servicio.

            builder.Services.AddSingleton<IOperationSingleton, Operation>();
            builder.Services.AddScoped<IOperationScoped, Operation>();
            builder.Services.AddTransient<IOperationTransient, Operation>();

            //////////////////////////////////////////////////////////////////////

            // Construye la aplicaci�n web a partir de la configuraci�n definida en builder
            // prepar�ndola para procesar solicitudes HTTP.

            var app = builder.Build();

            ////////////////////////////////////////////////////////////////////////

            /*******************************************************************************************************************/

            ////////////////////////////////////////////////////////////////////////
            // Configure the HTTP request pipeline.
            ////////////////////////////////////////////////////////////////////////

            // Habilita Swagger en el entorno de desarrollo.

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            // Habilita diferentes funcionalidades basadas en canalizaciones del Middleware.

            app.UseHttpsRedirection();
            app.UseAuthorization();


            // Canalizaci�n personalizada de seguridad de la API.
            // Validaci�n de tokens de autenticaci�n.
            //app.UseSecureAPI();


            // Canalizaci�n personalizada de la API.
            // A�ade HEADERS de control en todas las respuestas.
            app.UseCustomHeaders("Mensaje de Prueba");


            // Configura las rutas para que las solicitudes HTTP sean manejadas por los controladores definidos en la aplicaci�n.
            app.MapControllers();


            // Inicia la aplicaci�n y comienza a escuchar solicitudes entrantes.
            // Es el punto donde el servidor web integrado(Kestrel) empieza a funcionar.
            app.Run();
        }
    }
}
