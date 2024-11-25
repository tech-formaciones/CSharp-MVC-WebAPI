
using Demos.CSharp.Data;
using Demos.CSharp.WebApi1.AttributesFilters;
using Demos.CSharp.WebApi1.Middleware;
using Demos.CSharp.WebApi1.Services;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;

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

            // Registra los servicios necesarios para usar controladores en una aplicación ASP.NET Core.
            builder.Services.AddControllers();

            // Registra los servicios necesarios para usar controladores en una aplicación ASP.NET Core y establece Filtros GLOBALES
            //builder.Services.AddControllers(options => {
            //    options.Filters.Add<Autorizacion2>();
            //});

            //////////////////////////////////////////////////////////////////////

            // Registrar los servicios para disponer del contexto de conexión a las bases de datos
            builder.Services.AddDbContext<DBNorthwind>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Northwind")));

            //////////////////////////////////////////////////////////////////////

            // Registra el uso de Swagger en etapa de diseño y depuración

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("APIKey", new OpenApiSecurityScheme
                {
                    Name = "APIKey",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Query,
                    Description = "APIKey necesaria para acceder al servicio."
                });

                // Agregar el requisito de seguridad
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "APIKey"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            //////////////////////////////////////////////////////////////////////

            // Registran servicios en el contenedor de dependencias con distintos ciclos de vida en ASP.NET Core:
            //  - AddSingleton: Una única instancia de Operation es creada y compartida en toda la aplicación.
            //  - AddScoped: Se crea una nueva instancia de Operation para cada solicitud HTTP.
            //  - AddTransient: Se crea una nueva instancia de Operation cada vez que se solicita el servicio.

            builder.Services.AddSingleton<IOperationSingleton, Operation>();
            builder.Services.AddScoped<IOperationScoped, Operation>();
            builder.Services.AddTransient<IOperationTransient, Operation>();

            //////////////////////////////////////////////////////////////////////

            // Construye la aplicación web a partir de la configuración definida en builder
            // preparándola para procesar solicitudes HTTP.

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


            // Canalización personalizada de seguridad de la API.
            // Validación de tokens de autenticación.
            //app.UseSecureAPI();


            // Canalización personalizada de la API.
            // Añade HEADERS de control en todas las respuestas.
            app.UseCustomHeaders("Mensaje de Prueba");


            // Configura las rutas para que las solicitudes HTTP sean manejadas por los controladores definidos en la aplicación.
            app.MapControllers();


            // Inicia la aplicación y comienza a escuchar solicitudes entrantes.
            // Es el punto donde el servidor web integrado(Kestrel) empieza a funcionar.
            app.Run();
        }
    }
}
