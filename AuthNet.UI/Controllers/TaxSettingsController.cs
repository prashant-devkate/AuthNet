using AuthNet.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AuthNet.UI.Controllers
{
    public class TaxSettingsController : Controller
    {
        private readonly HttpClient _httpClient;

        public TaxSettingsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }
        public async Task<IActionResult> Index()
        {
            var taxResponse = await _httpClient.GetAsync("/api/TaxSettings");

            TaxSettingViewModel? tax = null;


            if (taxResponse.IsSuccessStatusCode)
            {
                var taxContent = await taxResponse.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(taxContent) && taxContent != "NaN")
                {
                    tax = JsonConvert.DeserializeObject<TaxSettingViewModel>(taxContent);
                }
            }
            return View(tax);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddTaxSettingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _httpClient.PostAsJsonAsync("api/TaxSettings", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Tax settings added successfully.";
                return RedirectToAction("Index");
            }

            var content = await response.Content.ReadAsStringAsync();
            try
            {

                var errorObj = JsonConvert.DeserializeObject<OperationResponse>(content);
                TempData["ErrorMessage"] = $"Failed to save tax settings. {errorObj?.Message}";
            }
            catch
            {
                TempData["ErrorMessage"] = "Failed to save tax settings. Unknown error.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var tax = await _httpClient.GetFromJsonAsync<TaxSettingDto>("api/TaxSettings");
            if (tax == null) return NotFound();

            var model = new EditTaxSettingViewModel
            {
                Id = tax.Id,
                CGST = tax.CGST,
                SGST = tax.SGST,
                IGST = tax.IGST
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditTaxSettingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _httpClient.PutAsJsonAsync($"api/TaxSettings/{model.Id}", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Tax settings updated successfully.";
                return RedirectToAction("Index");
            }

            var content = await response.Content.ReadAsStringAsync();
            var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            var errorMsg = errorObj.ContainsKey("message") ? errorObj["message"] : "Failed to update tax setings.";

            ModelState.AddModelError("", errorMsg);
            return RedirectToAction("Index");
        }
    }
}
