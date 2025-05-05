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
            if (string.IsNullOrWhiteSpace(toEmail))
            {
                throw new ArgumentException("Email người nhận không được để trống.");
            }

            if (!IsValidEmail(toEmail))
            {
                throw new ArgumentException($"Email không hợp lệ: {toEmail}.");
            }

            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentException("Tiêu đề email không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(body))
            {
                throw new ArgumentException("Nội dung email không được để trống.");
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
            catch (SmtpException ex)
            {
                throw new InvalidOperationException($"Không thể gửi email đến {toEmail}: Lỗi SMTP.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Không thể gửi email đến {toEmail}: Lỗi hệ thống.", ex);
            }
        }

        public async Task SendEmailToMultipleRecipientsAsync(List<string> toEmails, string subject, string body, bool isHtml = false)
        {
            if (toEmails == null || !toEmails.Any())
            {
                throw new ArgumentException("Danh sách email người nhận không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentException("Tiêu đề email không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(body))
            {
                throw new ArgumentException("Nội dung email không được để trống.");
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
                        throw new ArgumentException("Không có email người nhận hợp lệ nào để gửi.");
                    }

                    await smtpClient.SendMailAsync(mailMessage);
                    Console.WriteLine($"Đã gửi email thành công đến {mailMessage.To.Count} người nhận.");
                }
            }
            catch (SmtpException ex)
            {
                throw new InvalidOperationException("Không thể gửi email đến nhiều người nhận: Lỗi SMTP.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể gửi email đến nhiều người nhận: Lỗi hệ thống.", ex);
            }
        }

        public async Task SendAbsenceNotificationAsync(string parentEmail, string studentName, string className, DateTime absenceDate, string reason = null, string teacherName = null, string teacherEmail = null, string teacherPhone = null)
        {
            string subject = $"Thông báo tình trạng điểm danh học sinh {studentName}";
            string body = GetAbsenceNotificationBody(studentName, className, absenceDate, reason, teacherName, teacherEmail, teacherPhone);

            await SendEmailAsync(parentEmail, subject, body, isHtml: true);
        }

        private string GetAbsenceNotificationBody(string studentName, string className, DateTime absenceDate, string reason = null, string teacherName = null, string teacherEmail = null, string teacherPhone = null)
        {
            var contactInfo = "";
            if (!string.IsNullOrEmpty(teacherName))
            {
                contactInfo = $"<p>Vui lòng liên hệ giáo viên: <strong>{teacherName}</strong>";
                if (!string.IsNullOrEmpty(teacherEmail))
                    contactInfo += $", Email: <strong>{teacherEmail}</strong>";
                if (!string.IsNullOrEmpty(teacherPhone))
                    contactInfo += $", SĐT: <strong>{teacherPhone}</strong>";
                contactInfo += " để biết thêm chi tiết.</p>";
            }

            return $@"
        <p>Kính gửi phụ huynh học sinh {studentName},</p>
        <p>Chúng tôi xin thông báo rằng học sinh đã nghỉ học vào ngày <strong>{absenceDate:dd/MM/yyyy}</strong>.</p>
        <p>Lớp: <strong>{className}</strong></p>
        {(string.IsNullOrEmpty(reason) ? "" : $"<p>Lý do: {reason}</p>")}
        {contactInfo}
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
        public async Task SendAdminPasswordChangeNotificationAsync(string userEmail, string username, string newPassword)
        {
            string subject = "Thông báo: Mật khẩu tài khoản của bạn đã được thay đổi";
            string body = $@"
                <p>Kính gửi thầy/cô với tài khoản: {username},</p>
                <p>Mật khẩu tài khoản của bạn đã được quản trị viên thay đổi vào ngày <strong>{DateTime.Now:dd/MM/yyyy HH:mm}</strong>.</p>
                <p><strong>Mật khẩu mới:</strong> {newPassword}</p>
                <p><strong>Chú ý:</strong> Đây là thông tin bảo mật, vui lòng không chia sẻ mật khẩu này với bất kỳ ai. Để đảm bảo an toàn, hãy đổi mật khẩu ngay sau khi đăng nhập.</p>
                <p>Nếu bạn không yêu cầu thay đổi này, vui lòng liên hệ ngay với quản trị viên qua email support@haigiangschool.edu.vn.</p>
                <p>Trân trọng,<br/>Trường THCS Hải Giang</p>";

            await SendEmailAsync(userEmail, subject, body, isHtml: true);
        }
        public async Task SendUserStatusChangeNotificationAsync(string userEmail, string fullName, string newStatus)
        {
            string subject = "Thông báo: Trạng thái tài khoản của bạn đã được cập nhật";
            string body = $@"
                <p>Kính gửi thầy/cô {fullName},</p>
                <p>Trạng thái tài khoản của bạn đã được quản trị viên cập nhật vào ngày <strong>{DateTime.Now:dd/MM/yyyy HH:mm}</strong>.</p>
                <p><strong>Trạng thái mới:</strong> {newStatus}</p>
                <p>Nếu bạn có thắc mắc về sự thay đổi này, vui lòng liên hệ với quản trị viên qua email support@haigiangschool.edu.vn.</p>
                <p>Trân trọng,<br/>Trường THCS Hải Giang</p>";

            await SendEmailAsync(userEmail, subject, body, isHtml: true);
        }
        public async Task SendForgotPasswordNotificationAsync(string userEmail, string username, string newPassword)
        {
            string subject = "Yêu cầu đặt lại mật khẩu tài khoản của bạn";
            string body = $@"
                <p>Kính gửi {username},</p>
                <p>Chúng tôi đã nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn vào ngày <strong>{DateTime.Now:dd/MM/yyyy HH:mm}</strong>.</p>
                <p><strong>Mật khẩu mới:</strong> {newPassword}</p>
                <p><strong>Chú ý:</strong> Đây là thông tin bảo mật, vui lòng không chia sẻ mật khẩu này với bất kỳ ai. Để đảm bảo an toàn, hãy đổi mật khẩu ngay sau khi đăng nhập.</p>
                <p>Nếu bạn không gửi yêu cầu này, vui lòng liên hệ ngay với chúng tôi qua email support@haigiangschool.edu.vn.</p>
                <p>Trân trọng,<br/>Trường THCS Hải Giang</p>";

            await SendEmailAsync(userEmail, subject, body, isHtml: true);
        }
    }
}