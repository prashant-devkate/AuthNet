namespace AuthNet.Models.Domain
{
    public class CompanyInfoHistory
    {
        public int Id { get; set; }
        public DateTime ArchivedAt { get; set; } = DateTime.UtcNow;

        public string Name { get; set; }
        public string Address { get; set; }
        public string GSTIN { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
    }
}
