using System.Net.Mail;
using AuthNet.UI.Enums;
using SendGrid;
using SendGrid.Helpers.Mail;


namespace AuthNet.UI.Helpers
{
    public static class EmailHandler
    {
        public static async Task SendEmail(string fromEmailId, string fromName, string[] toEmailIds, string emailSub, int emailTemplateType, string[] keyvals)
        {
            string emailTemplate = string.Empty;
            string emailBody = string.Empty;
            string emailSubject = string.Empty;
            //string apiKey = System.Configuration.ConfigurationSettings.AppSettings["SendGridKey"].ToString();
            string apiKey = "Sc";
            switch (emailTemplateType)
            {
                case (int)EmailTemplates.NewUserLocal:
                    emailTemplate = "MailTemplates\\NewUserLocal.html";
                    emailSubject = string.IsNullOrEmpty(emailSub) ? "Welcome to SolHub" : emailSub;
                    break;
                case (int)EmailTemplates.NewUserGoogle:
                    emailTemplate = "MailTemplates\\NewUserGoogle.html";
                    emailSubject = string.IsNullOrEmpty(emailSub) ? "Welcome to SolHub" : emailSub;
                    break;
                case (int)EmailTemplates.ForgetPassword:
                    emailTemplate = "MailTemplates\\ForgetPassword.html";
                    emailSubject = string.IsNullOrEmpty(emailSub) ? "SolHub new password details" : emailSub;
                    break;
                case (int)EmailTemplates.ForgetIpin:
                    emailTemplate = "MailTemplates\\ForgetIpin.html";
                    emailSubject = string.IsNullOrEmpty(emailSub) ? "SolHub new unique key details" : emailSub;
                    break;
                case (int)EmailTemplates.RegenerateIpinUser:
                    emailTemplate = "MailTemplates\\RegenerateIpinUser.html";
                    emailSubject = string.IsNullOrEmpty(emailSub) ? "SolHub new unique key generate request" : emailSub;
                    break;
                case (int)EmailTemplates.RegenerateIpinSuperAdmin:
                    emailTemplate = "MailTemplates\\RegenerateIpinSuperAdmin.html";
                    emailSubject = string.IsNullOrEmpty(emailSub) ? "SolHub new unique key generate request" : emailSub;
                    break;
                case (int)EmailTemplates.RegeneratePasswordUser:
                    emailTemplate = "MailTemplates\\RegeneratePasswordUser.html";
                    emailSubject = string.IsNullOrEmpty(emailSub) ? "SolHub new passward generate request" : emailSub;
                    break;
                case (int)EmailTemplates.RegeneratePasswordSuperAdmin:
                    emailTemplate = "MailTemplates\\RegeneratePasswordSuperAdmin.html";
                    emailSubject = string.IsNullOrEmpty(emailSub) ? "SolHub new passward generate request" : emailSub;
                    break;
                case (int)EmailTemplates.ApprovalEmail:
                    emailTemplate = "MailTemplates\\ApprovalEmail.html";
                    emailSubject = string.IsNullOrEmpty(emailSub) ? "SolHub Blueprint approval request" : emailSub;
                    break;
                case (int)EmailTemplates.ApprovalStatus:
                    emailTemplate = "MailTemplates\\ApprovalStatus.html";
                    emailSubject = string.IsNullOrEmpty(emailSub) ? "SolHub Blueprint approval status" : emailSub;
                    break;
                case (int)EmailTemplates.DeploymentStatus:
                    emailTemplate = "MailTemplates\\DeploymentStatus.html";
                    emailSubject = string.IsNullOrEmpty(emailSub) ? "SolHub Blueprint deployment status" : emailSub;
                    break;
                case (int)EmailTemplates.NewCustomer:
                    emailTemplate = "MailTemplates\\NewCustomer.html";
                    emailSubject = string.IsNullOrEmpty(emailSub) ? "Welcome to SolHub" : emailSub;
                    break;
                case (int)EmailTemplates.NewRoleMap:
                    emailTemplate = "MailTemplates\\NewRoleMap.html";
                    emailSubject = string.IsNullOrEmpty(emailSub) ? "Welcome to SolHub" : emailSub;
                    break;
                case (int)EmailTemplates.NewRoleMapApproval:
                    emailTemplate = "MailTemplates\\NewRoleMapApproval.html";
                    emailSubject = string.IsNullOrEmpty(emailSub) ? "Welcome to SolHub" : emailSub;
                    break;
                case (int)EmailTemplates.EmailOTP:
                    emailTemplate = "MailTemplates\\EmailOTP.html";
                    emailSubject = string.IsNullOrEmpty(emailSub) ? "OTP Notification" : emailSub;
                    break;
            }
            using StreamReader reader = new(emailTemplate);
            emailBody = reader.ReadToEnd();
            foreach (string keyval in keyvals)
                emailBody = emailBody.Replace("{" + keyval.Split('|')[0] + "}", keyval.Split('|')[1]);

            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(fromEmailId, fromName),
                Subject = emailSubject,
                PlainTextContent = "",
                HtmlContent = emailBody,
            };

            foreach (string toEmailId in toEmailIds)
                msg.AddTo(new EmailAddress(toEmailId));
            var response = await client.SendEmailAsync(msg);
        }
    }
}
