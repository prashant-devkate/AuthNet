using AuthNet.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AuthNet.UI.Controllers
{
    public class SettingsController : Controller
    {
        private readonly HttpClient _httpClient;

        public SettingsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index()
        {
            var companyResponse = await _httpClient.GetAsync("/api/Settings/Get");
            var templateResponse = await _httpClient.GetAsync("/api/InvoiceTemplates");
            var taxResponse = await _httpClient.GetAsync("/api/TaxSettings");

            CompanyInfoViewModel? company = null;
            TaxSettingViewModel? tax = null;
            var templateList = new List<UiTemplateVm>();

            if (companyResponse.IsSuccessStatusCode)
            {
                var companyContent = await companyResponse.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(companyContent) && companyContent != "NaN")
                {
                    company = JsonConvert.DeserializeObject<CompanyInfoViewModel>(companyContent);
                }
            }

            if (taxResponse.IsSuccessStatusCode)
            {
                var taxContent = await taxResponse.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(taxContent) && taxContent != "NaN")
                {
                    tax = JsonConvert.DeserializeObject<TaxSettingViewModel>(taxContent);
                }
            }

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

            ViewBag.CompanyLogo = company?.logoUrl ?? "/default-logo.png";
            ViewBag.UserImage = $"https://ui-avatars.com/api/?name={HttpContext.Session.GetString("Username")}&background=0D8ABC&color=fff";

            var settingsVm = new SettingsViewModel
            {
                Company = company,
                Template = new TemplateViewModel
                {
                    InvoiceTemplates = templateList
                },
                TaxSetting = tax
            };

            return View(settingsVm);
        }



        public async Task<IActionResult> History()
        {
            var response = await _httpClient.GetAsync("/api/Settings/Get-History");
            if (!response.IsSuccessStatusCode)
                return View(Enumerable.Empty<CompanyHistoryInfoViewModel>());

            var content = await response.Content.ReadAsStringAsync();
            var companyHistoryList = JsonConvert.DeserializeObject<List<CompanyHistoryInfoViewModel>>(content);

            return View(companyHistoryList);
        }


        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var response = await _httpClient.GetAsync("/api/Settings/Get");

            if (!response.IsSuccessStatusCode)
                return View(new EditCompanyInfoViewModel());

            var content = await response.Content.ReadAsStringAsync();
            var dto = JsonConvert.DeserializeObject<CompanyInfoDto>(content);

            var model = new EditCompanyInfoViewModel
            {
                Name = dto.Name,
                Address = dto.Address,
                GSTIN = dto.GSTIN,
                LogoFilePath = dto.LogoFilePath,
                SignFilePath = dto.SignFilePath
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditCompanyInfoViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(model.Id.ToString()), nameof(model.Id));
            content.Add(new StringContent(model.Name ?? ""), nameof(model.Name));
            content.Add(new StringContent(model.Address ?? ""), nameof(model.Address));
            content.Add(new StringContent(model.GSTIN ?? ""), nameof(model.GSTIN));

            if (model.LogoUrl != null)
            {
                var streamContent = new StreamContent(model.LogoUrl.OpenReadStream());
                content.Add(streamContent, nameof(model.LogoUrl), model.LogoUrl.FileName);
            }

            if (model.SignatureUrl != null)
            {
                var streamContent = new StreamContent(model.SignatureUrl.OpenReadStream());
                content.Add(streamContent, nameof(model.SignatureUrl), model.SignatureUrl.FileName);
            }

            var response = await _httpClient.PutAsync("/api/Settings/Update", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Company info updated successfully.";
                return RedirectToAction("Index");
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            try
            {
                var errorObj = JsonConvert.DeserializeObject<OperationResponse>(responseContent);
                TempData["ErrorMessage"] = $"Failed to update company info. {errorObj?.Message}";
            }
            catch
            {
                TempData["ErrorMessage"] = "Failed to update company info. Unknown error.";
            }

            return RedirectToAction("Index");
        }



        [HttpGet]
        public IActionResult Add()
        {
            return View(new AddCompanyInfoViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddCompanyInfoViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(model.Name ?? ""), nameof(model.Name));
            content.Add(new StringContent(model.Address ?? ""), nameof(model.Address));
            content.Add(new StringContent(model.GSTIN ?? ""), nameof(model.GSTIN));

            if (model.LogoUrl != null)
            {
                var streamContent = new StreamContent(model.LogoUrl.OpenReadStream());
                content.Add(streamContent, nameof(model.LogoUrl), model.LogoUrl.FileName);
            }

            if (model.SignatureUrl != null)
            {
                var streamContent = new StreamContent(model.SignatureUrl.OpenReadStream());
                content.Add(streamContent, nameof(model.SignatureUrl), model.SignatureUrl.FileName);
            }

            var response = await _httpClient.PostAsync("/api/Settings/Save", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Company info saved successfully.";
                return RedirectToAction("Index");
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            try
            {
                var errorObj = JsonConvert.DeserializeObject<OperationResponse>(responseContent);
                TempData["ErrorMessage"] = $"Failed to save company info. {errorObj?.Message}";
            }
            catch
            {
                TempData["ErrorMessage"] = "Failed to save company info. Unknown error.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AddTemplate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddTemplate(AddInvoiceTemplateViewModel model)
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
        public async Task<IActionResult> EditTemplate(int id)
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
        public async Task<IActionResult> EditTemplate(EditInvoiceTemplateViewModel model)
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

        [HttpGet]
        public IActionResult AddTaxSetting()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddTaxSetting(AddTaxSettingViewModel model)
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
        public async Task<IActionResult> EditTaxSetting(int id)
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
        public async Task<IActionResult> EditTaxSetting(EditTaxSettingViewModel model)
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
            return View(model);
        }
    }
}