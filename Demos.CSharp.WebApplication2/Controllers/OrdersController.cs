using Demos.CSharp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Demos.CSharp.WebApplication2.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly HttpClient _http;

        public OrdersController(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("default");
        }

        public IActionResult Index()
        {
            ViewBag.ErrorMessage = string.Empty;

            try
            {
                IEnumerable<Order>? orders =
                    _http.GetFromJsonAsync<IEnumerable<Order>>("/orders").Result;

                return View(orders);
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = $"Error: {e.Message}";
                return View(new List<Order>());
            }
        }

        public IActionResult List(string id)
        {
            IEnumerable<Order>? orders;

            try
            {
                if (id.ToLower() == "all")
                {
                    orders = _http.GetFromJsonAsync<IEnumerable<Order>>("/orders").Result;
                }
                else
                {
                    orders = _http.GetFromJsonAsync<IEnumerable<Order>>($"customers/{id}/orders").Result;
                }
                return PartialView("_ListadoPedidos", orders);
            }
            catch (Exception)
            {
                return View("_ListadoPedidos", new List<Order>());
            }
        }
    }
}
