using AuthNet.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace AuthNet.UI.Controllers
{
    public class PurchaseOrdersController : Controller
    {
        private readonly HttpClient _httpClient;

        public PurchaseOrdersController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var purchaseOrders = await _httpClient.GetFromJsonAsync<List<PurchaseOrderDto>>("api/Purchase");
            var user = await _httpClient.GetFromJsonAsync<List<UserDto>>("api/Users");
            var supplier = await _httpClient.GetFromJsonAsync<List<SupplierDto>>("api/Suppliers");

            foreach (var order in purchaseOrders)
            {
                order.Username = user.FirstOrDefault(p => p.UserId == order.CreatedByUserId)?.Username ?? "Unknown";
                order.Suppliername = supplier.FirstOrDefault(c => c.SupplierId == order.SupplierId)?.CompanyName ?? "Unknown";
            }


            if (purchaseOrders == null)
            {
                purchaseOrders = new List<PurchaseOrderDto>();
            }

            return View(purchaseOrders ?? new List<PurchaseOrderDto>());
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new AddPurchaseOrderViewModel();

            await LoadLookupsAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddPurchaseOrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Re-populate dropdowns
                await LoadLookupsAsync();

                return View(model);
            }

            if (model.OrderItems != null && model.OrderItems.Any())
            {
                model.TotalAmount = model.OrderItems.Sum(item => item.Quantity * item.UnitPrice);
            }

            var response = await _httpClient.PostAsJsonAsync("api/Purchase", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Purchase order added successfully.";
                return RedirectToAction("Index");
            }

            var content = await response.Content.ReadAsStringAsync();
            var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            var errorMsg = errorObj.ContainsKey("message") ? errorObj["message"] : "Failed to add purchase order.";

            ModelState.AddModelError("", errorMsg);
            await LoadLookupsAsync();
            return View(model);
        }

        private async Task LoadLookupsAsync()
        {
            var suppliers = await _httpClient.GetFromJsonAsync<List<SupplierViewModel>>("api/Suppliers");
            ViewBag.Suppliers = new SelectList(suppliers, "SupplierId", "CompanyName");

            var users = await _httpClient.GetFromJsonAsync<List<UserViewModel>>("api/Users");
            ViewBag.Users = new SelectList(users, "UserId", "Username");

            var productResponse = await _httpClient.GetFromJsonAsync<List<ProductDto>>("api/Products");
            ViewBag.Products = new SelectList(productResponse, "ProductId", "Name");
            ViewBag.ProductPrices = productResponse.ToDictionary(p => p.ProductId, p => p.Price);


        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _httpClient.GetFromJsonAsync<PurchaseOrderDto>($"api/Purchase/{id}");
            if (order == null) return NotFound();

            var model = new EditPurchaseOrderViewModel
            {
                PurchaseOrderId = order.PurchaseOrderId,
                CreatedByUserId = order.CreatedByUserId,
                SupplierId = order.SupplierId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                OrderItems = order.OrderItems.Select(oi => new AddPurchaseOrderItemViewModel
                {
                    PurchaseOrderItemId = oi.OrderItemId,
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };

            await LoadLookupsAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditPurchaseOrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadLookupsAsync();

                return View(model);
            }

            if (model.PurchaseOrderId <= 0)
            {
                ModelState.AddModelError("", "Invalid Order ID.");

                await LoadLookupsAsync();

                return View(model);
            }

            model.TotalAmount = model.OrderItems.Sum(i => i.Quantity * i.UnitPrice);

            var updatePayload = new
            {
                orderId = model.PurchaseOrderId,
                SupplierId = model.SupplierId,
                CreatedByUserId = model.CreatedByUserId,
                orderDate = model.OrderDate,
                totalAmount = model.TotalAmount,
                orderItems = model.OrderItems.Select(i => new
                {
                    PurchaseOrderItemId = i.PurchaseOrderItemId,
                    productId = i.ProductId,
                    quantity = i.Quantity,
                    unitPrice = i.UnitPrice
                }).ToList()
            };

            var response = await _httpClient.PutAsJsonAsync($"api/Purchase/{model.PurchaseOrderId}", updatePayload);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Purchase order updated successfully.";
                return RedirectToAction("Index");
            }

            var content = await response.Content.ReadAsStringAsync();
            var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            var errorMsg = errorObj != null && errorObj.ContainsKey("message") ? errorObj["message"] : "Failed to update purchase order.";

            ModelState.AddModelError("", errorMsg);

            await LoadLookupsAsync();

            return View(model);
        }


        [HttpPost("PurchaseOrder/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var httpResponseMessage = await _httpClient.DeleteAsync($"api/Purchase/{id}");

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                var errorMsg = errorObj.ContainsKey("message") ? errorObj["message"] : "Failed to delete purchase order.";

                TempData["ErrorMessage"] = errorMsg;
                return RedirectToAction("Index", "PurchaseOrders");
            }

            TempData["SuccessMessage"] = "Purchase order deleted successfully.";
            return RedirectToAction("Index", "PurchaseOrders");
        }
    }
}
