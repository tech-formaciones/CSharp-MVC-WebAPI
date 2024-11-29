using System.Security.Claims;
using Demos.CSharp.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Demos.CSharp.WebApplication2.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Usuario? dataLogin, [FromQuery] string? returnUrl)
        {
            string? resturnUrl2 = HttpContext.Request.Query["ReturnUrl"];

            if (ModelState.IsValid)
            {
                // 1. Autenticar (obligatorio)
                // Para la demo el usuario valido es demo@curso.com con 12345678 de contraseña
                var user = AuthenticateUser(dataLogin.Email, dataLogin.Password);
                if (user == null)
                {
                    ViewBag.ErrorMessage = $"Intento de inicio de sesión no valido.";
                    return View();
                }

                // 1b. Incorporar autenticación MFA

                // 2. Crear los Claims (opcional)
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim("FullName", user.FullName),
                    new Claim(ClaimTypes.GroupSid, "Administrators"),
                    new Claim(ClaimTypes.Role, "Administrator"),
                    new Claim(ClaimTypes.Role, "User"),
                };
                
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // 3. Creamos las Propiedades de Autenticación (opcional)
                var authProperties = new AuthenticationProperties() { };

                // 4. Vamos a marcar la sesión como autenticada
                HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties).Wait();

                if (string.IsNullOrEmpty(returnUrl)) returnUrl = "/home";

                return Redirect(returnUrl);
            }
            else
            {
                ViewBag.ErrorMessage = $"Datos no validos para el inicio de sesión.";
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
            return View();
        }

        public IActionResult Forbidden()
        {
            return View();
        }

        private Usuario AuthenticateUser(string? user, string? password)
        {
            // Para la demo el usuario valido es demo@curso.com con 12345678 de contraseña
            if (user!.ToLower() == "demo@curso.com" && password == "12345678")
            {
                return new Usuario() 
                { 
                    Email = user, 
                    Password = "", 
                    FullName = "Adrian Sanz" 
                };
            }
            else return null;
        }
    }
}
