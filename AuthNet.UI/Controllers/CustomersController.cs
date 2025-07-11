using AuthNet.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AuthNet.UI.Controllers
{
    public class CustomersController : Controller
    {
        private readonly HttpClient _httpClient;

        public CustomersController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }
        public async Task<IActionResult> Index()
        {
            var customers = await _httpClient.GetFromJsonAsync<List<CustomerDto>>("api/Customers");

            if (customers == null)
            {
                customers = new List<CustomerDto>();
            }

            return View(customers ?? new List<CustomerDto>());
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddCustomerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _httpClient.PostAsJsonAsync("api/Customers", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Customer added successfully.";
                return RedirectToAction("Index");
            }

            var content = await response.Content.ReadAsStringAsync();
            var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            var errorMsg = errorObj.ContainsKey("message") ? errorObj["message"] : "Failed to add customer.";

            ModelState.AddModelError("", errorMsg);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _httpClient.GetFromJsonAsync<CustomerDto>($"api/Customers/{id}");
            if (customer == null) return NotFound();

            var model = new EditCustomerViewModel
            {
                CustomerId = customer.CustomerId,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditCustomerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _httpClient.PutAsJsonAsync($"api/Customers/{model.CustomerId}", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Customer updated successfully.";
                return RedirectToAction("Index");
            }

            var content = await response.Content.ReadAsStringAsync();
            var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            var errorMsg = errorObj.ContainsKey("message") ? errorObj["message"] : "Failed to update customer.";

            ModelState.AddModelError("", errorMsg);
            return View(model);

        }

        [HttpPost("Customer/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var httpResponseMessage = await _httpClient.DeleteAsync($"api/Customers/{id}");

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                var errorMsg = errorObj.ContainsKey("message") ? errorObj["message"] : "Failed to delete customer.";

                TempData["ErrorMessage"] = errorMsg;
                return RedirectToAction("Index", "Customers");
            }

            TempData["SuccessMessage"] = "Customer deleted successfully.";
            return RedirectToAction("Index", "Customers");

        }
    }
}
