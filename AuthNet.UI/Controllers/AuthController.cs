using AuthNet.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace AuthNet.UI.Controllers
{
    public class AuthController : Controller
    {
        private readonly HttpClient _httpClient;

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var response = await _httpClient.PostAsJsonAsync("/api/Auth/register", model);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Login");

            ModelState.AddModelError(string.Empty, "Registration failed.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var response = await _httpClient.PostAsJsonAsync("/api/Auth/login", model);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                HttpContext.Session.SetString("JWT", result.Token);
                HttpContext.Session.SetString("Username", model.Username);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid login.");
            return View(model);
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

    }
}
