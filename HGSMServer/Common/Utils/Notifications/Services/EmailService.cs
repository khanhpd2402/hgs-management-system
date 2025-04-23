using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Common.Utils.Notifications.Services
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

        // Hàm gửi email bất đồng bộ cho một người nhận
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

        // Hàm gửi email bất đồng bộ cho nhiều người nhận
        public async Task SendEmailToMultipleRecipientsAsync(List<string> toEmails, string subject, string body, bool isHtml = false)
        {
            if (toEmails == null || !toEmails.Any())
                throw new ArgumentException("Danh sách email người nhận không được rỗng.");

            try
            {
                using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_fromEmail, _fromName),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = isHtml
                    };

                    // Thêm tất cả email vào danh sách người nhận
                    foreach (var email in toEmails)
                    {
                        if (!string.IsNullOrWhiteSpace(email))
                            mailMessage.To.Add(email);
                    }

                    if (mailMessage.To.Count == 0)
                        throw new Exception("Không có email người nhận hợp lệ.");

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể gửi email đến nhiều người nhận: {ex.Message}", ex);
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
                <p>Trân trọng,<br/>Trường THCS Hải Giang</p>";

            await SendEmailAsync(parentEmail, subject, body, isHtml: true);
        }

        // Hàm gửi email thông báo kế hoạch bài giảng đến giáo viên
        public async Task SendLessonPlanNotificationAsync(string teacherEmail, string teacherName, string planTitle, string subjectName, int semesterId, DateTime? startDate, DateTime? endDate)
        {
            string subject = $"Thông báo: Bạn được giao kế hoạch bài giảng mới";
            string body = $@"
                <p>Kính gửi thầy/cô {teacherName},</p>
                <p>Bạn đã được giao một kế hoạch bài giảng mới với các thông tin sau:</p>
                <p><strong>Tiêu đề:</strong> {planTitle}</p>
                <p><strong>Môn học:</strong> {subjectName}</p>
                <p><strong>Học kỳ:</strong> {semesterId}</p>
                <p><strong>Ngày bắt đầu:</strong> {(startDate.HasValue ? startDate.Value.ToString("dd/MM/yyyy") : "Chưa xác định")}</p>
                <p><strong>Hạn hoàn thành:</strong> {(endDate.HasValue ? endDate.Value.ToString("dd/MM/yyyy") : "Chưa xác định")}</p>
                <p>Vui lòng truy cập hệ thống để xem chi tiết và bắt đầu thực hiện.</p>
                <p>Trân trọng,<br/>Trường THCS Hải Giang</p>";

            await SendEmailAsync(teacherEmail, subject, body, isHtml: true);
        }
    }
}