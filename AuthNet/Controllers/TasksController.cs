using AuthNet.Models.DTO;
using AuthNet.Services;
using AuthNet.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _service;
        public TasksController(ITaskService taskService)
        {
            _service = taskService;
        }

        [HttpGet]
        public IActionResult GetTasks()
        {
            var tasks = _service.GetAllTasks();
            return Ok(tasks);
        }

        // GET: api/Tasks/{id}
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var task = _service.GetTaskById(id);
            if (task == null) return NotFound();
            return Ok(task);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetTaskCount()
        {
            var count = await _service.GetTaskCountAsync();
            return Ok(count);
        }

        // POST: api/Tasks
        [HttpPost]
        public IActionResult Create([FromBody] TaskItemDto dto)
        {
            var success = _service.AddTask(dto);
            if (!success) return BadRequest("Failed to add task.");
            return Ok();
        }

        // PUT: api/Tasks/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] TaskItemDto dto)
        {
            var success = _service.UpdateTask(id, dto);
            if (!success) return NotFound("Task not found.");
            return Ok();
        }

        // DELETE: api/Tasks/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var success = _service.DeleteTask(id);
            if (!success) return NotFound("Task not found.");
            return Ok();
        }
    }
}
