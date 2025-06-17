using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMicroservice.Models;

namespace ProjectMicroservice.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly NorthwindContext _db;

        public CustomersController(NorthwindContext db)
        {
            _db = db;
        }

        [HttpGet("customers-from-germany")]
        public ActionResult<IEnumerable<object>> GetCustomersFromGermany()
        {
            var customers = _db.Customers
                .Where(c => c.Country == "Germany")
                .ToList();

            return Ok(customers);
        }

        [HttpGet("customers-by-country/{country}")]
        public ActionResult<IEnumerable<Customer>> GetCustomersByCountry(string country)
        {
            if (string.IsNullOrWhiteSpace(country))
                return BadRequest("Country parameter is required.");

            var customersQuery = _db.Customers
               .Where(c => (c.Country != null && c.Country.ToLower() == country.ToLower()));

            if (!customersQuery.Any())
                return NotFound($"No customers found in the specified country: {country}.");

            var customers = customersQuery.ToList();

            return Ok(customers);
        }
    }
}
