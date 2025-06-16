using System.ComponentModel.DataAnnotations;

namespace AuthNet.UI.Models.DTO
{
    public class EditTaskItemViewModel : AddTaskItemViewModel
    {
        [Required]
        public new int TaskId { get; set; }
    }
}
