using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class AddTaskItemViewModel
    {
        public int TaskId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public DateTime DueDate { get; set; }
    }
}
