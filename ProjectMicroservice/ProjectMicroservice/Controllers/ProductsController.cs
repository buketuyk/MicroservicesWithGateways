using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMicroservice.Models;
using System.Linq;

namespace ProjectMicroservice.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly NorthwindContext _db;

        public ProductsController(NorthwindContext db)
        {
            _db = db;
        }

        [HttpGet]
        public ActionResult<IEnumerable<object>> GetProductList()
        {
            var products = _db.Products
                .Take(10)
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    p.QuantityPerUnit,
                    p.UnitPrice,
                    p.CategoryId,
                    p.Category
                })
                .ToList();

            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public ActionResult GetById(int id)
        {
            var product = _db.Products
                .Where(p => p.ProductId == id)
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    p.CategoryId,
                    Category = p.Category == null ? null : new
                    {
                        p.Category.CategoryId,
                        p.Category.CategoryName,
                        p.Category.Description
                    }
                })
                .FirstOrDefault();

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public ActionResult<Product> Create([FromBody] Product product)
        {
            _db.Products.Add(product);
            _db.SaveChanges();


            return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, product);  // https://localhost:7219/api/Categories/21  Indicates the URL to redirect a page to.
        }

        [HttpGet("price-greater-than-20")]
        public ActionResult<IEnumerable<Product>> GetProductsWithPriceGreaterOrEqual20()
        {
            var products = _db.Products
                              .Where(p => p.UnitPrice >= 20)
                              .OrderByDescending(p => p.UnitPrice)
                              .Select(p =>  new {
                                  p.ProductName,
                                  p.UnitPrice
                              })
                              .ToList();
            
            if(!products.Any())
                return NotFound("No products found with UnitPrice >= 20.");

            return Ok(products);
        }

        // Kategorilere göre ürünlerin ortalama fiyatı
        [HttpGet("average-price-of-products-by-category")]
        public ActionResult<IEnumerable<object>> GetAveragePriceOfProductsByCategory()
        {
            var products = _db.Products
                .GroupBy(p => new
                {
                    p.CategoryId,
                    CategoryName = p.Category != null && p.Category.CategoryName != null
                        ? p.Category.CategoryName
                        : "Uncategorized"
                })
                .Select(g => new
                {
                    CategoryID = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName ?? "Uncategorized",
                    AvgProductPrice = g.Average(p => p.UnitPrice)
                })
                .ToList();

            return Ok(products);
        }

        [HttpGet("out-of-stock")]
        public ActionResult<IEnumerable<object>> GetOutOfStockProducts()
        {
            var products = _db.Products
                .Where(p => p.UnitsInStock.HasValue && p.UnitsInStock == 0)
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    p.UnitsInStock
                })
                .ToList();

            return Ok(products);
        }
    }
}
