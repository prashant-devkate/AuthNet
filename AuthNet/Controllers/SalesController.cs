using AuthNet.Models.Domain;
using AuthNet.Models.DTO;
using AuthNet.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ISalesService _reportService;

        public SalesController(ISalesService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("Daily")]
        public async Task<IActionResult> GetDailySales()
        {
            var result = await _reportService.GetDailySalesAsync();
            return Ok(result);
        }

        [HttpGet("Monthly")]
        public async Task<IActionResult> GetMonthlySales()
        {
            var result = await _reportService.GetMonthlySalesAsync();
            return Ok(result);
        }

        [HttpGet("Yearly")]
        public async Task<IActionResult> GetYearlySales()
        {
            var result = await _reportService.GetYearlySalesAsync();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sales = await _reportService.GetAllAsync();
            return Ok(sales);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _reportService.GetByIdAsync(id);
            return item == null
                ? NotFound(new { Message = $"Sale with ID {id} not found." })
                : Ok(item);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetSaleCount()
        {
            var count = await _reportService.GetSaleCountAsync();
            return Ok(count);
        }
        [HttpPost]
        public async Task<IActionResult> Create(SaleDto sale)
        {
            var result = await _reportService.AddAsync(sale);

            if (!result.Success)
                return BadRequest(new { result.Message });

            return Ok(new { result.Success, result.Message });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Sale sale)
        {
            var result = await _reportService.UpdateAsync(id, sale);
            if (!result.Success)
                return NotFound(new { result.Message });

            return Ok(new { result.Message });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _reportService.DeleteAsync(id);
            if (!result.Success)
                return BadRequest(new { result.Message });

            return Ok(new { result.Message });
        }
    }
}
