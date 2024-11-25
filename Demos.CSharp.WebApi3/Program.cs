using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Net;
using System.Text.RegularExpressions;
using Demos.CSharp.Data;
using Demos.CSharp.WebApi3.AttributesFilters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Demos.CSharp.WebApi3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ////////////////////////////////////////////////////////////
            // Add services to the container.
            ////////////////////////////////////////////////////////////
            builder.Services.AddAuthorization();

            // Registrar los servicios para disponer del contexto de conexi�n a las bases de datos
            builder.Services.AddDbContext<DBNorthwind>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Northwind")));


            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();
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



            var app = builder.Build();

            ////////////////////////////////////////////////////////////
            // Configure the HTTP request pipeline.
            ////////////////////////////////////////////////////////////
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            ///////////////////////////////////////////////////////////

            // El c�digo crea un grupo de rutas con el prefijo /customers
            // Aplica un filtro de endpoints llamado CustomHeaders a todos los endpoints dentro del grupo.
            // Aplica un segundo filtro de endpoints, que permite ejecutar l�gica personalizada antes o despu�s de procesar las solicitudes.
            var customerGroup = app.MapGroup("/customers")
            .AddEndpointFilter<CustomHeaders>()
            .AddEndpointFilter(async (context, next) => {
                // Ejecuci�n antes del c�digo 
                    app.Logger.LogInformation("Ejecuci�n Filtro [antes] ...");
                var result = await next(context);

                // Ejecuci�n despu�s del c�digo 
                    app.Logger.LogInformation("Ejecuci�n Filtro [despu�s]...");
                    context.HttpContext.Response.Headers.Append("X-Server-Name2", Environment.MachineName);

                    return result;
                });

            // Define un endpoint dentro del grupo customerGroup que responde a solicitudes GET
            customerGroup.MapGet("", async (DBNorthwind db) =>
                await db.Customers.ToListAsync());

            // Define un endpoint dentro del grupo customerGroup que responde a solicitudes GET
            customerGroup.MapGet("/{id}", async (DBNorthwind db, string id) =>
            {
                app.Logger.LogInformation("Inicio del GET");

                return await db.Customers.FindAsync(id) is Customer customer
                    ? Results.Ok(customer)
                    : Results.NotFound();
            });

            // Define un endpoint dentro del grupo customerGroup que responde a solicitudes POST
            customerGroup.MapPost("", async (DBNorthwind db, Customer customer) => {
                if (customer == null) return Results.BadRequest();

                db.Customers.Add(customer);
                await db.SaveChangesAsync();

                return Results.Created($"/customers/{customer.CustomerID}", customer);
            });

            // Define un endpoint dentro del grupo customerGroup que responde a solicitudes PUT
            customerGroup.MapPut("/{id}", async (DBNorthwind db, string id, Customer customer) => {
                if (customer == null || customer.CustomerID != id) return Results.BadRequest();

                db.Customers.Update(customer);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });

            // Define un endpoint dentro del grupo customerGroup que responde a solicitudes DELETE
            customerGroup.MapDelete("/{id}", async (DBNorthwind db, string id) => {
                if (await db.Customers.FindAsync(id) is Customer customer)
                {
                    db.Customers.Remove(customer);
                    await db.SaveChangesAsync();

                    return Results.Ok();
                }
                else return Results.NotFound();
            });

            ///////////////////////////////////////////////////////////

            // Define un endpoint que responde a solicitudes GET /info
            app.MapGet("/info", () => Results.Ok(new { Mensaje = "Demo" }));

            ///////////////////////////////////////////////////////////

            /* Agrega un middleware global en la tuber�a de procesamiento de solicitudes. Esto es lo que hace:
               
                - Registra un mensaje de informaci�n en los logs de la aplicaci�n(Middleware 1.).
                - A�ade un encabezado personalizado llamado "Middleware" con el valor "1" a la respuesta HTTP.
                - Llama a next() para pasar el control al siguiente middleware o endpoint en la tuber�a.

               El middleware se ejecuta para todas las solicitudes antes de llegar a los endpoints definidos en la aplicaci�n.
            */
            app.Use(async (context, next) => {
                app.Logger.LogInformation("Middleware 1.");
                context.Response.Headers.Append("Middleware", "1");

                await next();
            });

            app.Use(async (context, next) => {
                app.Logger.LogInformation("Middleware 2.");
                context.Response.Headers.Append("Middleware", "2");

                await next();
            });

            // Este middleware realiza la validaci�n de una clave de API (APIKey).
            // Responde con c�digos de estado apropiados si la validaci�n falla o ocurre un error.
            //
            // PROPOSITO:
            // Garantizar que todas las solicitudes incluyan una clave de API v�lida antes de procesar
            // cualquier endpoint, a�adiendo una capa b�sica de seguridad.
            app.Use(async (context, next) =>
            {
                try
                {
                    string clave = builder.Configuration.GetValue<string>("Clave");

                    context.Request.Headers.TryGetValue("APIKey", out var apikey);
                    context.Request.Query.TryGetValue("APIKey", out var apikey2);


                    if (clave != apikey && clave != apikey2)
                    {
                        context.Response.Headers.Clear();
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                        await context.Response.WriteAsJsonAsync(new
                        {
                            Error = HttpStatusCode.Unauthorized.ToString(),
                            Message = "El APIKey no es valida."
                        });
                    }
                    else await next();
                }
                catch (Exception e)
                {
                    context.Response.Headers.Clear();
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    await context.Response.WriteAsJsonAsync(new
                    {
                        Error = HttpStatusCode.InternalServerError.ToString(),
                        Message = e.Message
                    });
                }
            });

            app.Run();
        }
    }
}
