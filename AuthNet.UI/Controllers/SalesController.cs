using AuthNet.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace AuthNet.UI.Controllers
{
    public class SalesController : Controller
    {
        private readonly HttpClient _httpClient;

        public SalesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }
        public async Task<IActionResult> Index()
        {
            var allProducts = await _httpClient.GetFromJsonAsync<List<ProductDto>>("api/Products")
                              ?? new List<ProductDto>();
            var allTemplates = await _httpClient.GetFromJsonAsync<List<InvoiceTemplateDto>>("api/InvoiceTemplates")
                              ?? new List<InvoiceTemplateDto>();

            ViewBag.Templates = allTemplates;

            int pageSize = 10;
            int totalPages = (int)Math.Ceiling(allProducts.Count / (double)pageSize);
            var viewModelList = new List<ProductListViewModel>();

            for (int i = 0; i < totalPages; i++)
            {
                var productsPage = allProducts.Skip(i * pageSize).Take(pageSize).ToList();

                var vm = new ProductListViewModel
                {
                    Products = productsPage,
                    Invoices = allTemplates,
                    CurrentPage = i + 1,
                    TotalPages = totalPages
                };

                viewModelList.Add(vm);
            }

            return View(viewModelList);
        }
    }
}
