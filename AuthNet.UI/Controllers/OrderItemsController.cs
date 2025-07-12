using AuthNet.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace AuthNet.UI.Controllers
{
    public class OrderItemsController : Controller
    {
        private readonly HttpClient _httpClient;

        public OrderItemsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }
        public async Task<IActionResult> Index()
        {

            var orderItems = await _httpClient.GetFromJsonAsync<List<PurchaseOrderItemViewModel>>("api/OrderItems");

            if (orderItems == null)
            {
                orderItems = new List<PurchaseOrderItemViewModel>();
            }

            return View(orderItems);
        }
        public async Task<IActionResult> GroupedItems()
        {
            var response = await _httpClient.GetFromJsonAsync<List<OrderWithItemsViewModel>>("api/OrderItems/grouped");

            if (response == null)
            {
                response = new List<OrderWithItemsViewModel>();
            }
            return View(response);
        }

    }
}
