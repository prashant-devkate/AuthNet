using AuthNet.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace AuthNet.UI.Controllers
{
    public class CustomersController : Controller
    {
        private readonly HttpClient _httpClient;

        public CustomersController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }
        public async Task<IActionResult> Index()
        {
            var customers = await _httpClient.GetFromJsonAsync<List<CustomerDto>>("api/Customers");

            if (customers == null)
            {
                customers = new List<CustomerDto>();
            }

            return View(customers);
        }
    }
}
