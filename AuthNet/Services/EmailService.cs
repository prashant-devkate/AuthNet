using AuthNet.Helpers;
using AuthNet.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace AuthNet.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient(_configuration["Smtp:Host"])
                {
                    Port = int.Parse(_configuration["Smtp:Port"]),
                    Credentials = new NetworkCredential(
                        _configuration["Smtp:Username"],
                        _configuration["Smtp:Password"]
                    ),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_configuration["Smtp:Username"]),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SendForgotPasswordEmailAsync(string toEmail, string userName, string resetLink)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "ForgotPasswordTemplate.html");

            var placeholders = new Dictionary<string, string>
                {
                    { "UserName", userName },
                    { "ResetLink", resetLink }
                };

            var body = EmailTemplateHelper.LoadTemplate(templatePath, placeholders);

            return await SendEmailAsync(toEmail, "Reset Your Password", body);
        }

        public async Task<bool> SendNewUserEmailAsync(string toEmail, string userName)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "NewUserTemplate.html");

            var placeholders = new Dictionary<string, string>
                {
                    { "UserName", userName }
                };

            var body = EmailTemplateHelper.LoadTemplate(templatePath, placeholders);

            return await SendEmailAsync(toEmail, "Welcome to Our Platform", body);
        }


    }
}
