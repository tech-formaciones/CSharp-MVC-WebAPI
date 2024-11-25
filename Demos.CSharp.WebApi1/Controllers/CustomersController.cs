using Demos.CSharp.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demos.CSharp.WebApi1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly DBNorthwind _db;

        public CustomersController(DBNorthwind db) 
        { 
            _db = db;
        }


        // GET /api/customers
        // GET /api/customers?company=&city=&country=
        [HttpGet]
        public ActionResult Get([FromQuery] string company, [FromQuery] string city, [FromQuery]string country)
        {
            try
            {
                IQueryable<Customer> customers = _db.Customers;

                if (!string.IsNullOrEmpty(company))
                    customers = customers.Where(r => r.CompanyName.Contains(company));

                if (!string.IsNullOrEmpty(city))
                    customers = customers.Where(r => r.City == city);

                if (!string.IsNullOrEmpty(country))
                    customers = customers.Where(r => r.Country == country);

                return Ok(customers.ToList());
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
        }

        // GET /api/customers/ANATR
        [HttpGet("{id}")]
        public ActionResult Get(string id)
        {
            try
            {
                var customer = _db.Customers
                    .Where(r => r.CustomerID == id)
                    .FirstOrDefault();

                if (customer == null) return NotFound(new { Message = "El cliente no existe" });
                else return Ok(customer);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
        }

        // POST /api/customers
        [HttpPost]
        public ActionResult Post(Customer customer)
        {
            if (customer == null) return BadRequest(new { Message = "Faltan datos del cliente."});

            try
            {
                _db.Customers.Add(customer);
                _db.SaveChanges();

                return CreatedAtAction("Get", new { id = customer.CustomerID }, customer);
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
        }

        // PUT /api/customers/{id}
        [HttpPut("{id}")]
        public ActionResult Put(string id, Customer customer)
        {
            if (customer.CustomerID != id) return BadRequest(new { Message = "Error en indentificadores." });

            try
            {
                _db.Update(customer);
                _db.SaveChanges();

                return NoContent();
            }
            catch (DbUpdateException e)
            { 
                return Conflict(new { e.Message });
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }


        }

        // DELETE /api/customers/{id}
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        { 
            //var customer = _db.Customers.Where(r => r.CustomerID == id).FirstOrDefault();
            var customer = _db.Customers.Find(id);    
            
            if (customer == null) return NotFound(new { Message = "El cliente no existe." });

            try
            {
                _db.Customers.Remove(customer);
                _db.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                return Problem(detail: e.Message, statusCode: 500);
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        // GET /api/customers
        [HttpGet("v2")]
        public IEnumerable<Customer> Get2()
        {
            return _db.Customers.ToList();
        }
    }
}
