using AuthNet.Models.DTO;
using AuthNet.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequestDto request)
        {
            if (string.IsNullOrEmpty(request.ToEmail) || string.IsNullOrEmpty(request.Subject))
                return BadRequest("ToEmail and Subject are required.");

            var result = await _emailService.SendEmailAsync(request.ToEmail, request.Subject, request.Body);

            if (result)
                return Ok(new { message = "Email sent successfully." });

            return StatusCode(500, new { message = "Failed to send email." });
        }

        [HttpPost("send-forgot-password")]
        public async Task<IActionResult> SendForgotPasswordEmail(string email)
        {
            string userName = "John"; // Get from DB
            string resetLink = "https://yourapp.com/reset?token=abc123";

            var success = await _emailService.SendForgotPasswordEmailAsync(email, userName, resetLink);

            if (success)
                return Ok("Email sent");
            return StatusCode(500, "Failed to send email");
        }

        [HttpPost("send-new-user")]
        public async Task<IActionResult> SendNewUserEmail([FromBody] NewUserEmailDto request)
        {
            if (string.IsNullOrWhiteSpace(request.ToEmail) || string.IsNullOrWhiteSpace(request.UserName))
                return BadRequest("ToEmail and UserName are required.");

            var result = await _emailService.SendNewUserEmailAsync(request.ToEmail, request.UserName);

            return result
                ? Ok("Welcome email sent.")
                : StatusCode(500, "Failed to send welcome email.");
        }


    }
}
