using AuthNet.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AuthNet.UI.Controllers
{
    public class ForecastController : Controller
    {
        private readonly HttpClient _httpClient;

        public ForecastController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }
        public async Task<IActionResult> Index(int productId)
        {
            await LoadLookupsAsync();

            var res = await _httpClient.GetFromJsonAsync<List<ForecastDto>>($"api/Forecast/forecast/{productId}");
            var history = await _httpClient.GetFromJsonAsync<HistoryResponseDto>($"api/Forecast/{productId}");

            var vm = new ForecastViewModel
            {
                ProductId = productId,
                Forecast = res ?? new List<ForecastDto>(),
                History = history ?? new HistoryResponseDto()
            };

            return View(vm);
        }

        private async Task LoadLookupsAsync()
        {
            var productResponse = await _httpClient.GetFromJsonAsync<List<ProductDto>>("api/Products");
            ViewBag.Products = new SelectList(productResponse, "ProductId", "Name");
        }

    }
}
