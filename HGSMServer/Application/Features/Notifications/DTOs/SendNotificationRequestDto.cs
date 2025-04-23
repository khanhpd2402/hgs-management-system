namespace Application.Features.Notifications.DTOs
{
    public class SendNotificationRequestDto
    {
        public List<int> TeacherIds { get; set; } = new List<int>();
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; } = true;
    }
}