using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class AddInvoiceTemplateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Layout name is required")]
        public string Layout { get; set; }

        [Required(ErrorMessage = "Notes are required")]
        public string Notes { get; set; }

        [Required(ErrorMessage = "Terms & Conditions are required")]
        public string TermsAndConditions { get; set; }
    }
}
