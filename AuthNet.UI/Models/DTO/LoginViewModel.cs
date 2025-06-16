using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
    }
}
