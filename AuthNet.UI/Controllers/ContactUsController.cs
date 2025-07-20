using Microsoft.AspNetCore.Mvc;

namespace AuthNet.UI.Controllers
{
    public class ContactUsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
