using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Notification.DTOs
{
    public class SendNotificationRequestDto
    {
        public List<string> TeacherEmails { get; set; } = new List<string>(); 
        public string Subject { get; set; } 
        public string Body { get; set; } 
        public bool IsHtml { get; set; } = true;
    }
}
