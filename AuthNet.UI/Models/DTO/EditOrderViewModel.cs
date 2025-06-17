using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class EditOrderViewModel : AddOrderViewModel
    {
        [Required]
        public new int OrderId { get; set; }
    }
}
