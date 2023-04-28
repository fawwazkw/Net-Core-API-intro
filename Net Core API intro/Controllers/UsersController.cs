using Microsoft.AspNetCore.Mvc;
using Net_Core_API_intro.Models;
using Net_Core_API_intro.Data;

namespace Net_Core_API_intro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("AddUserWithTask")]
        public ActionResult AddUserWithTask(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();

            foreach (Task task in user.Tasks)
            {
                task.UserId = user.Id;
                _context.Tasks.Add(task);
            }

            _context.SaveChanges();

            return Ok();
        }

        [HttpGet("GetUserWithTask")]
        public ActionResult GetUserWithTask(string name)
        {
            var user = _context.Users.FirstOrDefault(u => u.Name == name);

            if (user == null)
            {
                return NotFound();
            }

            user.Tasks = _context.Tasks.Where(t => t.UserId == user.Id).ToList();

            return Ok(user);
        }
    }
}
