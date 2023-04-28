using Microsoft.AspNetCore.Mvc;
using Net_Core_API_intro.Models;
using Net_Core_API_intro.Data;

namespace Net_Core_API_intro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult GetTasks()
        {
            var tasks = _context.Tasks.ToList();

            return Ok(tasks);
        }
    }
}
