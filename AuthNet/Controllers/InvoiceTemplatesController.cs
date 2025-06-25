using AuthNet.Models.Domain;
using AuthNet.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceTemplatesController : ControllerBase
    {
        private readonly IInvoiceTemplateService _service;

        public InvoiceTemplatesController(IInvoiceTemplateService service)
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
            return item == null ? NotFound(new { Message = $"Invoice template with ID {id} not found." }) : Ok(item);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCategoryCount()
        {
            var count = await _service.GetInvoiceTemplateCountAsync();
            return Ok(count);
        }

        [HttpPost]
        public async Task<IActionResult> Create(InvoiceTemplate template)
        {
            var result = await _service.AddAsync(template);

            if (!result.Success)
            {
                return BadRequest(new { result.Message });
            }

            var created = await _service.GetByIdAsync(template.Id);
            return CreatedAtAction(nameof(GetById), new { id = template.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, InvoiceTemplate template)
        {
            var result = await _service.UpdateAsync(id, template);

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
