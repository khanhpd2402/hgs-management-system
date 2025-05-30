﻿using Application.Features.Attendances.DTOs;
using Application.Features.Attendances.Interfaces;
using Application.Features.Attendances.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _service;

        public AttendanceController(IAttendanceService service)
        {
            _service = service;
        }

        [HttpGet("weekly")]
        public async Task<IActionResult> GetWeeklyAttendance(
            [FromQuery] int teacherId,
            [FromQuery] int classId,
            [FromQuery] int semesterId,
            [FromQuery] DateOnly weekStart)
        {
            try
            {
                Console.WriteLine("Fetching weekly attendance...");
                var data = await _service.GetWeeklyAttendanceAsync(teacherId, classId, semesterId, weekStart);
                return Ok(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching weekly attendance: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy danh sách điểm danh hàng tuần." + ex.Message);
            }
        }
        [HttpGet("student/{studentId}/week")]
        public async Task<IActionResult> GetAttendanceByStudentAndWeek(int studentId, [FromQuery] string weekStart)
        {
            if (!DateOnly.TryParseExact(weekStart, "dd/MM/yyyy", out var startDate))
                return BadRequest("Ngày bắt đầu tuần không hợp lệ. Định dạng đúng: dd/MM/yyyy");

            try
            {
                var result = await _service.GetWeeklyAttendanceByStudentAsync(studentId, startDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy dữ liệu điểm danh: {ex.Message}");
            }
        }

        [HttpPost("upsert")]
        public async Task<IActionResult> UpsertAttendances(
            [FromQuery] int teacherId,
            [FromQuery] int classId,
            [FromQuery] int semesterId,
            [FromBody] List<AttendanceDto> dtos)
        {
            try
            {
                Console.WriteLine("Upserting attendances...");
                await _service.UpsertAttendancesAsync(teacherId, classId, semesterId, dtos);
                return Ok("Điểm danh thành công.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error upserting attendances: {ex.Message}");
                return StatusCode(500, "Lỗi khi điểm danh." + ex.Message);
            }
        }
        [HttpGet("homeroom-attendance/{teacherId}/{semesterId}")]
        [Authorize(Roles = "Giáo viên,Trưởng bộ môn")]
        public async Task<IActionResult> GetHomeroomAttendance(int teacherId, int semesterId, [FromQuery] DateOnly weekStart)
        {
            try
            {
                Console.WriteLine("Fetching homeroom attendance...");
                var attendances = await _service.GetHomeroomAttendanceAsync(teacherId, semesterId, weekStart);
                return Ok(attendances);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error fetching homeroom attendance: {ex.Message}");
                return BadRequest("Lỗi khi lấy thông tin điểm danh." + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error fetching homeroom attendance: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy thông tin điểm danh." + ex.Message);
            }
        }
    }
}