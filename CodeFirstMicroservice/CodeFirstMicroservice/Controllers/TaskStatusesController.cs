using CodeFirstMicroservice.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeFirstMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskStatusesController : ControllerBase
    {
        private readonly TaskManagementContext _db;

        public TaskStatusesController(TaskManagementContext taskManagement)
        {
            _db = taskManagement;
        }

        [HttpGet]
        public ActionResult<IEnumerable<object>> Get()
        {
            var taskStatuses = _db.TaskStatuses.ToList();

            return Ok(taskStatuses);
        }
    }
}
