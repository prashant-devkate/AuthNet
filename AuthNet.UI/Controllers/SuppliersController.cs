using AuthNet.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AuthNet.UI.Controllers
{
    public class SuppliersController : Controller
    {
        private readonly HttpClient _httpClient;

        public SuppliersController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var suppliers = await _httpClient.GetFromJsonAsync<List<SupplierDto>>("api/Suppliers");

            if (suppliers == null)
            {
                suppliers = new List<SupplierDto>();
            }

            return View(suppliers ?? new List<SupplierDto>());
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddSupplierViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _httpClient.PostAsJsonAsync("api/Suppliers", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Supplier added successfully.";
                return RedirectToAction("Index");
            }

            var content = await response.Content.ReadAsStringAsync();
            var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            var errorMsg = errorObj.ContainsKey("message") ? errorObj["message"] : "Failed to add supplier.";

            ModelState.AddModelError("", errorMsg);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await _httpClient.GetFromJsonAsync<SupplierDto>($"api/Suppliers/{id}");
            if (supplier == null) return NotFound();

            var model = new EditSupplierViewModel
            {
                SupplierId = supplier.SupplierId,
                CompanyName = supplier.CompanyName,
                ContactName = supplier.ContactName,
                Phone = supplier.Phone,
                Address = supplier.Address
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditSupplierViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _httpClient.PutAsJsonAsync($"api/Suppliers/{model.SupplierId}", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Supplier updated successfully.";
                return RedirectToAction("Index");
            }

            var content = await response.Content.ReadAsStringAsync();
            var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            var errorMsg = errorObj.ContainsKey("message") ? errorObj["message"] : "Failed to update supplier.";

            ModelState.AddModelError("", errorMsg);
            return View(model);
        }

        [HttpPost("Supplier/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var httpResponseMessage = await _httpClient.DeleteAsync($"api/Suppliers/{id}");

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                var errorMsg = errorObj.ContainsKey("message") ? errorObj["message"] : "Failed to delete supplier.";

                TempData["ErrorMessage"] = errorMsg;
                return RedirectToAction("Index", "Suppliers");
            }

            TempData["SuccessMessage"] = "Supplier deleted successfully.";
            return RedirectToAction("Index", "Suppliers");
        }
    }
}
