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
using Application.Features.Role.Interfaces; 

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
        private readonly IRoleService _roleService; 

        public AuthController(
            IUserService userService,
            ITokenService tokenService,
            ITeacherService teacherService,
            IAcademicYearRepository academicYearRepository,
            IRoleService roleService) 
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _teacherService = teacherService ?? throw new ArgumentNullException(nameof(teacherService));
            _academicYearRepository = academicYearRepository ?? throw new ArgumentNullException(nameof(academicYearRepository));
            _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
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

            if (user.Status == "Không hoạt động")
            {
                Console.WriteLine($"User {user.Username} Không hoạt động.");
                return Unauthorized(new { message = "Your account is deactivated. Please contact the administrator." });
            }

            // Kiểm tra mật khẩu
            if (!VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                Console.WriteLine($"Password verification failed for user {loginDto.Username}.");
                Console.WriteLine($"Stored hash: {user.PasswordHash}");
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

            // Tạo token (chỉ truyền UserDTO, không cần effectiveRole)
            var (tokenString, tokenPayload) = await _tokenService.GenerateTokenAsync(user);
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
        [Authorize(Roles = "Hiệu trưởng,Cán bộ văn thư")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto assignRoleDto)
        {
            if (assignRoleDto == null || assignRoleDto.UserId <= 0 || assignRoleDto.RoleId <= 0)
            {
                return BadRequest(new { message = "UserId and RoleId are required and must be positive." });
            }

            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(currentUserRole) || !new[] { "Hiệu trưởng", "Cán bộ văn thư" }.Contains(currentUserRole))
            {
                return StatusCode(403, new { message = "You do not have permission to assign roles." });
            }

            // Kiểm tra vai trò hợp lệ dựa trên RoleName
            var role = await _roleService.GetRoleByIdAsync(assignRoleDto.RoleId);
            if (role == null)
            {
                return BadRequest(new { message = "Invalid RoleId. Role does not exist." });
            }

            var validRoleNames = new[] { "Cán bộ văn thư", "Trưởng bộ môn", "Phụ huynh", "Hiệu trưởng", "Giáo viên", "Hiệu phó" };
            if (!validRoleNames.Contains(role.RoleName))
            {
                return BadRequest(new { message = $"Invalid role. RoleName must be one of: {string.Join(", ", validRoleNames)}." });
            }

            var userToUpdate = await _userService.GetUserByIdAsync(assignRoleDto.UserId);
            if (userToUpdate == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Ánh xạ từ UserDTO sang UpdateUserDTO
            var updateUserDto = new UpdateUserDTO
            {
                UserId = userToUpdate.UserId,
                Username = userToUpdate.Username,
                Email = userToUpdate.Email,
                PhoneNumber = userToUpdate.PhoneNumber,
                RoleId = assignRoleDto.RoleId
            };

            await _userService.UpdateUserAsync(updateUserDto);

            return Ok(new { message = "Role assigned successfully.", userId = assignRoleDto.UserId, newRoleId = assignRoleDto.RoleId, newRoleName = role.RoleName });
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