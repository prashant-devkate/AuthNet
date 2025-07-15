using AuthNet.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace AuthNet.UI.Controllers
{
    public class InventoriesController : Controller
    {
        private readonly HttpClient _httpClient;

        public InventoriesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var inventories = await _httpClient.GetFromJsonAsync<List<InventoryDto>>("api/Inventory");
            var products = await _httpClient.GetFromJsonAsync<List<ProductDto>>("api/Products");

            foreach (var inventory in inventories)
            {
                inventory.ProductName = products.FirstOrDefault(p => p.ProductId == inventory.ProductId)?.Name ?? "Unknown";
            }

            if (inventories == null)
            {
                inventories = new List<InventoryDto>();
            }

            return View(inventories ?? new List<InventoryDto>());
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new AddInventoryViewModel();
            await AddLoadLookupsAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddInventoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await AddLoadLookupsAsync();
                return View(model);
            }

            var existingInventories = await _httpClient.GetFromJsonAsync<List<InventoryDto>>("/api/Inventory");

            if (existingInventories.Any(inv => inv.ProductId == model.ProductId))
            {
                ModelState.AddModelError("ProductId", "This product is already in the inventory.");
                  await AddLoadLookupsAsync();
                return View(model);
            }

            var payload = new
            {
                productId = model.ProductId,
                quantityInStock = model.QuantityInStock,
                reorderLevel = model.ReorderLevel,
                lastUpdated = DateTime.UtcNow
            };

            var response = await _httpClient.PostAsJsonAsync("api/Inventory", payload);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Inventory added successfully.";
                return RedirectToAction("Index");
            }

            var content = await response.Content.ReadAsStringAsync();
            var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            var errorMsg = errorObj.ContainsKey("message") ? errorObj["message"] : "Failed to add inventory.";

            ModelState.AddModelError("", errorMsg);
            await AddLoadLookupsAsync();
            return View(model);
        }

        private async Task AddLoadLookupsAsync()
        {
            var products = await _httpClient.GetFromJsonAsync<List<ProductDto>>("api/Products");
            var inventory = await _httpClient.GetFromJsonAsync<List<InventoryDto>>("api/Inventory");

            var availableProducts = products.Where(p => !inventory.Any(i => i.ProductId == p.ProductId)).ToList();

            if (availableProducts.Any())
            {
                ViewBag.Products = new SelectList(availableProducts, "ProductId", "Name");
            }
            else
            {
                ViewBag.NoProductsMessage = "No product available to add";
            }
        }

        private async Task LoadLookupsAsync()
        {
            var products = await _httpClient.GetFromJsonAsync<List<ProductDto>>("api/Products");
            ViewBag.Products = new SelectList(products, "ProductId", "Name");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var inventory = await _httpClient.GetFromJsonAsync<InventoryDto>($"api/Inventory/{id}");
            if (inventory == null) return NotFound();

            var model = new EditInventoryViewModel
            {
                InventoryId = inventory.InventoryId,
                ProductId = inventory.ProductId,
                QuantityInStock = inventory.QuantityInStock,
                ReorderLevel = inventory.ReorderLevel,
                LastUpdated = inventory.LastUpdated

            };

            await LoadLookupsAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditInventoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadLookupsAsync();
                return View(model);
            }

            var existingInventories = await _httpClient.GetFromJsonAsync<List<InventoryDto>>("/api/Inventory");

            if (existingInventories.Any(inv => inv.ProductId == model.ProductId && inv.InventoryId != model.InventoryId))
            {
                ModelState.AddModelError("ProductId", "This product is already in the inventory.");
                await LoadLookupsAsync();
                return View(model);
            }

            var response = await _httpClient.PutAsJsonAsync($"api/Inventory/{model.InventoryId}", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Inventory updated successfully.";
                return RedirectToAction("Index");
            }

            var content = await response.Content.ReadAsStringAsync();
            var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            var errorMsg = errorObj.ContainsKey("message") ? errorObj["message"] : "Failed to update inventory.";

            ModelState.AddModelError("", errorMsg);
            return View(model);

        }

        [HttpPost("Inventory/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var httpResponseMessage = await _httpClient.DeleteAsync($"api/Inventory/{id}");

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                var errorMsg = errorObj.ContainsKey("message") ? errorObj["message"] : "Failed to delete inventory.";

                TempData["ErrorMessage"] = errorMsg;
                return RedirectToAction("Index", "Inventories");
            }

            TempData["SuccessMessage"] = "Inventory deleted successfully.";
            return RedirectToAction("Index", "Inventories");

        }
    }
}
