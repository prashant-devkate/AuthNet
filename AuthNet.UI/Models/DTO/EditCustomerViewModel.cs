using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class EditCustomerViewModel : AddCustomerViewModel
    {
        [Required]
        public new int CustomerId { get; set; }
    }
}
