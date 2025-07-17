using AuthNet.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace AuthNet.UI.Controllers
{
    public class CompanyController : Controller
    {
        private readonly HttpClient _httpClient;

        public CompanyController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index()
        {
            var companyResponse = await _httpClient.GetAsync("/api/Settings/Get");

            CompanyInfoViewModel? company = null;

            if (companyResponse.IsSuccessStatusCode)
            {
                var companyContent = await companyResponse.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(companyContent) && companyContent != "NaN")
                {
                    company = JsonConvert.DeserializeObject<CompanyInfoViewModel>(companyContent);
                }
            }

            ViewBag.CompanyLogo = company?.logoUrl ?? "/default-logo.png";
            ViewBag.UserImage = $"https://ui-avatars.com/api/?name={HttpContext.Session.GetString("Username")}&background=0D8ABC&color=fff";

            return View(company);
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
                ContactNumber = dto.ContactNumber,
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
            content.Add(new StringContent(model.ContactNumber ?? ""), nameof(model.ContactNumber));

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
            content.Add(new StringContent(model.ContactNumber ?? ""), nameof(model.ContactNumber));

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

        public async Task<IActionResult> History()
        {
            var response = await _httpClient.GetAsync("/api/Settings/Get-History");
            if (!response.IsSuccessStatusCode)
                return View(Enumerable.Empty<CompanyHistoryInfoViewModel>());

            var content = await response.Content.ReadAsStringAsync();
            var companyHistoryList = JsonConvert.DeserializeObject<List<CompanyHistoryInfoViewModel>>(content);

            return View(companyHistoryList);
        }
    }
}
