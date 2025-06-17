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
    public class OrderDetailsController : ControllerBase
    {
        private readonly NorthwindContext _db;

        public OrderDetailsController(NorthwindContext db)
        {
            _db = db;
        }

        [HttpGet("top5-ordered-products")]
        public ActionResult<IEnumerable<object>> GetTop5OrderedProducts()
        {
            var orderDetails = _db.OrderDetails
                .GroupBy(od => od.Product.ProductName)
                .Select(g => new
                    { 
                        ProductName = g.Key,
                        TotalQuantity = g.Sum(od => od.Quantity)
                    })
                .OrderByDescending(d => d.TotalQuantity)
                .Take(5)
                .ToList();

            return Ok(orderDetails);
        }

        /*
        SELECT TOP 5 p.ProductName
	    ,SUM(od.Quantity) AS TotalQuantity
        FROM [Order Details] od
        JOIN Products p ON od.ProductId = p.ProductId
        GROUP BY p.ProductName
        ORDER BY SUM(od.Quantity) DESC; 
         */
    }
}
