using AuthNet.Models.Domain;
using AuthNet.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderItemsController : ControllerBase
    {
        private readonly IPurchaseItemService _service;

        public PurchaseOrderItemsController(IPurchaseItemService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPurchaseOrderItems()
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("grouped")]
        public async Task<IActionResult> GetGroupedPurchaseOrderItems()
        {
            var items = await _service.GetGroupedPurchaseOrderItemsAsync();
            return Ok(items);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PurchaseOrderItems orderItem)
        {
            var created = await _service.AddAsync(orderItem);
            return CreatedAtAction(nameof(GetById), new { id = created.PurchaseOrderItemId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PurchaseOrderItems orderItem)
        {
            var updated = await _service.UpdateAsync(id, orderItem);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await _service.DeleteAsync(id) ? NoContent() : NotFound();
        }
    }
}
