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
using Infrastructure.Repositories.Interfaces;
using System.Text.Json;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly ITeacherService _teacherService;
        private readonly IAcademicYearRepository _academicYearRepository;

        public AuthController(IUserService userService, ITokenService tokenService, ITeacherService teacherService, IAcademicYearRepository academicYearRepository)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _teacherService = teacherService ?? throw new ArgumentNullException(nameof(teacherService));
            _academicYearRepository = academicYearRepository ?? throw new ArgumentNullException(nameof(academicYearRepository));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest(new { message = "Username and password are required." });
            }

            Console.WriteLine($"Attempting to login with username: {loginDto.Username}");

            var user = await _userService.GetUserByUsernameAsync(loginDto.Username);
            if (user == null)
            {
                Console.WriteLine($"User with username {loginDto.Username} not found.");
                return Unauthorized(new { message = "Invalid username or password." });
            }

            Console.WriteLine($"User found: {user.Username}, Status: {user.Status}");

            if (user.Status == "Deactive")
            {
                Console.WriteLine($"User {user.Username} is deactivated.");
                return Unauthorized(new { message = "Your account is deactivated. Please contact the administrator." });
            }

            // Kiểm tra mật khẩu
            var userFromRepo = await _userService.GetUserByUsernameAsync(loginDto.Username);
            if (userFromRepo == null || !VerifyPassword(loginDto.Password, userFromRepo.PasswordHash))
            {
                Console.WriteLine($"Password verification failed for user {loginDto.Username}.");
                Console.WriteLine($"Stored hash: {userFromRepo?.PasswordHash}");
                return Unauthorized(new { message = "Invalid username or password." });
            }

            Console.WriteLine($"Password verified successfully for user {loginDto.Username}.");

            var userRole = await GetAndValidateUserRole(user.RoleId);
            if (string.IsNullOrEmpty(userRole))
            {
                Console.WriteLine($"Role not found for user {user.Username} with RoleId {user.RoleId}.");
                return StatusCode(500, new { message = "Role not found for this user." });
            }

            Console.WriteLine($"User role: {userRole}");

            var teacher = await _teacherService.GetTeacherByIdAsync(user.UserId);
            string effectiveRole = userRole;
            if (teacher != null && teacher.IsHeadOfDepartment == true)
            {
                effectiveRole = "HeadOfDepartment";
                Console.WriteLine($"User {user.Username} is HeadOfDepartment. Effective role: {effectiveRole}");
            }

            (string tokenString, Dictionary<string, string> tokenPayload) = await _tokenService.GenerateTokenAsync(user, effectiveRole);
            Console.WriteLine($"Token generated for user {user.Username}.");

            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim("sub", user.UserId.ToString()),
                new Claim("email", user.Email ?? ""),
                new Claim("name", user.Username),
                new Claim("role", effectiveRole)
            }, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            Console.WriteLine($"User {user.Username} signed in with cookie authentication.");

            // Lưu thông tin người dùng vào Session
            var userSessionData = new
            {
                UserId = user.UserId,
                Username = user.Username,
                Role = effectiveRole
            };
            HttpContext.Session.SetString("UserSession", JsonSerializer.Serialize(userSessionData));
            Console.WriteLine($"User session data saved: {JsonSerializer.Serialize(userSessionData)}");

            // Lưu năm học vào Session
            var currentAcademicYear = await _academicYearRepository.GetCurrentAcademicYearAsync();
            if (currentAcademicYear == null)
            {
                Console.WriteLine("Current academic year not found.");
                return StatusCode(500, new { message = "Current academic year not found." });
            }
            HttpContext.Session.SetString("AcademicYear", currentAcademicYear.YearName ?? "Unknown");
            Console.WriteLine($"Academic year saved in session: {currentAcademicYear.YearName ?? "Unknown"}");

            Console.WriteLine($"Login successful for user {user.Username}.");
            return Ok(new
            {
                token = tokenString,
                decodedToken = tokenPayload
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Console.WriteLine("User logged out successfully.");
            return Ok(new { message = "Logged out successfully." });
        }

        [HttpPost("assign-role")]
        [Authorize(Roles = "Principal,AdministrativeOfficer")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto assignRoleDto)
        {
            if (assignRoleDto == null || assignRoleDto.UserId <= 0 || assignRoleDto.RoleId <= 0)
            {
                return BadRequest(new { message = "UserId and RoleId are required and must be positive." });
            }

            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(currentUserRole) || !new[] { "Principal", "AdministrativeOfficer" }.Contains(currentUserRole))
            {
                return StatusCode(403, new { message = "You do not have permission to assign roles." });
            }

            var validRoles = new[] { 1, 2, 3, 4, 5, 6 };
            if (!validRoles.Contains(assignRoleDto.RoleId))
            {
                return BadRequest(new { message = "Invalid RoleId." });
            }

            var userToUpdate = await _userService.GetUserByIdAsync(assignRoleDto.UserId);
            if (userToUpdate == null)
            {
                return NotFound(new { message = "User not found." });
            }

            userToUpdate.RoleId = assignRoleDto.RoleId;
            await _userService.UpdateUserAsync(userToUpdate);

            return Ok(new { message = "Role assigned successfully.", userId = assignRoleDto.UserId, newRoleId = assignRoleDto.RoleId });
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

            var hasHomeroomTeacher = await _teacherService.HasHomeroomTeacherAsync(assignHomeroomDto.ClassId, assignHomeroomDto.AcademicYearId);
            if (hasHomeroomTeacher)
            {
                return BadRequest(new { message = $"Class with ID {assignHomeroomDto.ClassId} already has a homeroom teacher in academic year {assignHomeroomDto.AcademicYearId}." });
            }

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

        private bool VerifyPassword(string inputPassword, string storedHash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(inputPassword, storedHash);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verifying password: {ex.Message}");
                return false;
            }
        }

        private async Task<string> GetAndValidateUserRole(int roleId)
        {
            var userRole = await _userService.GetRoleNameByRoleIdAsync(roleId);
            return userRole ?? throw new InvalidOperationException($"Role with ID {roleId} not found.");
        }
    }
}