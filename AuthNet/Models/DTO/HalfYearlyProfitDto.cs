namespace AuthNet.Models.DTO
{
    public class HalfYearlyProfitDto
    {
        public int Year { get; set; }
        public string Half { get; set; } // "H1" or "H2"
        public decimal TotalProfit { get; set; }
    }
}
