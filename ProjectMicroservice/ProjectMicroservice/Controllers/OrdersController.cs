using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMicroservice.Models;
using System.Collections;
using System.Reflection.Metadata.Ecma335;

namespace ProjectMicroservice.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly NorthwindContext _db;

        public OrdersController(NorthwindContext db)
        {
            _db = db;                
        }

        // Çalışan başına toplam sipariş sayısını listele
        [HttpGet("count-of-orders-by-employee")]
        public ActionResult<IEnumerable<object>> GetCountOfOrdersByEmployee()
        {
            var orders = _db.Orders
                .GroupBy(o => new
                {
                    o.EmployeeId,
                    FirstName = o.Employee != null ? o.Employee.FirstName : "Unknown",
                    LastName = o.Employee != null ? o.Employee.LastName : "Unknown"
                })
                .Select(g => new
                {
                    TotalOrdersByEmployee = g.Count(),
                    EmployeeFullName = g.Key.FirstName + " " + g.Key.LastName
                })
                .ToList();

            return Ok(orders);
        }

        [HttpGet("total-orders-by-customer")]
        public ActionResult<IEnumerable<object>> Get()
        {
            var orders = _db.Orders
                .Where(o => o.Customer != null)
                .GroupBy(
                    o => o.Customer != null ? o.Customer.CustomerId : "Unknown Customer"
                )
                .Select(g => new
                {
                    CustomerId = g.Key,
                    CustomerName = g.First().Customer.ContactName,
                    TotalOrder = g.Count()
                })
                .ToList();

            return Ok(orders);
        }

        [HttpGet("get-orders-by-years/{year}")]
        public ActionResult<IEnumerable<Order>> GetOrdersByYears(string year) // 1996-07-04 00:00:00.000
        {
            if (!int.TryParse(year, out int yearValue))
                return BadRequest("Invalid year value.");

            var startDate = new DateTime(yearValue, 1, 1);
            var endDate = startDate.AddYears(1);

            var orders = _db.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate < endDate)
                .ToList();

            return orders.Any() ? Ok(orders) : NotFound($"No orders found for year {year}.");
        }
    }
}
