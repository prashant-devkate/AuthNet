using AuthNet.Models.Domain;
using AuthNet.Services;
using AuthNet.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound(new { Message = $"Category with ID {id} not found." }) : Ok(item);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCategoryCount()
        {
            var count = await _service.GetCategoryCountAsync();
            return Ok(count);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            var result = await _service.AddAsync(category);

            if (!result.Success)
            {
                return BadRequest(new { result.Message });
            }

            var created = await _service.GetByIdAsync(category.CategoryId);
            return CreatedAtAction(nameof(GetById), new { id = category.CategoryId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Category category)
        {
            var result = await _service.UpdateAsync(id, category);

            if (!result.Success)
            {
                return NotFound(new { result.Message });
            }

            return Ok(new { result.Message });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result.Success)
            {
                return BadRequest(new { result.Message });
            }

            return Ok(new { result.Message });
        }
    }
}
