using AuthNet.Data;
using AuthNet.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForecastController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ForecastController(AppDbContext db)
        {
            _db = db;
        }

        // GET api/forecast/{productId}?days=7&historyDays=90
        [HttpGet("{productId:int}")]
        public async Task<IActionResult> Get(int productId, int days = 7, int historyDays = 90)
        {
            var to = DateTime.UtcNow.Date;
            var from = to.AddDays(-historyDays + 1);

            var series = await ForecastingHelper.GetDailyUnitsAsync(_db, productId, from, to);

            return Ok(new
            {
                productId,
                historyDays,
                totalPoints = series.Count,
                series = series.Select(s => new { date = s.day, quantity = s.qty })
            });
        }

        [HttpGet("forecast/{productId}")]
        public async Task<IActionResult> GetForecast(AppDbContext db, int productId, int days = 7)
        {
            var from = DateTime.UtcNow.AddDays(-90);
            var to = DateTime.UtcNow;

            // Get historical series
            var series = await ForecastingHelper.GetDailyUnitsAsync(db, productId, from, to);

            // Forecast
            var forecast = SalesForecaster.ForecastNextDays(series, days);

            // Combine with dates
            var results = new List<object>();
            for (int i = 0; i < forecast.Length; i++)
            {
                results.Add(new
                {
                    Date = to.AddDays(i + 1).ToString("yyyy-MM-dd"),
                    PredictedQty = Math.Max(0, (int)Math.Round(forecast[i])) // no negative sales
                });
            }

            return Ok(results);
        }

    }
}
