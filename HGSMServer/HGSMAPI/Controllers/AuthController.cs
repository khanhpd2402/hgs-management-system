using Application.Features.Users.DTOs;
using Application.Features.Users.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using System.Threading.Tasks;
using System.Security.Claims;
using Application.Features.Teachers.DTOs;
using Application.Features.Teachers.Interfaces;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly ITeacherService _teacherService;

        public AuthController(IUserService userService, ITokenService tokenService, ITeacherService teacherService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _teacherService = teacherService ?? throw new ArgumentNullException(nameof(teacherService));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest(new { message = "Username and password are required." });
            }

            var users = await _userService.GetAllUsersAsync();
            var user = users.FirstOrDefault(u => u.Username == loginDto.Username);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            if (!VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            var userRole = await GetAndValidateUserRole(user.RoleId);
            if (string.IsNullOrEmpty(userRole))
            {
                return StatusCode(500, new { message = "Role not found for this user." });
            }

            var (tokenString, tokenPayload) = await _tokenService.GenerateTokenAsync(user, userRole);

            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Role, userRole)
            }, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return Ok(new
            {
                token = tokenString,
                decodedToken = tokenPayload
            });
        }

        [HttpPost("assign-role")]
        [Authorize(Roles = "Principal,AdministrativeOfficer")] // Chỉ Principal và AdministrativeOfficer được phép
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto assignRoleDto)
        {
            if (assignRoleDto == null || assignRoleDto.UserId <= 0 || assignRoleDto.RoleId <= 0)
            {
                return BadRequest(new { message = "UserId and RoleId are required and must be positive." });
            }

            // Lấy role của user hiện tại (người thực hiện phân quyền)
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(currentUserRole) || !new[] { "Principal", "AdministrativeOfficer" }.Contains(currentUserRole))
            {
                return StatusCode(403, new { message = "You do not have permission to assign roles." });
            }

            // Kiểm tra xem RoleId có hợp lệ không (tùy thuộc vào bảng Roles)
            var validRoles = new[] { 1, 2, 3, 4, 5, 6 }; // Danh sách RoleId hợp lệ (tùy chỉnh theo database)
            if (!validRoles.Contains(assignRoleDto.RoleId))
            {
                return BadRequest(new { message = "Invalid RoleId." });
            }

            // Lấy user cần gán role
            var userToUpdate = await _userService.GetUserByIdAsync(assignRoleDto.UserId);
            if (userToUpdate == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Cập nhật RoleId cho user
            userToUpdate.RoleId = assignRoleDto.RoleId;
            await _userService.UpdateUserAsync(userToUpdate);

            return Ok(new { message = "Role assigned successfully.", userId = assignRoleDto.UserId, newRoleId = assignRoleDto.RoleId });
        }

        private bool VerifyPassword(string inputPassword, string storedHash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(inputPassword, storedHash);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<string> GetAndValidateUserRole(int roleId)
        {
            var userRole = await _userService.GetRoleNameByRoleIdAsync(roleId);
            return userRole; // Trả về role mà không kiểm tra, bao gồm tất cả role
        }

        [HttpPost("assign-homeroom")]
        [Authorize(Roles = "Principal,AdministrativeOfficer")]
        public async Task<IActionResult> AssignHomeroom([FromBody] AssignHomeroomDto assignHomeroomDto)
        {
            if (assignHomeroomDto == null || assignHomeroomDto.TeacherId <= 0 || assignHomeroomDto.ClassId <= 0 ||
                assignHomeroomDto.AcademicYearId <= 0 || assignHomeroomDto.SemesterId <= 0)
            {
                return BadRequest(new { message = "All IDs (TeacherId, ClassId, AcademicYearId, SemesterId) are required and must be positive." });
            }

            // Kiểm tra xem lớp đã có giáo viên chủ nhiệm trong năm học này chưa
            var hasHomeroomTeacher = await _teacherService.HasHomeroomTeacherAsync(assignHomeroomDto.ClassId, assignHomeroomDto.AcademicYearId);
            if (hasHomeroomTeacher)
            {
                return BadRequest(new { message = $"Class with ID {assignHomeroomDto.ClassId} already has a homeroom teacher in academic year {assignHomeroomDto.AcademicYearId}." });
            }

            // Kiểm tra xem giáo viên đã được phân công làm chủ nhiệm lớp này chưa
            var isAssigned = await _teacherService.IsHomeroomAssignedAsync(assignHomeroomDto.TeacherId, assignHomeroomDto.ClassId, assignHomeroomDto.AcademicYearId);
            if (isAssigned)
            {
                return BadRequest(new { message = "This teacher is already assigned as homeroom teacher for this class in the specified academic year." });
            }

            try
            {
                await _teacherService.AssignHomeroomAsync(assignHomeroomDto);
                return Ok(new { message = "Homeroom teacher assigned successfully.", teacherId = assignHomeroomDto.TeacherId, classId = assignHomeroomDto.ClassId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while assigning homeroom teacher.", error = ex.Message });
            }
        }
    }
}