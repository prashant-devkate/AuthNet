using AuthNet.Data;
using AuthNet.Models.DTO;
using AuthNet.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsService _settingsService;
        private readonly AppDbContext _context;

        public SettingsController(ISettingsService settingsService, AppDbContext dbContext)
        {
            _settingsService = settingsService;
            _context = dbContext;
        }

        [HttpPost]
        [Route("Save")]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB max
        public async Task<IActionResult> SaveCompanyInfo([FromForm] CompanyInfoDto request)
        {
            ValidateFileUpload(request);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _settingsService.SaveCompanyInfoAsync(request, Request);

            if (result.Success)
                return Ok(result); 

            return StatusCode(500, result);
        }

        private void ValidateFileUpload(CompanyInfoDto request)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".svg" };

            if (request.LogoUrl != null)
            {
                var logoExt = Path.GetExtension(request.LogoUrl.FileName).ToLower();
                if (!allowedExtensions.Contains(logoExt))
                    ModelState.AddModelError("Logo", "Invalid logo file extension");

                if (request.LogoUrl.Length > 5 * 1024 * 1024)
                    ModelState.AddModelError("Logo", "Logo file size cannot exceed 5MB");
            }

            if (request.SignatureUrl != null)
            {
                var signExt = Path.GetExtension(request.SignatureUrl.FileName).ToLower();
                if (!allowedExtensions.Contains(signExt))
                    ModelState.AddModelError("Signature", "Invalid signature file extension");

                if (request.SignatureUrl.Length > 5 * 1024 * 1024)
                    ModelState.AddModelError("Signature", "Signature file size cannot exceed 5MB");
            }
        }

        [HttpGet("Get")]
        public async Task<IActionResult> GetCompanyInfo()
        {
            var companyInfo = await _settingsService.GetCompanyInfoAsync();
            if (companyInfo == null)
                return NotFound("No company info found");

            return Ok(companyInfo);
        }

        [HttpGet("Get-History")]
        public async Task<IActionResult> GetCompanyHistoryInfo()
        {
            var companyHistoryList = await _settingsService.GetCompanyHistoryInfoAsync();

            if (companyHistoryList == null || !companyHistoryList.Any())
                return NotFound("No company history records found.");

            return Ok(companyHistoryList);
        }


        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromForm] CompanyInfoDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _settingsService.UpdateCompanyInfoAsync(request, Request);

            if (result.Success)
                return Ok(result);

            return StatusCode(500, result);
        }



        [HttpPost("SoftReset")]
        public async Task<IActionResult> SoftReset()
        {
            var success = await _settingsService.SoftResetCompanyInfoAsync();
            return success ? Ok("Company info has been cleared.") : NotFound("No company info found.");
        }

        [HttpGet("History")]
        public async Task<IActionResult> GetCompanyInfoHistory()
        {
            var history = await _context.CompanyInfoHistories
                .OrderByDescending(h => h.ArchivedAt)
                .ToListAsync();

            return Ok(history);
        }

    }
}
