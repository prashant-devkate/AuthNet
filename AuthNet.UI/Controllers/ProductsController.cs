using AuthNet.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace AuthNet.UI.Controllers
{
    public class ProductsController : Controller
    {
        private readonly HttpClient _httpClient;

        public ProductsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 5;
            List<ProductDto> products = new();
            int totalProducts = 0;

            try
            {
                var response = await _httpClient.GetAsync("api/Products");
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();
                if (data != null)
                {
                    products.AddRange(data);
                    totalProducts = products.Count;
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Failed to load products.";
            }

            // Apply pagination
            var paginated = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            var viewModel = new ProductListViewModel
            {
                Products = paginated,
                CurrentPage = page,
                TotalPages = totalPages
            };

            if (TempData.ContainsKey("SuccessMessage"))
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            if (TempData.ContainsKey("ErrorMessage"))
                ViewBag.ErrorMessage = TempData["ErrorMessage"];

            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new AddProductViewModel();

            var categoriesResponse = await _httpClient.GetFromJsonAsync<List<CategoryViewModel>>("api/Categories");
            ViewBag.Categories = new SelectList(categoriesResponse, "CategoryId", "Name");

            var suppliersResponse = await _httpClient.GetFromJsonAsync<List<SupplierViewModel>>("api/Suppliers");
            ViewBag.Suppliers = new SelectList(suppliersResponse, "SupplierId", "CompanyName");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Re-populate dropdowns before returning view
                var categories = await _httpClient.GetFromJsonAsync<List<CategoryViewModel>>("api/categories");
                ViewBag.Categories = new SelectList(categories, "CategoryId", "Name");

                var suppliers = await _httpClient.GetFromJsonAsync<List<SupplierViewModel>>("api/suppliers");
                ViewBag.Suppliers = new SelectList(suppliers, "SupplierId", "Name");

                return View(model);
            }

            var response = await _httpClient.PostAsJsonAsync("api/products", model);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Product added successfully.";
                return RedirectToAction("Index");
            }

            var content = await response.Content.ReadAsStringAsync();
            var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            var errorMsg = errorObj.ContainsKey("message") ? errorObj["message"] : "Failed to add product.";

            ModelState.AddModelError("", errorMsg);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _httpClient.GetFromJsonAsync<ProductDto>($"api/Products/{id}");
            if (product == null) return NotFound();

            var model = new EditProductViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ProductCode = product.ProductCode,
                CategoryId = product.CategoryId,
                SupplierId = product.SupplierId,
                //Inventory = new InventoryData
                //{
                //    QuantityInStock = product.Inventory?.QuantityInStock ?? 0,
                //    ReorderLevel = product.Inventory?.ReorderLevel ?? 0,
                //    LastUpdated = product.Inventory?.LastUpdated ?? DateTime.UtcNow
                //}
            };

            var categories = await _httpClient.GetFromJsonAsync<List<CategoryViewModel>>("api/Categories");
            ViewBag.Categories = new SelectList(categories, "CategoryId", "Name");

            var suppliers = await _httpClient.GetFromJsonAsync<List<SupplierViewModel>>("api/suppliers");
            ViewBag.Suppliers = new SelectList(suppliers, "SupplierId", "Name");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _httpClient.GetFromJsonAsync<List<CategoryViewModel>>("api/Categories");
                ViewBag.Categories = new SelectList(categories, "CategoryId", "Name");

                var suppliers = await _httpClient.GetFromJsonAsync<List<SupplierViewModel>>("api/suppliers");
                ViewBag.Suppliers = new SelectList(suppliers, "SupplierId", "Name");

                return View(model);
            }

            var response = await _httpClient.PutAsJsonAsync($"api/Products/{model.ProductId}", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Product updated successfully.";
                return RedirectToAction("Index");
            }

            var content = await response.Content.ReadAsStringAsync();
            var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            var errorMsg = errorObj.ContainsKey("message") ? errorObj["message"] : "Failed to update product.";

            ModelState.AddModelError("", errorMsg);
            return View(model);
           
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var httpResponseMessage = await _httpClient.DeleteAsync($"api/Products/{id}");

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                var errorMsg = errorObj.ContainsKey("message") ? errorObj["message"] : "Failed to delete product.";

                TempData["ErrorMessage"] = errorMsg;
                return RedirectToAction("Index", "Products");
            }

            TempData["SuccessMessage"] = "Product deleted successfully.";
            return RedirectToAction("Index");
            
        }
    }
}
