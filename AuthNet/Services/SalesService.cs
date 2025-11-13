using AuthNet.Data;
using AuthNet.Models.Domain;
using AuthNet.Models.DTO;
using AuthNet.Services.Interfaces;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Drawing;

namespace AuthNet.Services
{
    public class SalesService : ISalesService
    {
        private readonly AppDbContext _context;
        public SalesService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Sale>> GetAllAsync()
        {
            return await _context.Sales.Include(s => s.SaleItems).ToListAsync();
        }

        public async Task<Sale?> GetByIdAsync(int id)
        {
            return await _context.Sales
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<int> GetSaleCountAsync()
        {
            return await _context.Sales.CountAsync();
        }

        public async Task<OperationResponse> AddAsync(SaleDto saleDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(); // ensures atomicity

            try
            {

                //Get last invoice number from DB
                var lastSale = await _context.Sales
                    .OrderByDescending(s => s.Id)
                    .FirstOrDefaultAsync();

                string nextInvoiceNo;

                if (lastSale == null || string.IsNullOrEmpty(lastSale.InvoiceNo))
                {
                    // Start fresh
                    nextInvoiceNo = "INV-1001";
                }

                else
                {
                    // Extract numeric part safely
                    var match = System.Text.RegularExpressions.Regex.Match(lastSale.InvoiceNo, @"\d+");
                    int lastNumber = match.Success ? int.Parse(match.Value) : 1000;
                    nextInvoiceNo = $"INV-{lastNumber + 1}";
                }

                var sale = new Sale
                {
                    InvoiceNo = nextInvoiceNo,
                    InvoiceDate = DateTime.Now,
                    Customername = saleDto.Customername,
                    Address = saleDto.Address,
                    PhoneNumber = saleDto.PhoneNumber,
                    PrincipalAmount = saleDto.PrincipalAmount,
                    DiscountedAmount = saleDto.DiscountedAmount,
                    AfterTaxAmount = saleDto.AfterTaxAmount,
                    TotalAmount = saleDto.TotalAmount,
                    Template = saleDto.Template,
                    CreatedAt = DateTime.Now,
                    SaleItems = saleDto.SaleItems.Select(i => new SaleItem
                    {
                        ProductId = i.ProductId,
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        TotalPrice = i.TotalPrice > 0 ? i.TotalPrice : i.Quantity * i.UnitPrice
                    }).ToList()
                };
                    
                await _context.Sales.AddAsync(sale);


                // Reduce inventory stock for each product sold
                foreach (var item in sale.SaleItems)
                {
                    var inventory = await _context.Inventories
                        .FirstOrDefaultAsync(inv => inv.ProductId == item.ProductId);

                    if (inventory != null)
                    {
                        // Check if sufficient stock exists
                        if (inventory.QuantityInStock < item.Quantity)
                        {
                            await transaction.RollbackAsync();
                            return new OperationResponse
                            {
                                Success = false,
                                Message = $"Insufficient stock for product: {item.ProductName}"
                            };
                        }

                        // Reduce quantity
                        inventory.QuantityInStock -= item.Quantity;
                        inventory.LastUpdated = DateTime.Now;

                        _context.Inventories.Update(inventory);
                    }
                }




                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                //Return invoice number in the response
                return new OperationResponse
                {
                    Success = true,
                    Message = $"Sale added successfully.  Invoice No: {sale.InvoiceNo}",
                    Data = new { InvoiceNo = sale.InvoiceNo}
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new OperationResponse
                {
                    Success = false,
                    Message = $"Error adding sale: {ex.Message}"
                };
            }
        }

        public async Task<OperationResponse?> UpdateAsync(int id, Sale sale)
        {
            var existing = await _context.Sales.FindAsync(id);
            if (existing == null)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = $"Sale with ID {id} not found."
                };
            }

            _context.Entry(existing).CurrentValues.SetValues(sale);

            try
            {
                await _context.SaveChangesAsync();
                return new OperationResponse
                {
                    Success = true,
                    Message = "Sale updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = $"Error updating sale: {ex.Message}"
                };
            }
        }

