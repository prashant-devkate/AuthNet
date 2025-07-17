using Microsoft.AspNetCore.Mvc;

namespace AuthNet.UI.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
