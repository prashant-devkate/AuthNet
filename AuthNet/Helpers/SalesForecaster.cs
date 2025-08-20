using AuthNet.Models.DTO;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;

namespace AuthNet.Helpers
{
    public static class SalesForecaster
    {
        public static float[] ForecastNextDays(List<(DateTime day, int qty)> series, int forecastHorizon = 7)
        {
            var mlContext = new MLContext();

            // 1) Convert series into ML.NET input
            var data = series.Select(s => new DailySaleData { Quantity = s.qty }).ToList();
            var dataView = mlContext.Data.LoadFromEnumerable(data);

            // 2) Define forecasting pipeline
            var forecastingPipeline = mlContext.Forecasting.ForecastBySsa(
                outputColumnName: nameof(DailySaleForecast.Forecasted),
                inputColumnName: nameof(DailySaleData.Quantity),
                windowSize: 7,             // weekly pattern assumption
                seriesLength: data.Count,  // length of history
                trainSize: data.Count,     // all history for training
                horizon: forecastHorizon,  // how many days to predict
                confidenceLevel: 0.95f,    // optional, prediction intervals
                confidenceLowerBoundColumn: "LowerBound",
                confidenceUpperBoundColumn: "UpperBound");

            // 3) Train model
            var model = forecastingPipeline.Fit(dataView);

            // 4) Forecast future values
            var forecastEngine = model.CreateTimeSeriesEngine<DailySaleData, DailySaleForecast>(mlContext);
            var forecast = forecastEngine.Predict();

            return forecast.Forecasted; // array of 7 values (next 7 days)
        }
    }
}



