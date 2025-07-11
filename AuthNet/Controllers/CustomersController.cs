using AuthNet.Models.Domain;
using AuthNet.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomersController(ICustomerService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _service.GetAllAsync();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound(new { Message = $"Customer with ID {id} not found." }) : Ok(item);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCustomerCount()
        {
            var count = await _service.GetCustomerCountAsync();
            return Ok(count);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Customer customer)
        {
            var result = await _service.AddAsync(customer);

            if (!result.Success)
            {
                return BadRequest(new { result.Message });
            }

            var created = await _service.GetByIdAsync(customer.CustomerId);
            return CreatedAtAction(nameof(GetById), new { id = customer.CustomerId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Customer customer)
        {
            var result = await _service.UpdateAsync(id, customer);

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
