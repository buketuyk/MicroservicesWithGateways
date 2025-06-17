using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectMicroservice.Models;

namespace ProjectMicroservice.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly NorthwindContext _db;

        public EmployeesController(NorthwindContext db)
        {
            _db = db;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Employee>> GetAll()
        {
            var employees =  _db.Employees
                .Include(e => e.ReportsToNavigation) // todob: self-referencing -  should be DTO
                .ToList();

            if (!employees.Any())
                return NotFound("No employees found.");

            return Ok(employees);
        }


        [HttpGet("employees-info")]
        public ActionResult<IEnumerable<object>> GetEmployeesInfo()
        {
            var employeesInfo = _db.Employees
                .Select(e => new
                {
                    FullName = string.Concat(e.FirstName, " ", e.LastName)
                })
                .OrderBy(e => e.FullName)
                .ToList();

            return Ok(employeesInfo);
        }

        [HttpPost("create-employee")]
        public ActionResult<object> Create(Employee employee)
        {
            _db.Employees.Add(employee);
            _db.SaveChanges();
            
            return Ok(employee);
        }
    }
}
