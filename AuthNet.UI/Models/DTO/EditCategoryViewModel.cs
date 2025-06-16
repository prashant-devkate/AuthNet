using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class EditCategoryViewModel : AddCategoryViewModel
    {
        [Required]
        public new int CategoryId { get; set; }
    }
}
