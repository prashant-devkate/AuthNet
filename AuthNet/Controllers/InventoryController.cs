using AuthNet.Models.Domain;
using AuthNet.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _service;

        public InventoryController(IInventoryService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound(new { Message = $"Inventory with ID {id} not found." }) : Ok(item);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetInventoryCount()
        {
            var count = await _service.GetInventoryCountAsync();
            return Ok(count);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Inventory inventory)
        {
            var result = await _service.AddAsync(inventory);

            if (!result.Success)
            {
                return BadRequest(new { result.Message });
            }

            var created = await _service.GetByIdAsync(inventory.InventoryId);
            return CreatedAtAction(nameof(GetById), new { id = inventory.InventoryId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Inventory inventory)
        {
            var result = await _service.UpdateAsync(id, inventory);

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
