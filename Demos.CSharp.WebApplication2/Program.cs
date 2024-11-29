using Demos.CSharp.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
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

            // Registrar los servicios de autenticación basados en una Cookie
            builder.Services.AddAuthentication(
                CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options => {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                options.SlidingExpiration = true;
                options.AccessDeniedPath = "/account/forbidden";
                options.LoginPath = "/account/login";
                options.LogoutPath = "/account/logout";
            });

            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Registrar el uso de una Cookie para estados de sesión
            builder.Services.AddSession(options => {
                options.Cookie.Name = "AppSession";
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddAntiforgery();

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

            app.UseAntiforgery();


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Strict });
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
