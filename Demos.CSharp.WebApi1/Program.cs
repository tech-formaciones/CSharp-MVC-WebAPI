
using Demos.CSharp.WebApi1.Middleware;
using Demos.CSharp.WebApi1.Services;
using Microsoft.OpenApi.Models;

namespace Demos.CSharp.WebApi1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ////////////////////////////////////////
            // Add services to the container.
            ////////////////////////////////////////
            
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
                options.AddSecurityDefinition("APIKey", new OpenApiSecurityScheme { 
                    Name = "APIKey",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Description = "APIKey necesaria para acceder al servicio."
                })
            );

            builder.Services.AddSingleton<IOperationSingleton, Operation>();
            builder.Services.AddScoped<IOperationScoped, Operation>();
            builder.Services.AddTransient<IOperationTransient, Operation>();

            var app = builder.Build();


            ////////////////////////////////////////
            // Configure the HTTP request pipeline.
            ////////////////////////////////////////

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            //app.UseAuthorization();
            app.UseSecureAPI();

            app.UseCustomHeaders("Mensaje de Prueba");

            app.MapControllers();

            app.Run();
        }
    }
}
