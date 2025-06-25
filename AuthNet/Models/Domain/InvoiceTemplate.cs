namespace AuthNet.Models.Domain
{
    public class InvoiceTemplate
    {
        public int Id { get; set; }
        public string Layout { get; set; }
        public string Notes { get; set; }
        public string TermsAndConditions { get; set; }
    }
}
