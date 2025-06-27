using AuthNet.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace AuthNet.UI.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            var vm = new ProfileViewModel
            {
                Username = HttpContext.Session.GetString("Username"),
                Role = HttpContext.Session.GetString("Role")
            };
            return View(vm);

        }
    }
}
