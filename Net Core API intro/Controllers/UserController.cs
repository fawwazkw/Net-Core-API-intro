using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net_Core_API_intro.Data;
using Net_Core_API_intro.DTOs;
using Net_Core_API_intro.Models;
using Task = Net_Core_API_intro.Models.Task;

namespace Net_Core_API_intro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserData _userData;

        public UserController(UserData userData)
        {
            _userData = userData;
        }

        [HttpPost("AddUserWithTask")]
        public IActionResult AddUserWithTask([FromBody] UserDto userDto)
        {
            try
            {
                User user = new User()
                {
                    name = userDto.name
                };

                Task task = new Task()
                {
                    task_detail = userDto.task_detail
                };

                bool result = _userData.InsertData(task, user);

                if (result)
                {
                    return StatusCode(201, userDto);
                }
                else
                {
                    return StatusCode(500, "Data not Inserted");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
                throw;
            }
        }

        [HttpGet("GetUserWithTask")]
        public IActionResult GetAll()
        {
            try
            {
                List<User> users = _userData.GetAll();
                return StatusCode(200, users);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetUserWithTask{name}")]
        public IActionResult GetByName(string name)
        {
            try
            {
                User? user = _userData.GetByName(name);
                if (user == null)
                {
                    return NotFound($"Data {name} Not Found");
                }

                return StatusCode(200, user);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }
    }
}
