using AuthNet.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using SendGrid;

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

            var registerResponse = await _httpClient.PostAsJsonAsync("/api/Auth/register", model);

            if (registerResponse.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "User registered successfully.";

                var emailPayload = new
                {
                    ToEmail = model.Email,
                    UserName = model.Firstname
                };

                var emailResponse = await _httpClient.PostAsJsonAsync("/api/Email/send-new-user", emailPayload);

                return RedirectToAction("Login");
            }
            var errorMessage = await registerResponse.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = errorMessage;
            ModelState.AddModelError(string.Empty, "Registration failed.");
            return RedirectToAction("Register");
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
                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<LoginResponse>>();

                if (result != null && !string.IsNullOrWhiteSpace(result.Data?.Token))
                {
                    HttpContext.Session.SetString("JWT", result.Data.Token);
                    HttpContext.Session.SetString("Username", result.Data.username);
                    HttpContext.Session.SetInt32("UserId", result.Data.userId);
                    HttpContext.Session.SetString("Role", result.Data.role);
                    HttpContext.Session.SetString("Firstname", result.Data.firstname);
                    HttpContext.Session.SetString("Lastname", result.Data.lastname);
                    HttpContext.Session.SetString("Email", result.Data.email);

                    return RedirectToAction("Index", "Home");
                }

                TempData["ErrorMessage"] = result?.Message ?? "Login failed.";
                return RedirectToAction("Login");
            }

            var errorResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<object>>();
            TempData["ErrorMessage"] = errorResponse?.Message ?? "Invalid login.";
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

    }
}
