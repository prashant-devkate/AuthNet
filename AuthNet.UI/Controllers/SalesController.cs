using AuthNet.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
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
            ViewBag.ApiBaseUrl = _httpClient.BaseAddress?.ToString() ?? "https://localhost:7165/"; // Fallback
     
            var allProducts = await _httpClient.GetFromJsonAsync<List<ProductDto>>("api/Products")
                              ?? new List<ProductDto>();
            var allTemplates = await _httpClient.GetFromJsonAsync<List<InvoiceTemplateDto>>("api/InvoiceTemplates")
                              ?? new List<InvoiceTemplateDto>();

            ViewBag.Templates = allTemplates;

            var allCats = await _httpClient.GetFromJsonAsync<List<CategoryViewModel>>("api/categories")
                                ?? new List<CategoryViewModel>();
            ViewBag.UniqueCategories = allCats;

            var companyInfo = await _httpClient.GetFromJsonAsync<CompanyInfoDto>("api/Settings/Get");
            ViewBag.CompanyInfo = companyInfo;

            var taxSetting = await _httpClient.GetFromJsonAsync<TaxDto>("api/TaxSettings");
            ViewBag.TaxSetting = taxSetting;



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
                    Cats = allCats,
                    company = companyInfo,
                    tax = taxSetting,
                    CurrentPage = i + 1,
                    TotalPages = totalPages
                };

                viewModelList.Add(vm);
            }

            return View(viewModelList ?? new List<ProductListViewModel>());
        }

        public async Task<IActionResult> DailySalesReport(int page = 1)
        {
            int pageSize = 2;
            SalesReportDto fullReport = null;

            try
            {
                var response = await _httpClient.GetAsync("api/Sales/Daily");
                response.EnsureSuccessStatusCode();

                fullReport = await response.Content.ReadFromJsonAsync<SalesReportDto>();
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Failed to load daily sales.";
                return View(new SalesListViewModel());
            }

            if (fullReport == null || fullReport.Sales == null)
            {
                TempData["ErrorMessage"] = "No data received.";
                return View(new SalesListViewModel());
            }

            var totalSales = fullReport.Sales.Count;
            var paginatedSales = fullReport.Sales
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedReport = new SalesReportDto
            {
                Title = fullReport.Title,
                TotalSales = fullReport.TotalSales,
                InvoiceCount = fullReport.InvoiceCount,
                Sales = paginatedSales
            };

            var viewModel = new SalesListViewModel
            {
                Sales = new List<SalesReportDto> { pagedReport },
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalSales / (double)pageSize)
            };

            return View(viewModel);
        }

        public async Task<IActionResult> MonthlySalesReport(int page = 1)
        {
            int pageSize = 2;
            SalesReportDto fullReport = null;

            try
            {
                var response = await _httpClient.GetAsync("api/Sales/Monthly");
                response.EnsureSuccessStatusCode();

                fullReport = await response.Content.ReadFromJsonAsync<SalesReportDto>();
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Failed to load daily sales.";
                return View(new SalesListViewModel());
            }

            if (fullReport == null || fullReport.Sales == null)
            {
                TempData["ErrorMessage"] = "No data received.";
                return View(new SalesListViewModel());
            }

            var totalSales = fullReport.Sales.Count;
            var paginatedSales = fullReport.Sales
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedReport = new SalesReportDto
            {
                Title = fullReport.Title,
                TotalSales = fullReport.TotalSales,
                InvoiceCount = fullReport.InvoiceCount,
                Sales = paginatedSales
            };

            var viewModel = new SalesListViewModel
            {
                Sales = new List<SalesReportDto> { pagedReport },
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalSales / (double)pageSize)
            };

            return View(viewModel);
        }

        public async Task<IActionResult> YearlySalesReport(int page = 1)
        {
            int pageSize = 2;
            SalesReportDto fullReport = null;

            try
            {
                var response = await _httpClient.GetAsync("api/Sales/Yearly");
                response.EnsureSuccessStatusCode();

                fullReport = await response.Content.ReadFromJsonAsync<SalesReportDto>();
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Failed to load daily sales.";
                return View(new SalesListViewModel());
            }

            if (fullReport == null || fullReport.Sales == null)
            {
                TempData["ErrorMessage"] = "No data received.";
                return View(new SalesListViewModel());
            }

            var totalSales = fullReport.Sales.Count;
            var paginatedSales = fullReport.Sales
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedReport = new SalesReportDto
            {
                Title = fullReport.Title,
                TotalSales = fullReport.TotalSales,
                InvoiceCount = fullReport.InvoiceCount,
                Sales = paginatedSales
            };

            var viewModel = new SalesListViewModel
            {
                Sales = new List<SalesReportDto> { pagedReport },
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalSales / (double)pageSize)
            };

            return View(viewModel);
        }

    }
}
