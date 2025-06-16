using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AuthNet.UI.Models;
using AuthNet.UI.Models.DTO;

namespace AuthNet.UI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly HttpClient _httpClient;

    public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("ApiClient");
    }

    public async Task<IActionResult> Index()
    {
        var model = new DashboardViewModel();

        try
        {
            model.TotalProducts = await GetCountFromApi("api/Products/count");
            model.TotalCategories = await GetCountFromApi("api/Categories/count");
            model.TotalSuppliers = await GetCountFromApi("api/Suppliers/count");
            model.TotalTasks = await GetCountFromApi("api/Tasks/count");
            model.Tasks = await _httpClient.GetFromJsonAsync<List<TaskItemDto>>("api/Tasks");
        }
        catch (Exception)
        {
            // Handle errors or fallback values
            model.TotalProducts = model.TotalCategories = model.TotalSuppliers = model.TotalTasks = 0;
        }

        return View("Index", model);
    }

    private async Task<int> GetCountFromApi(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);
        if (response.IsSuccessStatusCode)
        {
            var value = await response.Content.ReadAsStringAsync();
            return int.TryParse(value, out int count) ? count : 0;
        }
        return 0;
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
