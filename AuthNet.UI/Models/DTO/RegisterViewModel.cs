using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class RegisterViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
