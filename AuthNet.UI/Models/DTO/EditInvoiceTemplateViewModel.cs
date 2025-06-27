using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class EditInvoiceTemplateViewModel : AddInvoiceTemplateViewModel
    {
        [Required]
        public new int Id { get; set; }
    }
}
