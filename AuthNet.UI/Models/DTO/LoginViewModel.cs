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
        public int userId { get; set; }
        public string username { get; set; }
        public string role { get; set; }
    }

    public class ServiceResponse<T>
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
