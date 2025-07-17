using Microsoft.AspNetCore.Mvc;

namespace AuthNet.UI.Controllers
{
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
