using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class ImageUploadViewModel
    {
        [Required]
        public IFormFile File { get; set; }

        [Required]
        public string FileName { get; set; }

        public string FileDescription { get; set; }
    }
}
