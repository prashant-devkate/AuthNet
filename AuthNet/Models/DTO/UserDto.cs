namespace AuthNet.Models.DTO
{
    public class RegisterDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }

    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
