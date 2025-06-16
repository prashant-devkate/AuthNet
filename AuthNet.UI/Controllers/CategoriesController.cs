using AuthNet.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http;

namespace AuthNet.UI.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly HttpClient _httpClient;

        public CategoriesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _httpClient.GetFromJsonAsync<List<CategoryDto>>("api/Categories");

            if (categories == null)  
            {
                categories = new List<CategoryDto>();
            }

            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); 
            }

            var response = await _httpClient.PostAsJsonAsync("api/Categories", model);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Failed to save category.");
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _httpClient.GetFromJsonAsync<CategoryDto>($"api/Categories/{id}");
            if (category == null) return NotFound();

            var model = new EditCategoryViewModel
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditCategoryViewModel model)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Categories/{model.CategoryId}", model);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Failed to update category.");

            return View(model);

        }

        [HttpPost("Category/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var httpResponseMessage = await _httpClient.DeleteAsync($"api/Categories/{id}");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Categories");
            }

            return View("Edit");
        }
    }
}
