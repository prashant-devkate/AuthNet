using Microsoft.AspNetCore.Mvc;

namespace AuthNet.UI.Controllers
{
    public class InventoriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
