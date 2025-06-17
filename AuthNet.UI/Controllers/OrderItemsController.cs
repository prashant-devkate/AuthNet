using Microsoft.AspNetCore.Mvc;

namespace AuthNet.UI.Controllers
{
    public class OrderItemsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
