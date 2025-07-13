namespace AuthNet.Models.DTO
{
    public class TaxSettingResponseDto
    {
        public int Id { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal IGST { get; set; }
    }
}
