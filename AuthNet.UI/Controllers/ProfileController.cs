using AuthNet.UI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace AuthNet.UI.Controllers
{
    public class ProfileController : Controller
    {
        private readonly HttpClient _httpClient;

        public ProfileController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public IActionResult Index()
        {
            var vm = new ProfileViewModel
            {
                Username = HttpContext.Session.GetString("Username"),
                Role = HttpContext.Session.GetString("Role")
            };
            return View(vm);

        }

        [HttpGet]
        public IActionResult UpdateProfile()
        {
            return View(new UpdateProfileDTO
            {
                UserId = HttpContext.Session.GetInt32("UserId") ?? 0,
                Username = HttpContext.Session.GetString("Username")
            });
        }


        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDTO dto)
        {
            dto.UserId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var response = await _httpClient.PostAsJsonAsync("api/Users/UpdateProfile", dto);
            var content = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            var status = result?.GetValueOrDefault("status");
            var message = result?.GetValueOrDefault("message") ?? "Profile update failed.";

            if (response.IsSuccessStatusCode && status == "Success")
            {
                TempData["SuccessMessage"] = message;
                HttpContext.Session.SetString("Username", dto.Username);
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordDTO { userId = HttpContext.Session.GetInt32("UserId") ?? 0 });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO dto)
        {
            dto.userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var response = await _httpClient.PostAsJsonAsync("api/Users/ChangePassword", dto);
            var content = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            var status = result?.GetValueOrDefault("status");
            var message = result?.GetValueOrDefault("message") ?? "Password change failed.";

            if (response.IsSuccessStatusCode && status == "Success")
                TempData["SuccessMessage"] = message;
            else
                TempData["ErrorMessage"] = message;

            return RedirectToAction("Index");
        }







    }
}
