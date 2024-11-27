using Azure;
using Demos.CSharp.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Demos.CSharp.WebApplication2.Controllers
{
    public class CustomersController : Controller
    {
        private readonly HttpClient _http;

        public CustomersController(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("default");
        }

        public IActionResult Index()
        {
            ViewBag.ErrorMessage = string.Empty;

            try
            {
                HttpResponseMessage response = _http.GetAsync("/customers").Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string dataJson = response.Content.ReadAsStringAsync().Result;
                    IEnumerable<Customer>? customers = 
                        JsonConvert.DeserializeObject<IEnumerable<Customer>>(dataJson);

                    return View(customers);
                }
                else
                {
                    ViewBag.ErrorMessage = $"Error: {response.StatusCode.ToString()}";
                    return View(new List<Customer>());
                }
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = $"Error: {e.Message}";
                return View(new List<Customer>());
            }
        }
    }
}
