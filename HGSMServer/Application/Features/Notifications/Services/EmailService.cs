using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = false)
        {
            if (!IsValidEmail(toEmail))
            {
                Console.WriteLine($"Email không hợp lệ: {toEmail}. Bỏ qua việc gửi email.");
                return;
            }

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
                    mailMessage.To.Add(toEmail);

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Không thể gửi email đến {toEmail}: {ex.Message}");
            }
        }

        public async Task SendEmailToMultipleRecipientsAsync(List<string> toEmails, string subject, string body, bool isHtml = false)
        {
            if (toEmails == null || !toEmails.Any())
            {
                Console.WriteLine("Danh sách email người nhận rỗng. Bỏ qua việc gửi email.");
                return;
            }

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

                    foreach (var email in toEmails)
                    {
                        if (IsValidEmail(email))
                        {
                            mailMessage.To.Add(email);
                        }
                        else
                        {
                            Console.WriteLine($"Email không hợp lệ: {email}. Bỏ qua email này.");
                        }
                    }

                    if (mailMessage.To.Count == 0)
                    {
                        Console.WriteLine("Không có email người nhận hợp lệ nào để gửi. Bỏ qua việc gửi email.");
                        return;
                    }

                    await smtpClient.SendMailAsync(mailMessage);
                    Console.WriteLine($"Đã gửi email thành công đến {mailMessage.To.Count} người nhận.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Không thể gửi email đến nhiều người nhận: {ex.Message}");
            }
        }
        public async Task SendAbsenceNotificationAsync(string parentEmail, string studentName, string className, DateTime absenceDate, string reason = null)
        {
            string subject = $"Thông báo tình trạng điểm danh học sinh {studentName}c";
            string body = GetAbsenceNotificationBody(studentName, className, absenceDate, reason);

            await SendEmailAsync(parentEmail, subject, body, isHtml: true);
        }

        private string GetAbsenceNotificationBody(string studentName, string className, DateTime absenceDate, string reason = null)
        {
            return $@"
                <p>Kính gửi phụ huynh học sinh {studentName},</p>
                <p>Chúng tôi xin thông báo rằng anh/chị đã nghỉ học vào ngày <strong>{absenceDate:dd/MM/yyyy}</strong>.</p>
                <p>Lớp: <strong>{className}</strong></p>
                {(string.IsNullOrEmpty(reason) ? "" : $"<p>Lý do: {reason}</p>")}
                <p>Vui lòng liên hệ với giáo viên chủ nhiệm để biết thêm chi tiết.</p>
                <p>Trân trọng,<br/>Trường THCS Hải Giang</p>";
        }

        public async Task SendLessonPlanNotificationAsync(string teacherEmail, string teacherName, string planTitle, string subjectName, int semesterId, DateTime? startDate, DateTime? endDate)
        {
            string subject = $"Thông báo: Bạn được giao kế hoạch bài giảng mới";
            string body = GetLessonPlanNotificationBody(teacherName, planTitle, subjectName, semesterId, startDate, endDate);

            await SendEmailAsync(teacherEmail, subject, body, isHtml: true);
        }

        private string GetLessonPlanNotificationBody(string teacherName, string planTitle, string subjectName, int semesterId, DateTime? startDate, DateTime? endDate)
        {
            return $@"
                <p>Kính gửi thầy/cô {teacherName},</p>
                <p>Bạn đã được giao một kế hoạch bài giảng mới với các thông tin sau:</p>
                <p><strong>Tiêu đề:</strong> {planTitle}</p>
                <p><strong>Môn học:</strong> {subjectName}</p>
                <p><strong>Học kỳ:</strong> {semesterId}</p>
                <p><strong>Ngày bắt đầu:</strong> {(startDate.HasValue ? startDate.Value.ToString("dd/MM/yyyy") : "Chưa xác định")}</p>
                <p><strong>Hạn hoàn thành:</strong> {(endDate.HasValue ? endDate.Value.ToString("dd/MM/yyyy") : "Chưa xác định")}</p>
                <p>Vui lòng truy cập hệ thống để xem chi tiết và bắt đầu thực hiện.</p>
                <p>Trân trọng,<br/>Trường THCS Hải Giang</p>";
        }
        public async Task SendLessonPlanStatusUpdateAsync(string teacherEmail, string teacherName, string planTitle, string subjectName, int semesterId, string status, string feedback = null)
        {
            string subject = $"Cập nhật trạng thái kế hoạch bài giảng: {planTitle}";
            string body = GetLessonPlanStatusUpdateBody(teacherName, planTitle, subjectName, semesterId, status, feedback);

            await SendEmailAsync(teacherEmail, subject, body, isHtml: true);
        }

        private string GetLessonPlanStatusUpdateBody(string teacherName, string planTitle, string subjectName, int semesterId, string status, string feedback = null)
        {
            return $@"
                <p>Kính gửi thầy/cô {teacherName},</p>
                <p>Kế hoạch bài giảng của bạn đã được cập nhật trạng thái:</p>
                <p><strong>Tiêu đề:</strong> {planTitle}</p>
                <p><strong>Môn học:</strong> {subjectName}</p>
                <p><strong>Học kỳ:</strong> {semesterId}</p>
                <p><strong>Trạng thái:</strong> {status}</p>
                {(string.IsNullOrEmpty(feedback) ? "" : $"<p><strong>Phản hồi:</strong> {feedback}</p>")}
                <p>Vui lòng truy cập hệ thống để xem chi tiết.</p>
                <p>Trân trọng,<br/>Trường THCS Hải Giang</p>";
        }

        public async Task SendExamProposalStatusUpdateAsync(string teacherEmail, string planTitle, string subjectName, int grade, int semesterId, string status, string feedback = null)
        {
            string subject = $"Cập nhật trạng thái đề thi: {planTitle}";
            string body = GetExamProposalStatusUpdateBody(planTitle, subjectName, grade, semesterId, status, feedback);

            await SendEmailAsync(teacherEmail, subject, body, isHtml: true);
        }

        private string GetExamProposalStatusUpdateBody(string planTitle, string subjectName, int grade, int semesterId, string status, string feedback = null)
        {
            return $@"
                <p>Kính gửi thầy/cô,</p>
                <p>Đề thi của bạn đã được cập nhật trạng thái:</p>
                <p><strong>Tiêu đề:</strong> {planTitle}</p>
                <p><strong>Môn học:</strong> {subjectName}</p>
                <p><strong>Khối:</strong> {grade}</p>
                <p><strong>Học kỳ:</strong> {semesterId}</p>
                <p><strong>Trạng thái:</strong> {status}</p>
                {(string.IsNullOrEmpty(feedback) ? "" : $"<p><strong>Phản hồi:</strong> {feedback}</p>")}
                <p>Vui lòng truy cập hệ thống để xem chi tiết.</p>
                <p>Trân trọng,<br/>Trường THCS Hải Giang</p>";
        }
    }
}