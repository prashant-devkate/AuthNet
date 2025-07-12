namespace AuthNet.UI.Models.DTO
{
    public class EmailSendViewModel
    {
        public string FromEmail { get; set; } = "noreply@gmail.com";
        public string FromName { get; set; } = "Mudraa Team";
        public string ToEmails { get; set; } = ""; // comma-separated
        public string Subject { get; set; }
        public int SelectedTemplate { get; set; }
        public string KeyValuePairs { get; set; }
    }
}
