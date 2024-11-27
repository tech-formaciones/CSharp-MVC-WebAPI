using Demos.CSharp.Data;
using Microsoft.AspNetCore.Mvc;

namespace Demos.CSharp.WebApplication1.Controllers
{
    public class Demo2Controller : Controller
    {
        private readonly DBNorthwind _db;

        public Demo2Controller(DBNorthwind db)
        { 
            _db = db;
        }

        public IActionResult Index()
        {
            var cliente = _db.Customers
                .Where(r => r.CustomerID == "ANATR")
                .FirstOrDefault();

            ViewData["Title"] = "Demo2 | Index";
            ViewBag.Cliente = cliente;
            ViewData["Cliente"] = cliente;

            //TempData["Valor"] = cliente;

            return View(cliente);
        }

        public IActionResult Search(string id)
        {
            var cliente = _db.Customers
                .Where(r => r.CustomerID == id)
                .FirstOrDefault();

            ViewData["Title"] = "Demo2 | Search";


            return View(cliente);
        }

        public IActionResult First()
        {
            var cliente = _db.Customers
                .Where(r => r.CustomerID == "ANTON")
                .FirstOrDefault();

            ViewBag.Action = "First";
            ViewData["Title"] = "Demo2 | First";

            // Ejecuta FIRST, Ejecuta SECOND y retorna la vista SECOND
            return RedirectToAction("Second");

            // Ejecuta FIRST y retorna la vista FIRST
            return View();

            // Ejecuta FIRST y retorna la vista SECOND
            return View("Second");
        }

        public IActionResult Second()
        {
            ViewBag.Action = "Second";
            ViewData["Title"] = "Demo2 | Second";

            return View();
        }

        [HttpGet]
        public IActionResult DemoGet()
        {
            var cliente = _db.Customers
                .Where(r => r.CustomerID == "ANTON")
                .FirstOrDefault();

            // Retorna el JSON del cliente + Status Code 200
            //return ("Mensaje de texto");

            // Retorna el JSON del cliente + Status Code 200
            return Ok(cliente);

            // Retorna el JSON del cliente
            return Json(cliente);

            // Retorna un fragmento de HTML (partial)
            return PartialView("_DemoPartial");

            // Retorna un vista completa HTML
            return View();
        }

        [HttpPost]
        public IActionResult DemoPost()
        {
            return View();
        }
    }
}
