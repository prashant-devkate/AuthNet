namespace AuthNet.UI.Models.DTO
{
    public class SalesListViewModel
    {
        public List<SalesReportDto> Sales { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
