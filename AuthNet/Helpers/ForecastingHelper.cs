using AuthNet.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthNet.Helpers
{
    public static class ForecastingHelper
    {
        public static async Task<List<(DateTime day, int qty)>> GetDailyUnitsAsync(
     AppDbContext db, int productId, DateTime from, DateTime to)
        {
            var start = from.Date;
            var end = to.Date.AddDays(1).AddTicks(-1); // include full "to" day

            // Get raw grouped quantities by date
            var raw = await db.SaleItems
                .Where(si => si.ProductId == productId)
                .Join(db.Sales, si => si.SaleId, s => s.Id, (si, s) => new { si, s.InvoiceDate })
                .Where(x => x.InvoiceDate >= start && x.InvoiceDate <= end)
                .GroupBy(x => x.InvoiceDate.Date)
                .Select(g => new { Day = g.Key, Qty = g.Sum(x => x.si.Quantity) })
                .ToListAsync();

            // Normalize dictionary with Date-only keys
            var dict = raw.ToDictionary(x => x.Day.Date, x => x.Qty);

            // Fill gaps with 0
            var series = new List<(DateTime day, int qty)>();
            for (var d = start; d <= to.Date; d = d.AddDays(1))
            {
                dict.TryGetValue(d, out var q);
                series.Add((d, q));
            }

            return series;
        }

    }
}