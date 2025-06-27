using AuthNet.Models.Domain;
using AuthNet.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaxSettingsController : ControllerBase
    {
        private readonly ITaxSettingService _service;

        public TaxSettingsController(ITaxSettingService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound(new { Message = $"Tax settings with ID {id} not found." }) : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TaxSetting tax)
        {
            var result = await _service.AddAsync(tax);

            if (!result.Success)
            {
                return BadRequest(new { result.Message });
            }

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TaxSetting tax)
        {
            var result = await _service.UpdateAsync(id, tax);

            if (!result.Success)
            {
                return NotFound(new { result.Message });
            }

            return Ok(new { result.Message });
        }

    }
}
