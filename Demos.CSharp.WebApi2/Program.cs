
using System.Diagnostics.Metrics;
using Demos.CSharp.Data;
using Demos.CSharp.WebApi2.AttributesFilters;
using Demos.CSharp.WebApi2.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Demos.CSharp.WebApi2
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
            builder.Services.AddSwaggerGen();

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

            // Define un endpoint que responde a solicitudes GET
            app.MapGet("/customers", async (DBNorthwind db) => await db.Customers.ToListAsync());

            /*
              Este endpoint combina funcionalidad, filtros personalizados y soporte para documentaci�n Swagger.
              Este c�digo define un endpoint en ASP.NET Core Minimal API que:

                Ruta: 
                 + Responde a GET /customers/{id} para obtener un cliente por su ID desde la base de datos DBNorthwind.

                Documentaci�n OpenAPI:
                 + Establece un nombre (Clientes) y una descripci�n del endpoint.
                 + Configura detalles del par�metro id en Swagger (requerido, no vac�o, ubicaci�n en la ruta).

                Filtros de Endpoint:
                 + Filtro 1 y 2: Ejecutan l�gica antes y despu�s del endpoint (loguean mensajes, modifican encabezados).
                 + CustomHeaders: Aplica un filtro adicional predefinido.

                Devuelve:
                 + C�digo 200 OK si encuentra el cliente.
                 + Cdigo 404 Not Found si no lo encuentra.
             */
            app.MapGet("/customers/{id}", async (DBNorthwind db, string id) => {
                app.Logger.LogInformation("Inicio del GET");

                return await db.Customers.FindAsync(id) is Customer customer
                    ? Results.Ok(customer)
                    : Results.NotFound();
            })
            .WithName("Clientes")
            .WithDescription("Retornar los datos de un cliente desde su identificador")
            .WithOpenApi(options => {
                var parameter = options.Parameters[0];
                parameter.Description = "Identificador del cliente";
                parameter.Required = true;
                parameter.AllowEmptyValue = false;
                parameter.In = Microsoft.OpenApi.Models.ParameterLocation.Path;

                return options;
            })
            .AddEndpointFilter(async (context, next) => {
                // Ejecuci�n antes del c�digo 
                app.Logger.LogInformation("Ejecuci�n Filtro [antes] ...");

                var result = await next(context);

                // Ejecuci�n despu�s del c�digo 
                app.Logger.LogInformation("Ejecuci�n Filtro [despu�s]...");
                context.HttpContext.Response.Headers.Append("X-Server-Name", Environment.MachineName);

                return result;  
            })
            .AddEndpointFilter(async (context, next) => {
                // Ejecuci�n antes del c�digo 
                app.Logger.LogInformation("Ejecuci�n Filtro 2 [antes] ...");

                var result = await next(context);

                // Ejecuci�n despu�s del c�digo 
                app.Logger.LogInformation("Ejecuci�n Filtro 2 [despu�s]...");

                return result;
            })
            .AddEndpointFilter<CustomHeaders>();

            // Define un endpoint que responde a solicitudes POST
            app.MapPost("/customers", async (DBNorthwind db, Customer customer) => {
                if (customer == null) return Results.BadRequest();

                db.Customers.Add(customer);
                await db.SaveChangesAsync();

                return Results.Created($"/customers/{customer.CustomerID}", customer);
            });

            // Define un endpoint que responde a solicitudes PUT
            app.MapPut("/customers/{id}", async (DBNorthwind db, string id, Customer customer) => { 
                if(customer == null || customer.CustomerID != id) return Results.BadRequest();

                db.Customers.Update(customer);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });

            // Define un endpoint que responde a solicitudes DELETE
            app.MapDelete("/customers/{id}", async (DBNorthwind db, string id) => {
                if (await db.Customers.FindAsync(id) is Customer customer)
                {
                    db.Customers.Remove(customer);
                    await db.SaveChangesAsync();

                    return Results.Ok();
                }
                else return Results.NotFound();
            });

            /////////////////////////////////////////////////////////////////////////////////

            /*
             /api/v0/customers:
              + Filtra clientes en base a los par�metros company, city, y country usando IQueryable<Customer>.
              + Maneja errores con un bloque try-catch y devuelve un error 500 si ocurre una excepci�n.

             /api/v1/customers:
              + Llama a DBCustomers.ToList() para obtener clientes filtrados.
              + No tiene manejo de excepciones expl�cito.
             
             /api/v2/customers:
              + Usa ToListAsync() para obtener los clientes de manera as�ncrona.
              + Filtra por los mismos par�metros company, city, y country.
            
             /api/v2.1/customers:
              + Similar a la versi�n /v2, pero adem�s verifica si el resultado es un IEnumerable<Customer>.
              + Si es v�lido, devuelve un 200 OK, de lo contrario un 400 BadRequest.
            
             Diferencias clave:
              - v0 usa ToList() de manera s�ncrona con manejo de errores.
              - v1 usa ToList() sin manejo de errores.
              - v2 y v2.1 son as�ncronos con ToListAsync(), pero v2.1 valida si el resultado es un IEnumerable<Customer>.
             */

            app.MapGet("/api/v0/customers", (DBNorthwind db,
                [FromQuery] string? company,
                [FromQuery] string? city,
                [FromQuery] string? country) => {
                    try
                    {
                        IQueryable<Customer> customers = db.Customers;

                        if (!string.IsNullOrEmpty(company))
                            customers = customers.Where(r => r.CompanyName.Contains(company));

                        if (!string.IsNullOrEmpty(city))
                            customers = customers.Where(r => r.City == city);

                        if (!string.IsNullOrEmpty(country))
                            customers = customers.Where(r => r.Country == country);

                        return Results.Ok(customers.ToList());
                    }
                    catch (Exception e)
                    {
                        return Results.Problem(detail: e.Message, statusCode: 500);
                    }
                });

            app.MapGet("/api/v1/customers", (DBNorthwind db, 
                [FromQuery] string? company, 
                [FromQuery] string? city, 
                [FromQuery] string? country) => DBCustomers.ToList(db, company, city,country));

            app.MapGet("/api/v2/customers", async (DBNorthwind db,
                [FromQuery] string? company,
                [FromQuery] string? city,
                [FromQuery] string? country) => await DBCustomers.ToListAsync(db, company, city, country));

            app.MapGet("/api/v2.1/customers", async (DBNorthwind db,
                [FromQuery] string? company,
                [FromQuery] string? city,
                [FromQuery] string? country) => 
                    await DBCustomers.ToListAsync(db, company, city, country)
                        is IEnumerable<Customer> customers
                            ? Results.Ok(customers)
                            : Results.BadRequest());

            /////////////////////////////////////////////////////////////////////////////////


            app.Run();
        }
    }
}
