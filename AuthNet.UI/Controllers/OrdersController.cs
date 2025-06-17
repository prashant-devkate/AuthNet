using AuthNet.UI.Models.DTO;
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

            if (orders == null)
            {
                orders = new List<OrderDto>();
            }

            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new AddOrderViewModel();

            var customersResponse = await _httpClient.GetFromJsonAsync<List<CustomerViewModel>>("api/Customers");
            ViewBag.Customers = new SelectList(customersResponse, "CustomerId", "Name");

            var usersResponse = await _httpClient.GetFromJsonAsync<List<UserViewModel>>("api/Users");
            ViewBag.Users = new SelectList(usersResponse, "UserId", "Username");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddOrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Re-populate dropdowns before returning view
                var customers = await _httpClient.GetFromJsonAsync<List<CustomerViewModel>>("api/Customers");
                ViewBag.Customers = new SelectList(customers, "CustomerId", "Name");

                var users = await _httpClient.GetFromJsonAsync<List<UserViewModel>>("api/Users");
                ViewBag.Users = new SelectList(users, "UserId", "Username");

                return View(model);
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
            return View(model);
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
                TotalAmount = order.TotalAmount
            };

            var customersResponse = await _httpClient.GetFromJsonAsync<List<CustomerViewModel>>("api/Customers");
            ViewBag.Customers = new SelectList(customersResponse, "CustomerId", "Name");

            var usersResponse = await _httpClient.GetFromJsonAsync<List<UserViewModel>>("api/Users");
            ViewBag.Users = new SelectList(usersResponse, "UserId", "Username");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditOrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var customersResponse = await _httpClient.GetFromJsonAsync<List<CustomerViewModel>>("api/Customers");
                ViewBag.Customers = new SelectList(customersResponse, "CustomerId", "Name");

                var usersResponse = await _httpClient.GetFromJsonAsync<List<UserViewModel>>("api/Users");
                ViewBag.Users = new SelectList(usersResponse, "UserId", "Username");

                return View(model);
            }

            if (model.OrderId <= 0)
            {
                ModelState.AddModelError("", "Invalid Order ID.");

                var customersResponse = await _httpClient.GetFromJsonAsync<List<CustomerViewModel>>("api/Customers");
                ViewBag.Customers = new SelectList(customersResponse, "CustomerId", "Name");

                var usersResponse = await _httpClient.GetFromJsonAsync<List<UserViewModel>>("api/Users");
                ViewBag.Users = new SelectList(usersResponse, "UserId", "Username");

                return View(model);
            }

            var updatePayload = new
            {
                orderId = model.OrderId,
                customerId = model.CustomerId,
                userId = model.UserId,
                orderDate = model.OrderDate,
                totalAmount = model.TotalAmount
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

            var customers = await _httpClient.GetFromJsonAsync<List<CustomerViewModel>>("api/Customers");
            ViewBag.Customers = new SelectList(customers, "CustomerId", "Name");

            var users = await _httpClient.GetFromJsonAsync<List<UserViewModel>>("api/Users");
            ViewBag.Users = new SelectList(users, "UserId", "Username");

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
