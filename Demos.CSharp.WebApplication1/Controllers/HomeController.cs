using System.Diagnostics;
using Demos.CSharp.WebApplication1.Models;
using Demos.CSharp.WebApplication1.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace Demos.CSharp.WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOperationSingleton _singleton;


        public HomeController(ILogger<HomeController> logger, IOperationSingleton s)
        {
            _logger = logger;
            _singleton = s;
        }

        public IActionResult Index()
        {
            ViewBag.Singleton = _singleton.OperationId;
            ViewBag.ContSingleton = _singleton.Cont;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
