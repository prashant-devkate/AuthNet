namespace AuthNet.UI.Models.DTO
{
    public class ChangePasswordDTO
    {
        public int userId { get; set; }
        public string currentPassword { get; set; }
        public string newPassword { get; set; }
    }
}
