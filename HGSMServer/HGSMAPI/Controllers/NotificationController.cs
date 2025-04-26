using Application.Features.Notification.DTOs;
using Application.Features.Teachers.Interfaces;
using Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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
                    return BadRequest(new { message = "Tiêu đề và nội dung tin nhắn không được để trống." });
                }

                if (request.TeacherIds == null || !request.TeacherIds.Any())
                {
                    return BadRequest(new { message = "Danh sách ID giáo viên không được để trống." });
                }

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
                    return BadRequest(new { message = "Không tìm thấy email hợp lệ nào để gửi tin nhắn." });
                }

                await _emailService.SendEmailToMultipleRecipientsAsync(
                    toEmails: teacherEmails,
                    subject: request.Subject,
                    body: request.Body,
                    isHtml: request.IsHtml
                );

                return Ok(new { message = "Gửi tin nhắn thành công!" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã xảy ra lỗi trong quá trình gửi tin nhắn." });
            }
        }
    }
}