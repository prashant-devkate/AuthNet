using Microsoft.AspNetCore.Mvc;

namespace AuthNet.UI.Controllers
{
    public class NotFoundController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
