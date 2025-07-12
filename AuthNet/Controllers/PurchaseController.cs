using AuthNet.Models.Domain;
using AuthNet.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseService _service;

        public PurchaseController(IPurchaseService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var entries = await _service.GetAllAsync();
            return Ok(entries);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null
                ? NotFound(new { Message = $"Purchase order with ID {id} not found." })
                : Ok(item);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetPurchaseOrderCount()
        {
            var count = await _service.GetPurchaseOrderCountAsync();
            return Ok(count);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PurchaseOrder purchase)
        {
            var result = await _service.AddAsync(purchase);
            if (!result.Success)
                return BadRequest(new { result.Message });

            return CreatedAtAction(nameof(GetById), new { id = purchase.PurchaseOrderId }, purchase);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PurchaseOrder purchase)
        {
            var result = await _service.UpdateAsync(id, purchase);
            if (!result.Success)
                return NotFound(new { result.Message });

            return Ok(new { result.Message });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result.Success)
                return BadRequest(new { result.Message });

            return Ok(new { result.Message });
        }
    }
}
