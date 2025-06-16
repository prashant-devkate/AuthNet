using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class EditProductViewModel : AddProductViewModel
    {
        [Required]
        public new int ProductId { get; set; }
    }
}