        public async Task<OperationResponse> DeleteAsync(int id)
        {
            var sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = $"Sale with ID {id} not found."
                };
            }

            try
            {
                _context.Sales.Remove(sale);
                await _context.SaveChangesAsync();
                return new OperationResponse
                {
                    Success = true,
                    Message = "Sale deleted successfully."
                };
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("FK_") == true)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = "Delete failed: The sale is referenced by other records."
                };
            }
            catch (DbUpdateException ex)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = $"Database update error: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new OperationResponse
                {
                    Success = false,
                    Message = $"An unexpected error occurred: {ex.Message}"
                };
            }
        }

        public async Task<ReportResultDto> GetDailySalesAsync()
        {
            var today = DateTime.Today;

            var sales = await _context.Sales
                .Where(s => s.InvoiceDate.Date == today)
                .Include(s => s.SaleItems)
                .ToListAsync();

            return BuildReport("Daily Sales", sales);
        }

       

    public async Task<byte[]> GenerateDailySalesExcelAsync()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var sales = await _context.Sales
                .Where(s => s.InvoiceDate >= today && s.InvoiceDate < tomorrow)
                .Include(s => s.SaleItems)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Daily Sales");

            // Header
            worksheet.Cell(1, 1).Value = "Invoice No";
            worksheet.Cell(1, 2).Value = "Date";
            worksheet.Cell(1, 3).Value = "Total Items";
            worksheet.Cell(1, 4).Value = "Total Amount";

            int row = 2;

            if (sales.Any())
            {
                foreach (var sale in sales)
                {
                    worksheet.Cell(row, 1).Value = sale.InvoiceNo;
                    worksheet.Cell(row, 2).Value = sale.InvoiceDate.ToString("yyyy-MM-dd");
                    worksheet.Cell(row, 3).Value = sale.SaleItems.Count;
                    worksheet.Cell(row, 4).Value = sale.SaleItems.Sum(item => item.Quantity * item.UnitPrice);
                    row++;
                }
            }
            else
            {
                worksheet.Cell(row, 1).Value = $"No sales data available for {today:yyyy-MM-dd}";
                worksheet.Range(row, 1, row, 4).Merge();
                worksheet.Cell(row, 1).Style.Font.Italic = true;
                worksheet.Cell(row, 1).Style.Font.FontColor = XLColor.Gray;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }


        public async Task<byte[]> GenerateMonthlySalesExcelAsync()
        {
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);

            var sales = await _context.Sales
                .Where(s => s.InvoiceDate >= firstDayOfMonth && s.InvoiceDate < firstDayOfNextMonth)
                .Include(s => s.SaleItems)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Monthly Sales");

            // Header
            worksheet.Cell(1, 1).Value = "Invoice No";
            worksheet.Cell(1, 2).Value = "Date";
            worksheet.Cell(1, 3).Value = "Total Items";
            worksheet.Cell(1, 4).Value = "Total Amount";

            int row = 2;

            if (sales.Any())
            {
                foreach (var sale in sales)
                {
                    worksheet.Cell(row, 1).Value = sale.InvoiceNo;
                    worksheet.Cell(row, 2).Value = sale.InvoiceDate.ToString("yyyy-MM-dd");
                    worksheet.Cell(row, 3).Value = sale.SaleItems.Count;
                    worksheet.Cell(row, 4).Value = sale.SaleItems.Sum(item => item.Quantity * item.UnitPrice);
                    row++;
                }
            }
            else
            {
                worksheet.Cell(row, 1).Value = $"No sales data available for {firstDayOfMonth:MMMM yyyy}";
                worksheet.Range(row, 1, row, 4).Merge();
                worksheet.Cell(row, 1).Style.Font.Italic = true;
                worksheet.Cell(row, 1).Style.Font.FontColor = XLColor.Gray;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<ReportResultDto> GetMonthlySalesAsync()
        {
            var now = DateTime.Now;
            var start = new DateTime(now.Year, now.Month, 1);
            var end = start.AddMonths(1);

            var sales = await _context.Sales
                .Where(s => s.InvoiceDate >= start && s.InvoiceDate < end)
                .Include(s => s.SaleItems)
                .ToListAsync();

            return BuildReport("Monthly Sales", sales);
        }

        public async Task<ReportResultDto> GetYearlySalesAsync()
        {
            var now = DateTime.Now;
            var start = new DateTime(now.Year, 1, 1);
            var end = start.AddYears(1);

            var sales = await _context.Sales
                .Where(s => s.InvoiceDate >= start && s.InvoiceDate < end)
                .Include(s => s.SaleItems)
                .ToListAsync();

            return BuildReport("Yearly Sales", sales);
        }

        private ReportResultDto BuildReport(string title, List<Sale> sales)
        {
            return new ReportResultDto
            {
                Title = title,
                TotalSales = sales.Sum(s => s.TotalAmount),
                InvoiceCount = sales.Count,
                Sales = sales.Select(s => new ReportInvoiceDto
                {
                    InvoiceNo = s.InvoiceNo,
                    InvoiceDate = s.InvoiceDate,
                    Customername = s.Customername,
                    Address = s.Address,
                    PhoneNumber = s.PhoneNumber,
                    PrincipalAmount = s.PrincipalAmount,
                    DiscountedAmount = s.DiscountedAmount,
                    AfterTaxAmount = s.AfterTaxAmount,
                    TotalAmount = s.TotalAmount,
                    Items = s.SaleItems.Select(i => new ReportSaleItemDto
                    {
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        TotalPrice = i.TotalPrice
                    }).ToList()
                }).ToList()
            };
        }


        public async Task<DailyProfitDto> CalculateDailyProfitAsync()
        {
            DateTime date = DateTime.Today;

            // Load all sales for today with their items
            var sales = await _context.Sales
                .Include(s => s.SaleItems)
                .Where(s => s.InvoiceDate.Date == date.Date)
                .ToListAsync();

            // Collect all product ids used in today's sales
            var productIds = sales
                .SelectMany(s => s.SaleItems)
                .Select(i => i.ProductId)
                .Distinct()
                .ToList();

            // Load product cost prices into a dictionary for fast lookup
            var products = await _context.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToDictionaryAsync(p => p.ProductId, p => p);

            decimal totalProfit = 0m;

            foreach (var sale in sales)
            {
                // Sum of cost for this sale (costPrice * quantity)
                decimal saleCost = 0m;

                foreach (var item in sale.SaleItems)
                {
                    if (products.TryGetValue(item.ProductId, out var product) && product.CostPrice > 0)
                    {
                        saleCost += product.CostPrice * item.Quantity;
                    }
                    // if product not found or cost price missing, we treat cost as 0.
                    // Optionally you can log or handle missing cost prices differently.
                }

                // Use sale.TotalAmount (after discounts/taxes) as revenue for profit calculation
                decimal saleRevenue = sale.TotalAmount;

                decimal saleProfit = saleRevenue - saleCost;
                totalProfit += saleProfit;
            }

            return new DailyProfitDto
            {
                Date = date.Date,
                TotalProfit = totalProfit
            };
        }


        public async Task<List<DailyProfitDto>> CalculateWeeklyProfitAsync()
        {
            DateTime startDate = DateTime.Today.AddDays(-6); // last 7 days (including today)
            DateTime endDate = DateTime.Today;

            // Load sales for the last 7 days (with items)
            var sales = await _context.Sales
                .Include(s => s.SaleItems)
                .Where(s => s.InvoiceDate.Date >= startDate && s.InvoiceDate.Date <= endDate)
                .ToListAsync();

            // Collect all unique product IDs
            var productIds = sales
                .SelectMany(s => s.SaleItems)
                .Select(i => i.ProductId)
                .Distinct()
                .ToList();

            // Fetch product cost prices
            var products = await _context.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToDictionaryAsync(p => p.ProductId, p => p);

            // Calculate profit per day
            var profitByDate = new Dictionary<DateTime, decimal>();

            foreach (var sale in sales)
            {
                decimal saleCost = 0m;

                foreach (var item in sale.SaleItems)
                {
                    if (products.TryGetValue(item.ProductId, out var product))
                    {
                        saleCost += product.CostPrice * item.Quantity;
                    }
                }

                // Revenue after discounts/tax
                decimal saleRevenue = sale.TotalAmount;
                decimal saleProfit = saleRevenue - saleCost;

                DateTime saleDate = sale.InvoiceDate.Date;
                if (profitByDate.ContainsKey(saleDate))
                    profitByDate[saleDate] += saleProfit;
                else
                    profitByDate[saleDate] = saleProfit;
            }

            // Generate 7-day list including days with no sales
            var result = Enumerable.Range(0, 7)
                .Select(i =>
                {
                    var date = startDate.AddDays(i);
                    return new DailyProfitDto
                    {
                        Date = date,
                        TotalProfit = profitByDate.TryGetValue(date, out var profit) ? profit : 0m
                    };
                })
                .ToList();

            return result;
        }

        //Wrong logic
        //public async Task<List<MonthlyProfitDto>> CalculateMonthlyProfitAsync(int? year = null)
        //{
        //    year ??= DateTime.Today.Year;

        //    var saleItems = await _context.SaleItems
        //        .Include(si => si.Sale)
        //        .Where(si => si.Sale.InvoiceDate.Year == year)
        //        .ToListAsync();

        //    var productIds = saleItems.Select(si => si.ProductId).Distinct().ToList();

        //    var products = await _context.Products
        //        .Where(p => productIds.Contains(p.ProductId))
        //        .ToDictionaryAsync(p => p.ProductId, p => p);

        //    var grouped = saleItems
        //        .GroupBy(si => new { si.Sale.InvoiceDate.Year, si.Sale.InvoiceDate.Month })
        //        .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
        //        .Select(g =>
        //        {
        //            decimal profit = 0;
        //            foreach (var item in g)
        //            {
        //                if (products.TryGetValue(item.ProductId, out var product))
        //                {
        //                    var profitPerItem = item.UnitPrice - product.CostPrice;
        //                    profit += profitPerItem * item.Quantity;
        //                }
        //            }

        //            return new MonthlyProfitDto
        //            {
        //                Year = g.Key.Year,
        //                Month = g.Key.Month,
        //                TotalProfit = profit
        //            };
        //        })
        //        .ToList();

        //    return grouped;
        //}

        public async Task<MonthlyProfitDto> CalculateCurrentMonthProfitAsync()
        {
            var today = DateTime.Today;
            int currentYear = today.Year;
            int currentMonth = today.Month;

            // Load sales for the current month with their items
            var sales = await _context.Sales
                .Include(s => s.SaleItems)
                .Where(s => s.InvoiceDate.Year == currentYear &&
                            s.InvoiceDate.Month == currentMonth)
                .ToListAsync();

            if (!sales.Any())
            {
                return new MonthlyProfitDto
                {
                    Year = currentYear,
                    Month = currentMonth,
                    TotalProfit = 0m
                };
            }

            // Collect all unique product IDs
            var productIds = sales
                .SelectMany(s => s.SaleItems)
                .Select(i => i.ProductId)
                .Distinct()
                .ToList();

            // Load products for cost prices
            var products = await _context.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToDictionaryAsync(p => p.ProductId, p => p);

            decimal totalProfit = 0m;

            foreach (var sale in sales)
            {
                decimal saleCost = 0m;

                foreach (var item in sale.SaleItems)
                {
                    if (products.TryGetValue(item.ProductId, out var product))
                    {
                        saleCost += product.CostPrice * item.Quantity;
                    }
                }

                // Revenue after discount/tax
                decimal saleRevenue = sale.TotalAmount;
                totalProfit += (saleRevenue - saleCost);
            }

            return new MonthlyProfitDto
            {
                Year = currentYear,
                Month = currentMonth,
                TotalProfit = totalProfit
            };
        }

        //Wrong logic
        //public async Task<List<HalfYearlyProfitDto>> CalculateHalfYearlyProfitAsync(int? year = null)
        //{
        //    year ??= DateTime.Today.Year;

        //    var saleItems = await _context.SaleItems
        //        .Include(si => si.Sale)
        //        .Where(si => si.Sale.InvoiceDate.Year == year)
        //        .ToListAsync();

        //    var productIds = saleItems.Select(si => si.ProductId).Distinct().ToList();

        //    var products = await _context.Products
        //        .Where(p => productIds.Contains(p.ProductId))
        //        .ToDictionaryAsync(p => p.ProductId, p => p);

        //    var grouped = saleItems
        //        .GroupBy(si =>
        //        {
        //            var month = si.Sale.InvoiceDate.Month;
        //            var half = (month <= 6) ? "H1" : "H2";
        //            return new { si.Sale.InvoiceDate.Year, Half = half };
        //        })
        //        .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Half)
        //        .Select(g =>
        //        {
        //            decimal profit = 0;
        //            foreach (var item in g)
        //            {
        //                if (products.TryGetValue(item.ProductId, out var product))
        //                {
        //                    var profitPerItem = item.UnitPrice - product.CostPrice;
        //                    profit += profitPerItem * item.Quantity;
        //                }
        //            }

        //            return new HalfYearlyProfitDto
        //            {
        //                Year = g.Key.Year,
        //                Half = g.Key.Half,
        //                TotalProfit = profit
        //            };
        //        })
        //        .ToList();

        //    return grouped;
        //}


        //Wrong logic
        //public async Task<List<YearlyProfitDto>> CalculateYearlyProfitAsync()
        //{
        //    var saleItems = await _context.SaleItems
        //        .Include(si => si.Sale)
        //        .ToListAsync();

        //    var productIds = saleItems.Select(si => si.ProductId).Distinct().ToList();

        //    var products = await _context.Products
        //        .Where(p => productIds.Contains(p.ProductId))
        //        .ToDictionaryAsync(p => p.ProductId, p => p);

        //    var grouped = saleItems
        //        .GroupBy(si => si.Sale.InvoiceDate.Year)
        //        .OrderBy(g => g.Key)
        //        .Select(g =>
        //        {
        //            decimal profit = 0;
        //            foreach (var item in g)
        //            {
        //                if (products.TryGetValue(item.ProductId, out var product))
        //                {
        //                    var profitPerItem = item.UnitPrice - product.CostPrice;
        //                    profit += profitPerItem * item.Quantity;
        //                }
        //            }

        //            return new YearlyProfitDto
        //            {
        //                Year = g.Key,
        //                TotalProfit = profit
        //            };
        //        })
        //        .ToList();

        //    return grouped;
        //}

        public async Task<YearlyProfitDto> CalculateCurrentYearProfitAsync()
        {
            int currentYear = DateTime.Today.Year;

            // Load all sales for the current year including their items
            var sales = await _context.Sales
                .Include(s => s.SaleItems)
                .Where(s => s.InvoiceDate.Year == currentYear)
                .ToListAsync();

            if (!sales.Any())
            {
                return new YearlyProfitDto
                {
                    Year = currentYear,
                    TotalProfit = 0m
                };
            }

            // Collect all unique product IDs from the sales
            var productIds = sales
                .SelectMany(s => s.SaleItems)
                .Select(i => i.ProductId)
                .Distinct()
                .ToList();

            // Fetch product cost prices
            var products = await _context.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToDictionaryAsync(p => p.ProductId, p => p);

            decimal totalProfit = 0m;

            foreach (var sale in sales)
            {
                decimal saleCost = 0m;

                foreach (var item in sale.SaleItems)
                {
                    if (products.TryGetValue(item.ProductId, out var product))
                    {
                        saleCost += product.CostPrice * item.Quantity;
                    }
                }

                // Revenue after discount/tax
                decimal saleRevenue = sale.TotalAmount;
                totalProfit += (saleRevenue - saleCost);
            }

            return new YearlyProfitDto
            {
                Year = currentYear,
                TotalProfit = totalProfit
            };
        }




    }
}
