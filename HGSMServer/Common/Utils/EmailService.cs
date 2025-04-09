using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Common.Utils
{
    public class EmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;

       
        public EmailService(string smtpHost, int smtpPort, string smtpUsername, string smtpPassword,
                          string fromEmail, string fromName = "Trường THCS Hải Giang")
        {
            _smtpHost = smtpHost ?? throw new ArgumentNullException(nameof(smtpHost));
            _smtpPort = smtpPort;
            _smtpUsername = smtpUsername ?? throw new ArgumentNullException(nameof(smtpUsername));
            _smtpPassword = smtpPassword ?? throw new ArgumentNullException(nameof(smtpPassword));
            _fromEmail = fromEmail ?? throw new ArgumentNullException(nameof(fromEmail));
            _fromName = fromName;
        }

        // Hàm gửi email bất đồng bộ
        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = false)
        {
            try
            {
                using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                {
                    smtpClient.EnableSsl = true; // Use SSL/TLS
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_fromEmail, _fromName),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = isHtml
                    };
                    mailMessage.To.Add(toEmail);

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                
                throw new Exception($"Không thể gửi email: {ex.Message}", ex);
            }
        }

        // Hàm gửi email thông báo học sinh nghỉ học đến phụ huynh
        public async Task SendAbsenceNotificationAsync(string parentEmail, string studentName, string className, DateTime absenceDate, string reason = null)
        {
            string subject = $"Thông báo học sinh {studentName} nghỉ học";
            string body = $@"
                <p>Kính gửi phụ huynh học sinh {studentName},</p>
                <p>Chúng tôi xin thông báo rằng con bạn đã nghỉ học vào ngày <strong>{absenceDate:dd/MM/yyyy}</strong>.</p>
                <p>Lớp: <strong>{className}</strong></p>
                {(string.IsNullOrEmpty(reason) ? "" : $"<p>Lý do: {reason}</p>")}
                <p>Vui lòng liên hệ với giáo viên chủ nhiệm để biết thêm chi tiết.</p>
                <p>Trân trọng,<br/>Hệ thống quản lý trường học</p>";

            await SendEmailAsync(parentEmail, subject, body, isHtml: true);
        }
    }
}