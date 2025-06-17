using Microsoft.AspNetCore.Mvc;

namespace AuthNet.UI.Controllers
{
    public class PurchaseOrdersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
