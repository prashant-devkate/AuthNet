namespace AuthNet.UI.Models.DTO
{
    public class TaskItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime DueDate { get; set; }
        public string BadgeColor { get; set; }
        public string DueLabel { get; set; }
    }
}
