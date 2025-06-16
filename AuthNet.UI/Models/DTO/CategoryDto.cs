using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }

        [Display(Name = "Category Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}
