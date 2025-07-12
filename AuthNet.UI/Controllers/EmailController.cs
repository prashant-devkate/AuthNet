using AuthNet.UI.Helpers;
using AuthNet.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace AuthNet.UI.Controllers
{
    public class EmailController : Controller
    {
        public IActionResult Send()
        {
            return View(new EmailSendViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Send(EmailSendViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var toEmails = model.ToEmails.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var keyvals = model.KeyValuePairs.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            await EmailHandler.SendEmail(
                model.FromEmail,
                model.FromName,
                toEmails,
                model.Subject,
                model.SelectedTemplate,
                keyvals
            );

            ViewBag.Message = "Email sent successfully!";
            return View(model);
        }
    }
}
