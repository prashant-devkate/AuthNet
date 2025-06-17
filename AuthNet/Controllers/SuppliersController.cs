using AuthNet.Models.Domain;
using AuthNet.Services;
using AuthNet.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AuthNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _service;

        public SuppliersController(ISupplierService service)
        {
            _service = service;
        }

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
            var result = await _service.AddAsync(supplier);
            if (!result.response.Success)
            {
                return BadRequest(new { result.response.Message });
            }
            var addedSupplier = await _service.GetByIdAsync(supplier.SupplierId);
            return CreatedAtAction(nameof(GetById), new { id = supplier.SupplierId }, addedSupplier);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Supplier supplier)
        {
            var result = await _service.UpdateAsync(id, supplier);

            if (!result.response.Success)
            {
                return NotFound(new { result.response.Message });
            }

            return Ok(new { result.response.Message });
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
