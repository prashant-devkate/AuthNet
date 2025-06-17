using AuthNet.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace AuthNet.Models.DTO
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
