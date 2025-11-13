using AuthNet.UI.Models;
using AuthNet.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;

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
            model.DailySale = await _httpClient.GetFromJsonAsync<DailySalesDto>("api/Sales/Daily");
            model.MonthlySale = await _httpClient.GetFromJsonAsync<MonthlySalesDto>("api/Sales/Monthly");
            model.TotalProducts = await GetCountFromApi("api/Products/count");
            model.TotalSuppliers = await GetCountFromApi("api/Suppliers/count");
            model.TotalCategories = await GetCountFromApi("api/Categories/count");
            model.TotalOrders = await GetCountFromApi("api/Orders/count");
            model.TotalCustomers = await GetCountFromApi("api/Customers/count");
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

            model.DailySales.TotalProfit = model.MonthlySales.TotalProfit = model.YearlySales.TotalProfit =
                model.TotalProducts = model.TotalCategories = model.TotalSuppliers = model.TotalOrders =
                model.TotalCustomers = 0;
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

    [HttpGet]
    public async Task<IActionResult> DownloadDailySalesReport()
    {
        var response = await _httpClient.GetAsync("/api/Sales/daily-sales-excel");

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, "Failed to fetch report");
        }

        var content = await response.Content.ReadAsByteArrayAsync();

        // Get filename from Content-Disposition header
        var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"')
                       ?? "report.xlsx"; // fallback if header missing

        var contentType = response.Content.Headers.ContentType?.ToString()
                          ?? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        return File(content, contentType, fileName);
    }

    [HttpGet]
    public async Task<IActionResult> DownloadMonthlySalesReport()
    {
        var response = await _httpClient.GetAsync("/api/Sales/monthly-sales-excel");

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, "Failed to fetch report");
        }

        var content = await response.Content.ReadAsByteArrayAsync();

        // Get filename from Content-Disposition header
        var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"')
                       ?? $"MonthlySales_{DateTime.Today:yyyyMM}.xlsx";

        var contentType = response.Content.Headers.ContentType?.ToString()
                          ?? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        return File(content, contentType, fileName);
    }

    public async Task<IActionResult> DailySalesReport(int page = 1)
    {
        int pageSize = 10;
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
        int pageSize = 10;
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


    [HttpGet]
    public async Task<IActionResult> GenerateSalesReportPdf()
    {
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
            return RedirectToAction(nameof(DailySalesReport));
        }

        if (fullReport == null || fullReport.Sales == null || !fullReport.Sales.Any())
        {
            TempData["ErrorMessage"] = "No sales data found.";
            return RedirectToAction(nameof(DailySalesReport));
        }

        using (var stream = new MemoryStream())
        {
            var doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 25, 25, 30, 30);
            iTextSharp.text.pdf.PdfWriter.GetInstance(doc, stream);
            doc.Open();

            var titleFont = iTextSharp.text.FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLUE);
            var normalFont = iTextSharp.text.FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
            var boldFont = iTextSharp.text.FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);

            // Title
            var title = new iTextSharp.text.Paragraph(new iTextSharp.text.Phrase("Daily Sales Report", titleFont))
            {
                Alignment = iTextSharp.text.Element.ALIGN_CENTER,
                SpacingAfter = 15f
            };
            doc.Add(title);

            // Summary
            var dayHeader = new iTextSharp.text.Paragraph(
                new iTextSharp.text.Phrase($"Total Sales: ₹{fullReport.TotalSales} | Invoices: {fullReport.InvoiceCount}", boldFont))
            {
                SpacingBefore = 10f,
                SpacingAfter = 8f
            };
            doc.Add(dayHeader);

            // Table structure
            var table = new iTextSharp.text.pdf.PdfPTable(10)
            {
                WidthPercentage = 100
            };
            table.SetWidths(new float[] { 8, 12, 15, 10, 15, 12, 6, 8, 8, 10 });

            string[] headers = { "Invoice No", "Date", "Customer", "Mobile", "Address", "Product", "Qty", "Unit Price", "Total", "Invoice Total" };
            foreach (var h in headers)
            {
                var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(h, boldFont))
                {
                    BackgroundColor = new iTextSharp.text.BaseColor(245, 245, 245),
                    HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER,
                    Padding = 4
                };
                table.AddCell(cell);
            }

            // Loop over all sales
            foreach (var sale in fullReport.Sales)
            {
                int itemCount = sale.Items.Count();
                bool firstItem = true;

                foreach (var item in sale.Items)
                {
                    if (firstItem)
                    {
                        table.AddCell(new iTextSharp.text.Phrase(sale.InvoiceNo, normalFont));
                        table.AddCell(new iTextSharp.text.Phrase(sale.InvoiceDate.ToString("dd-MMM-yyyy HH:mm"), normalFont));
                        table.AddCell(new iTextSharp.text.Phrase(sale.Customername, normalFont));
                        table.AddCell(new iTextSharp.text.Phrase(sale.PhoneNumber, normalFont));
                        table.AddCell(new iTextSharp.text.Phrase(sale.Address, normalFont));
                        firstItem = false;
                    }
                    else
                    {
                        // Empty cells for rowspan effect
                        table.AddCell(""); table.AddCell(""); table.AddCell(""); table.AddCell(""); table.AddCell("");
                    }

                    table.AddCell(new iTextSharp.text.Phrase(item.ProductName, normalFont));
                    table.AddCell(new iTextSharp.text.Phrase(item.Quantity.ToString(), normalFont));
                    table.AddCell(new iTextSharp.text.Phrase("₹" + item.UnitPrice.ToString("0.00"), normalFont));
                    table.AddCell(new iTextSharp.text.Phrase("₹" + item.TotalPrice.ToString("0.00"), normalFont));

                    if (item == sale.Items.Last())
                    {
                        var totalCell = new iTextSharp.text.pdf.PdfPCell(
                            new iTextSharp.text.Phrase("₹" + sale.TotalAmount.ToString("0.00"), boldFont))
                        {
                            HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT,
                            VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,
                            Rowspan = itemCount
                        };
                        table.AddCell(totalCell);
                    }
                    else
                    {
                        table.AddCell("");
                    }
                }
            }

            doc.Add(table);
            doc.Close();

            string fileName = $"DailySalesReport_{DateTime.Now:dd-MMM-yyyy}.pdf";
            return File(stream.ToArray(), "application/pdf", fileName);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GenerateCurrentMonthSalesReportPdf()
    {
        SalesReportDto fullReport = null;

        try
        {
            var response = await _httpClient.GetAsync("api/Sales/Monthly");
            response.EnsureSuccessStatusCode();
            fullReport = await response.Content.ReadFromJsonAsync<SalesReportDto>();
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to load monthly sales.";
            return RedirectToAction(nameof(MonthlySalesReport));
        }

        if (fullReport == null || fullReport.Sales == null || !fullReport.Sales.Any())
        {
            TempData["ErrorMessage"] = "No sales data found.";
            return RedirectToAction(nameof(MonthlySalesReport));
        }

        // 🗓 Filter for current month
        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;
        var currentMonthSales = fullReport.Sales
            .Where(s => s.InvoiceDate.Month == currentMonth && s.InvoiceDate.Year == currentYear)
            .OrderBy(s => s.InvoiceDate)
            .ToList();

        // Group by day
        var groupedByDay = currentMonthSales
            .GroupBy(s => s.InvoiceDate.Date)
            .OrderBy(g => g.Key)
            .ToList();

        using (var stream = new MemoryStream())
        {
            var doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 25, 25, 30, 30);
            iTextSharp.text.pdf.PdfWriter.GetInstance(doc, stream);
            doc.Open();

            var titleFont = iTextSharp.text.FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLUE);
            var normalFont = iTextSharp.text.FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
            var boldFont = iTextSharp.text.FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);

            // Main Title
            var title = new iTextSharp.text.Paragraph(
                new iTextSharp.text.Phrase($"Sales Report - {DateTime.Now:MMMM yyyy}", titleFont))
            {
                Alignment = iTextSharp.text.Element.ALIGN_CENTER,
                SpacingAfter = 15f
            };
            doc.Add(title);

            // Loop over each day
            foreach (var dayGroup in groupedByDay)
            {
                var daySales = dayGroup.ToList();
                var dayTotal = daySales.Sum(s => s.TotalAmount);
                var dayInvoices = daySales.Count;

                // 📅 Day header
                var dayHeader = new iTextSharp.text.Paragraph(
                    new iTextSharp.text.Phrase($"{dayGroup.Key:dd-MMM-yyyy}  —  Total Sales: ₹{dayTotal} | Invoices: {dayInvoices}", boldFont))
                {
                    SpacingBefore = 10f,
                    SpacingAfter = 8f
                };
                doc.Add(dayHeader);

                // Table structure
                var table = new iTextSharp.text.pdf.PdfPTable(10)
                {
                    WidthPercentage = 100
                };
                table.SetWidths(new float[] { 8, 12, 15, 10, 15, 12, 6, 8, 8, 10 });

                string[] headers = { "Invoice No", "Date", "Customer", "Mobile", "Address", "Product", "Qty", "Unit Price", "Total", "Invoice Total" };
                foreach (var h in headers)
                {
                    var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(h, boldFont))
                    {
                        BackgroundColor = new iTextSharp.text.BaseColor(245, 245, 245),
                        HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER,
                        Padding = 4
                    };
                    table.AddCell(cell);
                }

                foreach (var sale in daySales)
                {
                    int itemCount = sale.Items.Count();
                    bool firstItem = true;

                    foreach (var item in sale.Items)
                    {
                        if (firstItem)
                        {
                            table.AddCell(new iTextSharp.text.Phrase(sale.InvoiceNo, normalFont));
                            table.AddCell(new iTextSharp.text.Phrase(sale.InvoiceDate.ToString("HH:mm"), normalFont));
                            table.AddCell(new iTextSharp.text.Phrase(sale.Customername, normalFont));
                            table.AddCell(new iTextSharp.text.Phrase(sale.PhoneNumber, normalFont));
                            table.AddCell(new iTextSharp.text.Phrase(sale.Address, normalFont));
                            firstItem = false;
                        }
                        else
                        {
                            table.AddCell(""); table.AddCell(""); table.AddCell(""); table.AddCell(""); table.AddCell("");
                        }

                        table.AddCell(new iTextSharp.text.Phrase(item.ProductName, normalFont));
                        table.AddCell(new iTextSharp.text.Phrase(item.Quantity.ToString(), normalFont));
                        table.AddCell(new iTextSharp.text.Phrase("₹" + item.UnitPrice.ToString("0.00"), normalFont));
                        table.AddCell(new iTextSharp.text.Phrase("₹" + item.TotalPrice.ToString("0.00"), normalFont));

                        if (item == sale.Items.Last())
                        {
                            var totalCell = new iTextSharp.text.pdf.PdfPCell(
                                new iTextSharp.text.Phrase("₹" + sale.TotalAmount.ToString("0.00"), boldFont))
                            {
                                HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT,
                                VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE,
                                Rowspan = itemCount
                            };
                            table.AddCell(totalCell);
                        }
                        else
                        {
                            table.AddCell("");
                        }
                    }
                }

                doc.Add(table);
            }

            doc.Close();

            string fileName = $"SalesReport_{DateTime.Now:MMMM_yyyy}.pdf";
            return File(stream.ToArray(), "application/pdf", fileName);
        }
    }


}
