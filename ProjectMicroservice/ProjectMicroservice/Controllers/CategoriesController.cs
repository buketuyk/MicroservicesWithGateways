using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ProjectMicroservice.Models;
using StackExchange.Profiling;

namespace ProjectMicroservice.Controllers
{
    [Route("api/v2/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v2")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly NorthwindContext _db;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(NorthwindContext db, ILogger<CategoriesController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        [EnableRateLimiting("DataPolicy")]
        public ActionResult<IEnumerable<object>> GetCategoryList()
        {
            var categories = _db.Categories
                .Take(10)
                .Select(c => 
                new
                {
                    c.CategoryId,
                    c.CategoryName,
                    c.Description
                })
                .ToList();
            return Ok(categories);
        }

        [HttpGet("{id:int}")]
        public ActionResult<Category> GetById(int id)
        {
            using (MiniProfiler.Current.Step("Category | GetById | The registration tracking process has been initiated."))
            { 
                //var category = _db.Categories
                //    .Where(c => c.CategoryId == id)
                //    .Select(c => new
                //    {
                //        c.CategoryId,
                //        c.CategoryName,
                //        c.Description
                //    })
                //    .FirstOrDefault();

                var category = _db.Categories
                    .Where(c => c.CategoryId == id)
                    .FirstOrDefault();

                if (category == null)
                    return NotFound();

                return Ok(category);
            }
        }

        [HttpPost]
        public ActionResult<Category> Create([FromBody] Category c)
        {
            _logger.LogInformation("CategoriesController | Create request received {@Category}", c);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("CategoriesController | Create request model validation failed: {@ModelStateErrors}", ModelState.Values.SelectMany(v => v.Errors));
                return BadRequest(ModelState); // 400
            }

            try
            {
                _db.Categories.Add(c);
                _db.SaveChanges();

                _logger.LogInformation("CategoriesController | Category created successfully with Id: {CategoryId}", c.CategoryId);

                return CreatedAtAction(nameof(GetById), new { id = c.CategoryId }, c); // 201 Created
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CategoriesController | Database error occurred while creating category {@Category}", c);
                return StatusCode(500, "Database issue: " + ex.Message);
            }
        }

        [HttpPut]
        public ActionResult<Category> Update([FromBody] Category c)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (c == null || c.CategoryId == 0)
                return BadRequest("Missing Category Id!"); // 400

            var existingCategory = _db.Categories.Find(c.CategoryId);
            if (existingCategory == null)
                return NotFound();

            // _db.Categories.Update(c); todob
            existingCategory.CategoryName = c.CategoryName;
            existingCategory.Description = c.Description;

            _db.SaveChanges();

            return Ok(existingCategory);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var category = _db.Categories.Find(id);
            if (category == null)
                return NotFound();
            
            _db.Categories.Remove(category);
            _db.SaveChanges();
            
            return Ok(new { success = true, message = $"Deleted category id: {id} category name: {category.CategoryName} successfully"}); // 204
        }
    }
}
