using AuthNet.Models.DTO;
using AuthNet.Services;
using AuthNet.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var task = _service.GetTaskById(id);
            return task == null
                ? NotFound(new { Message = $"Task with ID {id} not found." })
                : Ok(task);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetTaskCount()
        {
            var count = await _service.GetTaskCountAsync();
            return Ok(count);
        }

        [HttpPost]
        public IActionResult Create([FromBody] TaskItemDto dto)
        {
            var result = _service.AddTask(dto);
            if (!result.Success)
                return BadRequest(new { result.Message });

            return Ok(new { result.Message });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] TaskItemDto dto)
        {
            var result = _service.UpdateTask(id, dto);
            if (!result.Success)
                return NotFound(new { result.Message });

            return Ok(new { result.Message });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _service.DeleteTask(id);
            if (!result.Success)
                return NotFound(new { result.Message });

            return Ok(new { result.Message });
        }
    }
}
