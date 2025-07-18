namespace AuthNet.Models.DTO
{
    public class CompanyInfoHistoryDto
    {
        public int Id { get; set; }
        public DateTime ArchivedAt { get; set; }

        public string Name { get; set; }
        public string Address { get; set; }
        public string GSTIN { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
    }
}
