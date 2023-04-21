using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using Dapper;

namespace MyApplication.Controllers
{
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly IConfiguration _config;
        public class UserTaskDto
        {
            public string Name { get; set; }
            public List<string> Tasks { get; set; }
        }

        public TaskController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        [Route("api/tasks/AddUserWithTask")]
        public IActionResult AddUserWithTask([FromBody] UserTaskDto userTaskDto)
        {
            try
            {
                using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();

                    // Insert user
                    var insertUserSql = "INSERT INTO [users] ([name]) VALUES (@name); SELECT SCOPE_IDENTITY();";
                    var userId = connection.ExecuteScalar<int>(insertUserSql, new { name = userTaskDto.Name });

                    // Insert tasks
                    foreach (var taskDetail in userTaskDto.Tasks)
                    {
                        var insertTaskSql = "INSERT INTO [tasks] ([task_detail], [fk_users_id]) VALUES (@taskDetail, @userId)";
                        connection.Execute(insertTaskSql, new { taskDetail, userId });
                    }

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet]
        [Route("api/tasks/GetUserWithTask")]
        public IActionResult GetUserWithTask([FromQuery] string name)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var getUserWithTaskSql = "SELECT [u].[pk_users_id], [u].[name], [t].[pk_task_id], [t].[task_detail] " +
                                             "FROM [users] [u] " +
                                             "LEFT JOIN [tasks] [t] ON [u].[pk_users_id] = [t].[fk_users_id] " +
                                             "WHERE [u].[name] LIKE @name";
                    var userTasks = connection.Query<UserTaskQueryResult>(getUserWithTaskSql, new { name = $"%{name}%" });

                    var result = new List<UserDto>();
                    foreach (var userTask in userTasks.GroupBy(ut => ut.UserId))
                    {
                        var user = new UserDto { Id = userTask.Key, Name = userTask.First().Name };
                        user.Tasks = userTask.Where(ut => ut.TaskId != null).Select(ut => ut.TaskDetail).ToList();
                        result.Add(user);
                    }

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        public class UserTaskQueryResult
        {
            public int UserId { get; set; }
            public string Name { get; set; }
            public int? TaskId { get; set; }
            public string TaskDetail { get; set; }
        }

        public class UserDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public List<string> Tasks { get; set; }
        }
        ...
[HttpGet]
        [Route("api/tasks/GetUserWithTask")]
        public IActionResult GetUserWithTask([FromQuery] string name)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Get users and tasks based on name filter
                    var getUserWithTaskSql = @"SELECT [u].[pk_users_id], [u].[name], [t].[pk_task_id], [t].[task_detail]
                                       FROM [users] [u] 
                                       LEFT JOIN [tasks] [t] ON [u].[pk_users_id] = [t].[fk_users_id] 
                                       WHERE [u].[name] LIKE @name + '%'";
                    var userTasks = connection.Query<UserTaskDto>(getUserWithTaskSql, new { name });

                    // Group tasks by user
                    var result = userTasks.GroupBy(u => new { u.Id, u.Name })
                                          .Select(g => new UserTaskDto
                                          {
                                              Id = g.Key.Id,
                                              Name = g.Key.Name,
                                              Tasks = g.Select(t => new TaskDto { Id = t.TaskId, Detail = t.TaskDetail }).ToList()
                                          })
                                          .ToList();

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
...

    }
}
