using AuthNet.Enums;
using AuthNet.Models.Domain;
using AuthNet.Services;
using AuthNet.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthNet.Controllers
{
    //[Authorize(Roles = Roles.Admin)]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetProductCount()
        {
            var count = await _service.GetProductCountAsync();
            return Ok(count);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            try
            {
                var created = await _service.AddAsync(product);
                return CreatedAtAction(nameof(GetById), new { id = created.ProductId }, created);
            }
            catch(DbUpdateException ex)
            {
                return StatusCode(500, "Database error: " + ex.InnerException?.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Product product)
        {
            var updated = await _service.UpdateAsync(id, product);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await _service.DeleteAsync(id) ? NoContent() : NotFound();
        }
    }

}
