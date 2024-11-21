using System;
using Demos.CSharp.WebApplication1.Servicios;
using static System.Net.WebRequestMethods;

namespace Demos.CSharp.WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ////////////////////////////////////////////////////////////////////////
            // Add services to the container.
            ////////////////////////////////////////////////////////////////////////

            // Registra los servicios necesarios para habilitar los controladores y vistas en una aplicación web.
            builder.Services.AddControllersWithViews();

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

            // Solo se aplica si el entorno no es de desarrollo.
            if (!app.Environment.IsDevelopment())
            {
                // Redirige a una página de error personalizada, "/Home/Error" en caso de una excepción no controlada.
                app.UseExceptionHandler("/Home/Error");

                // Habilita HTTP Strict Transport Security(HSTS), obligando a los navegadores a usar HTTPS para futuras solicitudes al sitio.
                app.UseHsts();
            }

            // Redirige automáticamente las solicitudes HTTP a HTTPS.
            app.UseHttpsRedirection();

            // Sirve archivos estáticos (como CSS, JS, imágenes).
            app.UseStaticFiles();

            // Habilita el enrutamiento de solicitudes.
            app.UseRouting();

            // Activa la autorización para controlar el acceso a las rutas.
            // Requiere configuración previa.
            app.UseAuthorization();

            // Define la ruta predeterminada para los controladores /Home/Index.
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Inicia la aplicación y comienza a escuchar solicitudes entrantes.
            // Es el punto donde el servidor web integrado(Kestrel) empieza a funcionar.
            app.Run();
        }
    }
}
