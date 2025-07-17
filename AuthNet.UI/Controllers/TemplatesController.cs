using AuthNet.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace AuthNet.UI.Controllers
{
    public class TemplatesController : Controller
    {
        private readonly HttpClient _httpClient;

        public TemplatesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }
        public async Task<IActionResult> Index()
        {
            var templateResponse = await _httpClient.GetAsync("/api/InvoiceTemplates");

            var templateList = new List<UiTemplateVm>();

            if (templateResponse.IsSuccessStatusCode)
            {
                var templateContent = await templateResponse.Content.ReadAsStringAsync();
                var templates = JsonConvert.DeserializeObject<List<InvoiceTemplateDto>>(templateContent);

                templateList = templates.Select(t => new UiTemplateVm
                {
                    Id = t.Id,
                    Layout = t.Layout,
                    Notes = t.Notes,
                    TermsAndConditions = t.TermsAndConditions
                }).ToList();
            }
                     
            return View(templateList);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddInvoiceTemplateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _httpClient.PostAsJsonAsync("api/InvoiceTemplates", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Invoice template added successfully.";
                return RedirectToAction("Index");
            }

            var content = await response.Content.ReadAsStringAsync();
            var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            var errorMsg = errorObj.ContainsKey("message") ? errorObj["message"] : "Failed to add invoice template.";

            ModelState.AddModelError("", errorMsg);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var template = await _httpClient.GetFromJsonAsync<InvoiceTemplateDto>($"api/InvoiceTemplates/{id}");
            if (template == null) return NotFound();

            var model = new EditInvoiceTemplateViewModel
            {
                Id = template.Id,
                Layout = template.Layout,
                Notes = template.Notes,
                TermsAndConditions = template.TermsAndConditions
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditInvoiceTemplateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _httpClient.PutAsJsonAsync($"api/InvoiceTemplates/{model.Id}", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Invoice template updated successfully.";
                return RedirectToAction("Index");
            }

            var content = await response.Content.ReadAsStringAsync();
            var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            var errorMsg = errorObj.ContainsKey("message") ? errorObj["message"] : "Failed to update invoice template.";

            ModelState.AddModelError("", errorMsg);
            return View(model);
        }

        [HttpPost("InvoiceTemplate/Delete/{id}")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            var httpResponseMessage = await _httpClient.DeleteAsync($"api/InvoiceTemplates/{id}");

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                var errorMsg = errorObj.ContainsKey("message") ? errorObj["message"] : "Failed to delete invoice template.";

                TempData["ErrorMessage"] = errorMsg;
                return RedirectToAction("Index", "Settings");
            }

            TempData["SuccessMessage"] = "Invoice template deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
