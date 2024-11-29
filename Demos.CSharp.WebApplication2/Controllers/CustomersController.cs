using System.Text;
using Azure;
using Demos.CSharp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Demos.CSharp.WebApplication2.Controllers
{
    [Authorize]
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

        [HttpGet]
        public IActionResult Edit(string id)
        {
            ViewBag.InfoMessage = TempData["InfoMessage"];

            ViewBag.ErrorMessage = string.Empty;

            try
            {
                HttpResponseMessage response = _http.GetAsync($"/customers/{id}").Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string dataJson = response.Content.ReadAsStringAsync().Result;
                    Customer? customer =
                        JsonConvert.DeserializeObject<Customer>(dataJson);

                    ViewBag.Pedidos = GetOrders(id);

                    return View(customer);
                }
                else
                {
                    ViewBag.ErrorMessage = $"Error: {response.StatusCode.ToString()}";
                    return View(new Customer());
                }
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = $"Error: {e.Message}";
                return View(new Customer());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Customer customer)
        {
            ViewBag.Pedidos = GetOrders(customer.CustomerID);

            if (ModelState.IsValid)
            {
                try
                {
                    string customerJSON = JsonConvert.SerializeObject(customer);
                    StringContent resquestContent =
                        new StringContent(customerJSON, Encoding.UTF8, "application/json");

                    HttpResponseMessage response =
                        _http.PutAsync($"/customers/{customer.CustomerID}", resquestContent).Result;

                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        ViewBag.InfoMessage = $"Cliente actulizado correctamente.";
                        return View(customer);
                    }
                    else
                    {
                        ViewBag.ErrorMessage = $"Error: {response.StatusCode.ToString()}";
                        return View(customer);
                    }
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = $"Error: {e.Message}";
                    return View(new List<Customer>());
                }
            }
            return View(customer);

        }

        [HttpGet]
        public IActionResult New()
        {
            return View(new Customer());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult New(Customer customer)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string customerJSON = JsonConvert.SerializeObject(customer);
                    StringContent resquestContent =
                        new StringContent(customerJSON, Encoding.UTF8, "application/json");

                    HttpResponseMessage response =
                        _http.PostAsync($"/customers", resquestContent).Result;

                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        TempData["InfoMessage"] = $"Cliente creado correctamente.";
                        return RedirectToAction("edit", new { id = customer.CustomerID});
                    }
                    else
                    {
                        ViewBag.ErrorMessage = $"Error: {response.StatusCode.ToString()}";
                        return View(customer);
                    }
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = $"Error: {e.Message}";
                    return View(new List<Customer>());
                }
            }
            return View(customer);

        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            try
            {
                HttpResponseMessage response = _http.DeleteAsync($"/customers/{id}").Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK) return Json(new { result = "OK", message = "" });
                else return Json(new { result = "NOK", message = response.StatusCode.ToString() });

            }
            catch (Exception e)
            {
                return Json(new { result = "error", message = e.Message });
            }
        }

        [HttpPost]
        public IActionResult Delete2(string id)
        {
            try
            {
                HttpResponseMessage response = _http.DeleteAsync($"/customers/{id}").Result;

                if (response.StatusCode == System.Net.HttpStatusCode.OK) return Content($"<span id=\"dataresult\" data-result=\"OK\">");
                else return Content($"<span id=\"dataresult\" data-result=\"NOK\">");

            }
            catch (Exception e)
            {
                return Content($"<span id=\"dataresult\" data-result=\"error\" data-message=\"{e.Message}\">");
            }
        }

        private IEnumerable<Order>? GetOrders(string id)
        {
            var orders = _http
                .GetFromJsonAsync<IEnumerable<Order>>($"/customers/{id}/orders").Result;

            return orders; 
        }
    }
}
