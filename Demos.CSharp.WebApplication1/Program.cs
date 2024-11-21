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

            // Registra los servicios necesarios para habilitar los controladores y vistas en una aplicaci�n web.
            builder.Services.AddControllersWithViews();

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

            // Solo se aplica si el entorno no es de desarrollo.
            if (!app.Environment.IsDevelopment())
            {
                // Redirige a una p�gina de error personalizada, "/Home/Error" en caso de una excepci�n no controlada.
                app.UseExceptionHandler("/Home/Error");

                // Habilita HTTP Strict Transport Security(HSTS), obligando a los navegadores a usar HTTPS para futuras solicitudes al sitio.
                app.UseHsts();
            }

            // Redirige autom�ticamente las solicitudes HTTP a HTTPS.
            app.UseHttpsRedirection();

            // Sirve archivos est�ticos (como CSS, JS, im�genes).
            app.UseStaticFiles();

            // Habilita el enrutamiento de solicitudes.
            app.UseRouting();

            // Activa la autorizaci�n para controlar el acceso a las rutas.
            // Requiere configuraci�n previa.
            app.UseAuthorization();

            // Define la ruta predeterminada para los controladores /Home/Index.
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Inicia la aplicaci�n y comienza a escuchar solicitudes entrantes.
            // Es el punto donde el servidor web integrado(Kestrel) empieza a funcionar.
            app.Run();
        }
    }
}
