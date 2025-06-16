namespace AuthNet.Models.DTO
{
    public class TaskItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime DueDate { get; set; }  // For edit form
        public string DueLabel { get; set; } = string.Empty;   // e.g., "Today"
        public string BadgeColor { get; set; } = "secondary";  // e.g., "danger"
    }

    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
    }

    public class UpdateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
    }

}
