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
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetByProductId(int productId)
        {
            var item = await _service.GetByProductIdAsync(productId);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdate(Inventory inventory)
        {
            var result = await _service.AddOrUpdateAsync(inventory);
            return Ok(result);
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete(int productId)
        {
            return await _service.DeleteAsync(productId) ? NoContent() : NotFound();
        }
    }
}
