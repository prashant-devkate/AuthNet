﻿using AuthNet.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace AuthNet.UI.Controllers
{
    public class OrdersController : Controller
    {
        private readonly HttpClient _httpClient;

        public OrdersController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var orders = await _httpClient.GetFromJsonAsync<List<OrderDto>>("api/Orders");
            var user = await _httpClient.GetFromJsonAsync<List<UserDto>>("api/Users");
            var customer = await _httpClient.GetFromJsonAsync<List<CustomerDto>>("api/Customers");

            foreach (var order in orders)
            {
                order.Username = user.FirstOrDefault(p => p.UserId == order.UserId)?.Username ?? "Unknown";
                order.Customername = customer.FirstOrDefault(c => c.CustomerId == order.CustomerId)?.Name ?? "Unknown";
            }


            if (orders == null)
            {
                orders = new List<OrderDto>();
            }

            return View(orders ?? new List<OrderDto>());
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new AddOrderViewModel();

            await LoadLookupsAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddOrderViewModel model)
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

            var response = await _httpClient.PostAsJsonAsync("api/Orders", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Order added successfully.";
                return RedirectToAction("Index");
            }

            var content = await response.Content.ReadAsStringAsync();
            var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            var errorMsg = errorObj.ContainsKey("message") ? errorObj["message"] : "Failed to add order.";

            ModelState.AddModelError("", errorMsg);
            await LoadLookupsAsync();
            return View(model);
        }

        private async Task LoadLookupsAsync()
        {
            var customers = await _httpClient.GetFromJsonAsync<List<CustomerViewModel>>("api/Customers");
            ViewBag.Customers = new SelectList(customers, "CustomerId", "Name");

            var users = await _httpClient.GetFromJsonAsync<List<UserViewModel>>("api/Users");
            ViewBag.Users = new SelectList(users, "UserId", "Username");

            var productResponse = await _httpClient.GetFromJsonAsync<List<ProductDto>>("api/Products");
            ViewBag.Products = new SelectList(productResponse, "ProductId", "Name");
            ViewBag.ProductPrices = productResponse.ToDictionary(p => p.ProductId, p => p.Price);


        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _httpClient.GetFromJsonAsync<OrderDto>($"api/Orders/{id}");
            if (order == null) return NotFound();

            var model = new EditOrderViewModel
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                CustomerId = order.CustomerId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                OrderItems = order.OrderItems.Select(oi => new AddOrderItemViewModel
                {
                    OrderItemId = oi.OrderItemId,
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };

            await LoadLookupsAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditOrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadLookupsAsync();

                return View(model);
            }

            if (model.OrderId <= 0)
            {
                ModelState.AddModelError("", "Invalid Order ID.");

                await LoadLookupsAsync();

                return View(model);
            }

            model.TotalAmount = model.OrderItems.Sum(i => i.Quantity * i.UnitPrice);

            var updatePayload = new
            {
                orderId = model.OrderId,
                customerId = model.CustomerId,
                userId = model.UserId,
                orderDate = model.OrderDate,
                totalAmount = model.TotalAmount,
                orderItems = model.OrderItems.Select(i => new
                {
                    orderItemId = i.OrderItemId,
                    productId = i.ProductId,
                    quantity = i.Quantity,
                    unitPrice = i.UnitPrice
                }).ToList()
            };

            var response = await _httpClient.PutAsJsonAsync($"api/Orders/{model.OrderId}", updatePayload);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Order updated successfully.";
                return RedirectToAction("Index");
            }

            var content = await response.Content.ReadAsStringAsync();
            var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            var errorMsg = errorObj != null && errorObj.ContainsKey("message") ? errorObj["message"] : "Failed to update order.";

            ModelState.AddModelError("", errorMsg);

            await LoadLookupsAsync();

            return View(model);
        }


        [HttpPost("Order/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var httpResponseMessage = await _httpClient.DeleteAsync($"api/Orders/{id}");

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                var errorMsg = errorObj.ContainsKey("message") ? errorObj["message"] : "Failed to delete order.";

                TempData["ErrorMessage"] = errorMsg;
                return RedirectToAction("Index", "Orders");
            }

            TempData["SuccessMessage"] = "Order deleted successfully.";
            return RedirectToAction("Index");
        }
    }
}
