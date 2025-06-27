using AuthNet.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;

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

            if (inventories == null)
            {
                inventories = new List<InventoryDto>();
            }

            return View(inventories);
        }
    }
}
