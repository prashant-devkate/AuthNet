using System.ComponentModel.DataAnnotations;

namespace AuthNet.Models.Domain
{
    public class AuditLog
    {
        [Key]
        public int LogId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        [StringLength(100)]
        public string Action { get; set; }

        [Required]
        public string TableName { get; set; }

        public int? EntityId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
