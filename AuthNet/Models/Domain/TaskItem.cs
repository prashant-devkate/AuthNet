namespace AuthNet.Models.Domain
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
    }
}
