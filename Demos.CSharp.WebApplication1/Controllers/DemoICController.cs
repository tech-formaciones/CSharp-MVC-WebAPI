

using Demos.CSharp.WebApplication1.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace Demos.CSharp.WebApplication1.Controllers
{
    public class DemoICController : Controller
    {
        private readonly IOperationSingleton _singleton;
        private readonly IOperationScoped _scoped;
        private readonly IOperationTransient _transient;

        public IActionResult Index()
        {
            ViewBag.Singleton = _singleton.OperationId;
            ViewBag.ContSingleton = _singleton.Cont;

            ViewData["Scoped"] = _scoped.OperationId;
            ViewData["Transient"] = _transient.OperationId;

            return View();
        }

        public DemoICController(IOperationSingleton singleton, IOperationScoped scoped, IOperationTransient transient)
        { 
            _singleton = singleton;
            _scoped = scoped;
            _transient = transient;
        }
    }
}
