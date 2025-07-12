using AuthNet.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace AuthNet.UI.Controllers
{
    public class PurchaseItemsController : Controller
    {
        private readonly HttpClient _httpClient;

        public PurchaseItemsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }
        public async Task<IActionResult> Index()
        {

            var purchaseOrderItems = await _httpClient.GetFromJsonAsync<List<OrderItemViewModel>>("api/PurchaseOrderItems");

            if (purchaseOrderItems == null)
            {
                purchaseOrderItems = new List<OrderItemViewModel>();
            }

            return View(purchaseOrderItems);
        }
        public async Task<IActionResult> GroupedItems()
        {
            var response = await _httpClient.GetFromJsonAsync<List<PurchaseOrderWithItemsViewModel>>("api/PurchaseOrderItems/grouped");

            if (response == null)
            {
                response = new List<PurchaseOrderWithItemsViewModel>();
            }
            return View(response);
        }

    }
}
