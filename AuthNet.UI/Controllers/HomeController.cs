using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AuthNet.UI.Models;
using AuthNet.UI.Models.DTO;
using Newtonsoft.Json;

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
        ViewBag.ApiBaseUrl = _httpClient.BaseAddress?.ToString() ?? "https://localhost:7165/"; // Fallback
        var model = new DashboardViewModel();

        try
        {
            model.DailySales = await _httpClient.GetFromJsonAsync<DailyProfitDto>("api/Sales/DailyProfit");
            model.MonthlySales = await _httpClient.GetFromJsonAsync<MonthlyProfitDto>("api/Sales/CurrentMonthProfit");
            model.YearlySales = await _httpClient.GetFromJsonAsync<YearlyProfitDto>("api/Sales/CurrentYearProfit");
            model.Tasks = await _httpClient.GetFromJsonAsync<List<TaskItemDto>>("api/Tasks");

            // Fetch and set company logo
            var companyResp = await _httpClient.GetAsync("api/Settings/Get");
            if (companyResp.IsSuccessStatusCode)
            {
                var content = await companyResp.Content.ReadAsStringAsync();
                var company = JsonConvert.DeserializeObject<CompanyInfoViewModel>(content);
                ViewBag.CompanyLogo = company?.logoUrl;
            }

            // Set user avatar (optional)
            ViewBag.UserImage = $"https://ui-avatars.com/api/?name={HttpContext.Session.GetString("Username")}&background=0D8ABC&color=fff";
        }
        catch (Exception)
        {
            if (model.DailySales == null)
                model.DailySales = new DailyProfitDto();

            if (model.MonthlySales == null)
                model.MonthlySales = new MonthlyProfitDto();

            if (model.YearlySales == null)
                model.YearlySales = new YearlyProfitDto();

            model.DailySales.TotalProfit = model.MonthlySales.TotalProfit = model.YearlySales.TotalProfit = 0;
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
