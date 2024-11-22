using Demos.CSharp.WebApi1.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Demos.CSharp.WebApi1.AttributesFilters;
using Demos.CSharp.Data;

namespace Demos.CSharp.WebApi1.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private readonly IOperationSingleton _singleton;
        private readonly IOperationScoped _scoped;
        private readonly IOperationTransient _transient;

        [HttpGet]
        public object Get()
        {
            return new 
            {
                Method = "GET",
                Singleton = _singleton.OperationId,
                Scoped = _scoped.OperationId,
                Transient = _transient.OperationId
            };
        }

        [HttpPost]
        public object Post()
        {
            return new
            {
                Method = "POST",
                Singleton = _singleton.OperationId,
                Scoped = _scoped.OperationId,
                Transient = _transient.OperationId
            };
        }

        public InfoController(IOperationSingleton singleton, IOperationScoped scoped, IOperationTransient transient)
        {
            _singleton = singleton;
            _scoped = scoped;
            _transient = transient;
        }
    }
}
