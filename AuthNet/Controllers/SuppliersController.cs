using AuthNet.Models.Domain;
using AuthNet.Services;
using AuthNet.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _service;

        public SuppliersController(ISupplierService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var suppliers = await _service.GetAllAsync();
            return Ok(suppliers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetSupplierCount()
        {
            var count = await _service.GetSupplierCountAsync();
            return Ok(count);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Supplier supplier)
        {
            var created = await _service.AddAsync(supplier);
            return CreatedAtAction(nameof(GetById), new { id = created.SupplierId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Supplier supplier)
        {
            var updated = await _service.UpdateAsync(id, supplier);
            return updated == null ? NotFound() : Ok(updated);
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
