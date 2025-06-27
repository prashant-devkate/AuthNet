using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class AddTaxSettingViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "CGST is required")]
        public decimal CGST { get; set; }

        [Required(ErrorMessage = "SGST is required")]
        public decimal SGST { get; set; }

        [Required(ErrorMessage = "IGST is required")]
        public decimal IGST { get; set; }
    }
}
