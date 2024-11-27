using Demos.CSharp.Data;
using Microsoft.EntityFrameworkCore;

namespace Demos.CSharp.WebApplication2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ////////////////////////////////////////////////////////////
            // Add services to the container.
            ////////////////////////////////////////////////////////////
            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

            // Registrar los servicios para disponer del contexto de conexión a las bases de datos
            builder.Services.AddDbContext<DBNorthwind>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Northwind")));

            //Regristrar el servicio para disponer de un cliente HTTP 
            builder.Services.AddHttpClient("default", options => { 
                options.BaseAddress = 
                    new Uri(builder.Configuration.GetValue<string>("APIUrl"));

                options.DefaultRequestHeaders.Add("APIKey",
                    builder.Configuration.GetValue<string>("APIClave"));
            });

            var app = builder.Build();

            ////////////////////////////////////////////////////////////
            // Configure the HTTP request pipeline.
            ////////////////////////////////////////////////////////////
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
