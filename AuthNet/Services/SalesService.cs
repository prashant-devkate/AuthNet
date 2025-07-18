using AuthNet.Data;
using AuthNet.Models.Domain;
using AuthNet.Models.DTO;
using AuthNet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            var sale = new Sale
            {
                InvoiceNo = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                InvoiceDate = DateTime.Now,
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

            try
            {
                await _context.Sales.AddAsync(sale);
                await _context.SaveChangesAsync();

                return new OperationResponse
                {
                    Success = true,
                    Message = "Sale added successfully."
                };
            }
            catch (Exception ex)
            {
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

            var saleItems = await _context.SaleItems
                .Include(si => si.Sale)
                .Where(si => si.Sale.InvoiceDate.Date == date.Date)
                .ToListAsync();

            // Get product cost prices
            var productIds = saleItems.Select(si => si.ProductId).Distinct().ToList();

            var products = await _context.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToDictionaryAsync(p => p.ProductId, p => p);

            decimal totalProfit = 0;

            foreach (var item in saleItems)
            {
                if (products.TryGetValue(item.ProductId, out var product))
                {
                    var profitPerItem = item.UnitPrice - product.CostPrice;
                    totalProfit += profitPerItem * item.Quantity;
                }
            }

            return new DailyProfitDto
            {
                Date = date.Date,
                TotalProfit = totalProfit
            };
        }

        public async Task<List<DailyProfitDto>> CalculateWeeklyProfitAsync()
        {
            DateTime startDate = DateTime.Today.AddDays(-6);
            DateTime endDate = DateTime.Today;

            var saleItems = await _context.SaleItems
                .Include(si => si.Sale)
                .Where(si => si.Sale.InvoiceDate.Date >= startDate && si.Sale.InvoiceDate.Date <= endDate)
                .ToListAsync();

            var productIds = saleItems.Select(si => si.ProductId).Distinct().ToList();

            var products = await _context.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToDictionaryAsync(p => p.ProductId, p => p);

            var grouped = saleItems
                .GroupBy(si => si.Sale.InvoiceDate.Date)
                .OrderBy(g => g.Key)
                .Select(g =>
                {
                    decimal profit = 0;
                    foreach (var item in g)
                    {
                        if (products.TryGetValue(item.ProductId, out var product))
                        {
                            var profitPerItem = item.UnitPrice - product.CostPrice;
                            profit += profitPerItem * item.Quantity;
                        }
                    }

                    return new DailyProfitDto
                    {
                        Date = g.Key,
                        TotalProfit = profit
                    };
                })
                .ToList();

            return grouped;
        }

        public async Task<List<MonthlyProfitDto>> CalculateMonthlyProfitAsync(int? year = null)
        {
            year ??= DateTime.Today.Year;

            var saleItems = await _context.SaleItems
                .Include(si => si.Sale)
                .Where(si => si.Sale.InvoiceDate.Year == year)
                .ToListAsync();

            var productIds = saleItems.Select(si => si.ProductId).Distinct().ToList();

            var products = await _context.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToDictionaryAsync(p => p.ProductId, p => p);

            var grouped = saleItems
                .GroupBy(si => new { si.Sale.InvoiceDate.Year, si.Sale.InvoiceDate.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g =>
                {
                    decimal profit = 0;
                    foreach (var item in g)
                    {
                        if (products.TryGetValue(item.ProductId, out var product))
                        {
                            var profitPerItem = item.UnitPrice - product.CostPrice;
                            profit += profitPerItem * item.Quantity;
                        }
                    }

                    return new MonthlyProfitDto
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        TotalProfit = profit
                    };
                })
                .ToList();

            return grouped;
        }

        public async Task<List<HalfYearlyProfitDto>> CalculateHalfYearlyProfitAsync(int? year = null)
        {
            year ??= DateTime.Today.Year;

            var saleItems = await _context.SaleItems
                .Include(si => si.Sale)
                .Where(si => si.Sale.InvoiceDate.Year == year)
                .ToListAsync();

            var productIds = saleItems.Select(si => si.ProductId).Distinct().ToList();

            var products = await _context.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToDictionaryAsync(p => p.ProductId, p => p);

            var grouped = saleItems
                .GroupBy(si =>
                {
                    var month = si.Sale.InvoiceDate.Month;
                    var half = (month <= 6) ? "H1" : "H2";
                    return new { si.Sale.InvoiceDate.Year, Half = half };
                })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Half)
                .Select(g =>
                {
                    decimal profit = 0;
                    foreach (var item in g)
                    {
                        if (products.TryGetValue(item.ProductId, out var product))
                        {
                            var profitPerItem = item.UnitPrice - product.CostPrice;
                            profit += profitPerItem * item.Quantity;
                        }
                    }

                    return new HalfYearlyProfitDto
                    {
                        Year = g.Key.Year,
                        Half = g.Key.Half,
                        TotalProfit = profit
                    };
                })
                .ToList();

            return grouped;
        }

        public async Task<List<YearlyProfitDto>> CalculateYearlyProfitAsync()
        {
            var saleItems = await _context.SaleItems
                .Include(si => si.Sale)
                .ToListAsync();

            var productIds = saleItems.Select(si => si.ProductId).Distinct().ToList();

            var products = await _context.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToDictionaryAsync(p => p.ProductId, p => p);

            var grouped = saleItems
                .GroupBy(si => si.Sale.InvoiceDate.Year)
                .OrderBy(g => g.Key)
                .Select(g =>
                {
                    decimal profit = 0;
                    foreach (var item in g)
                    {
                        if (products.TryGetValue(item.ProductId, out var product))
                        {
                            var profitPerItem = item.UnitPrice - product.CostPrice;
                            profit += profitPerItem * item.Quantity;
                        }
                    }

                    return new YearlyProfitDto
                    {
                        Year = g.Key,
                        TotalProfit = profit
                    };
                })
                .ToList();

            return grouped;
        }


    }
}
