using Application.Features.Notification.DTOs;
using Application.Features.Teachers.Interfaces;
using Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly ITeacherService _teacherService;

        public NotificationController(EmailService emailService, ITeacherService teacherService)
        {
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _teacherService = teacherService ?? throw new ArgumentNullException(nameof(teacherService));
        }

        [HttpPost("send")]
        [Authorize(Roles = "Hiệu trưởng,Hiệu phó,Trưởng bộ môn,Cán bộ văn thư")]
        public async Task<IActionResult> SendNotification([FromBody] SendNotificationRequestDto request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Subject) || string.IsNullOrWhiteSpace(request.Body))
                {
                    Console.WriteLine("Invalid notification data.");
                    return BadRequest("Tiêu đề và nội dung tin nhắn không được để trống.");
                }

                if (request.TeacherIds == null || !request.TeacherIds.Any())
                {
                    Console.WriteLine("Empty teacher IDs list.");
                    return BadRequest("Danh sách ID giáo viên không được để trống.");
                }

                Console.WriteLine("Sending notification...");
                var teacherEmails = new List<string>();
                foreach (var teacherId in request.TeacherIds)
                {
                    try
                    {
                        var email = await _teacherService.GetEmailByTeacherIdAsync(teacherId);
                        if (!string.IsNullOrWhiteSpace(email))
                        {
                            teacherEmails.Add(email);
                        }
                    }
                    catch (KeyNotFoundException)
                    {
                        continue;
                    }
                }

                if (!teacherEmails.Any())
                {
                    Console.WriteLine("No valid emails found.");
                    return BadRequest("Không tìm thấy email hợp lệ để gửi tin nhắn.");
                }

                await _emailService.SendEmailToMultipleRecipientsAsync(
                    toEmails: teacherEmails,
                    subject: request.Subject,
                    body: request.Body,
                    isHtml: request.IsHtml
                );

                return Ok("Gửi tin nhắn thành công.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error sending notification: {ex.Message}");
                return BadRequest("Lỗi khi gửi tin nhắn.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error sending notification: {ex.Message}");
                return BadRequest("Lỗi khi gửi tin nhắn.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error sending notification: {ex.Message}");
                return StatusCode(500, "Lỗi khi gửi tin nhắn.");
            }
        }
    }
}