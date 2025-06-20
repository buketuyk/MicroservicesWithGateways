using CodeFirstMicroservice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeFirstMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusesController : ControllerBase
    {
        private readonly TaskManagementContext _context;

        public StatusesController(TaskManagementContext taskManagement)
        {
            _context = taskManagement;
        }

        //// GET: api/Statuses
        //[HttpGet]
        //public async Task<ActionResult<Status>> GetAll()
        //{
           

        //}

        //// GET: api/Statuses/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Status>> Get(int id)
        //{

        //}

        //// POST: api/Statuses
        //[HttpPost]
        //public async Task<ActionResult<Status>> Post([FromBody] Status status)
        //{

        //}         

    }
}
