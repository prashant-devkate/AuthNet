using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class AddCategoryViewModel
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
