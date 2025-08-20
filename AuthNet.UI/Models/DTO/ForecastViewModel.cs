namespace AuthNet.UI.Models.DTO
{
    public class ForecastViewModel
    {
        public List<ForecastDto> Forecast { get; set; } = new();
        public HistoryResponseDto History { get; set; } = new();
        public int ProductId { get; set; }
    }
}
