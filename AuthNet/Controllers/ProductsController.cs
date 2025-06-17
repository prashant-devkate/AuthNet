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
        public async Task<IActionResult> GetAll()
        {
            var products = await _service.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null
                ? NotFound(new { Message = $"Product with ID {id} not found." })
                : Ok(item);
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
            var result = await _service.AddAsync(product);
            if (!result.Success)
                return BadRequest(new { result.Message });

            return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Product product)
        {
            var result = await _service.UpdateAsync(id, product);
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
