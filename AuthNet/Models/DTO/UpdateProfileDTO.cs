namespace AuthNet.Models.DTO
{
    public class UpdateProfileDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}
