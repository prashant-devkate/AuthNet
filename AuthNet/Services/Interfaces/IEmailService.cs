namespace AuthNet.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string body);
        Task<bool> SendForgotPasswordEmailAsync(string toEmail, string userName, string resetLink);
        Task<bool> SendNewUserEmailAsync(string toEmail, string userName);

    }
}
