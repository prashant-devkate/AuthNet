using AuthNet.Models.Domain;
using AuthNet.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrdersController(IOrderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _service.GetAllAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetOrderCount()
        {
            var count = await _service.GetOrderCountAsync();
            return Ok(count);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Order order)
        {
            var result = await _service.AddAsync(order);
            if (!result.response.Success)
            {
                return BadRequest(new { result.response.Message });
            }
            var addedOrder = await _service.GetByIdAsync(order.OrderId);
            return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, addedOrder);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Order order)
        {
            var result = await _service.UpdateAsync(id, order);

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
