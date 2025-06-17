using Microsoft.AspNetCore.Mvc;

namespace AuthNet.UI.Controllers
{
    public class AuditLogsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
