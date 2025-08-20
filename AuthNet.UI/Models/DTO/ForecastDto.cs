namespace AuthNet.UI.Models.DTO
{
    public class ForecastDto
    {
        public DateTime Date { get; set; }
        public int PredictedQty { get; set; }   // Predicted qty
    }

    public class HistorySeriesDto
    {
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
    }

    public class HistoryResponseDto
    {
        public int ProductId { get; set; }
        public int HistoryDays { get; set; }
        public int TotalPoints { get; set; }

        public List<HistorySeriesDto> Series { get; set; }
    }
}
