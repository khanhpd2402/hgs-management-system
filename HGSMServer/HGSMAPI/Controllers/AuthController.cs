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
                Console.WriteLine("Invalid login input data.");
                return BadRequest(new { message = "Vui lòng điền đầy đủ tài khoản và mật khẩu." });
            }

            Console.WriteLine("Attempting to login...");

            var user = await _userService.GetUserByUsernameAsync(loginDto.Username);
            if (user == null)
            {
                Console.WriteLine("User not found.");
                return Unauthorized(new { message = "Tài khoản không hợp lệ." });
            }

            Console.WriteLine("Checking user status...");

            if (user.Status == "Không hoạt động")
            {
                Console.WriteLine("User account is inactive.");
                return Unauthorized(new { message = "Tài khoản của bạn không hoạt động. Liên hệ cán bộ văn thư để giải quyết." });
            }

            if (!VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                Console.WriteLine("Password verification failed.");
                return Unauthorized(new { message = "Mật khẩu không đúng." });
            }

            Console.WriteLine("Password verified successfully.");

            var userRole = await GetAndValidateUserRole(user.RoleId);
            if (string.IsNullOrEmpty(userRole))
            {
                Console.WriteLine("Role not found for user.");
                return StatusCode(500, new { message = "Vai trò không hợp lệ." });
            }

            Console.WriteLine("Fetching teacher information...");

            var teacher = await _teacherService.GetTeacherByIdAsync(user.UserId);
            string effectiveRole = userRole;
            if (teacher != null && teacher.IsHeadOfDepartment == true)
            {
                effectiveRole = "Trưởng bộ môn";
                Console.WriteLine("User is HeadOfDepartment.");
            }

            var (tokenString, tokenPayload) = await _tokenService.GenerateTokenAsync(user);
            Console.WriteLine("Token generated successfully.");

            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim("sub", user.UserId.ToString()),
                new Claim("email", user.Email ?? ""),
                new Claim("name", user.Username),
                new Claim("role", effectiveRole)
            }, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            Console.WriteLine("User signed in with cookie authentication.");

            var userSessionData = new
            {
                UserId = user.UserId,
                Username = user.Username,
                Role = effectiveRole
            };
            HttpContext.Session.SetString("UserSession", JsonSerializer.Serialize(userSessionData));
            Console.WriteLine("User session data saved.");

            Console.WriteLine("Login successful.");
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
            return Ok(new { message = "Đăng xuất thành công." });
        }

        [HttpPost("assign-role")]
        [Authorize(Roles = "Hiệu trưởng,Cán bộ văn thư")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto assignRoleDto)
        {
            if (assignRoleDto == null || assignRoleDto.UserId <= 0 || assignRoleDto.RoleId <= 0)
            {
                Console.WriteLine("Invalid role assignment data.");
                return BadRequest(new { message = "Dữ liệu không hợp lệ." });
            }

            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(currentUserRole) || !new[] { "Hiệu trưởng", "Cán bộ văn thư" }.Contains(currentUserRole))
            {
                Console.WriteLine("User does not have permission to assign roles.");
                return StatusCode(403, new { message = "Bạn không có quyền phân công vai trò người dùng." });
            }

            var role = await _roleService.GetRoleByIdAsync(assignRoleDto.RoleId);
            if (role == null)
            {
                Console.WriteLine("Role not found.");
                return BadRequest(new { message = "Vai trò không tồn tại." });
            }

            var validRoleNames = new[] { "Cán bộ văn thư", "Trưởng bộ môn", "Phụ huynh", "Hiệu trưởng", "Giáo viên", "Hiệu phó" };
            if (!validRoleNames.Contains(role.RoleName))
            {
                Console.WriteLine("Invalid role name.");
                return BadRequest(new { message = "Vai trò không tồn tại." });
            }

            var userToUpdate = await _userService.GetUserByIdAsync(assignRoleDto.UserId);
            if (userToUpdate == null)
            {
                Console.WriteLine("User not found.");
                return NotFound(new { message = "Không tìm thấy người dùng." });
            }

            var updateUserDto = new UpdateUserDTO
            {
                UserId = userToUpdate.UserId,
                Username = userToUpdate.Username,
                Email = userToUpdate.Email,
                PhoneNumber = userToUpdate.PhoneNumber,
                RoleId = assignRoleDto.RoleId
            };

            await _userService.UpdateUserAsync(updateUserDto);

            return Ok(new { message = "Phân công vai trò người dùng thành công.", userId = assignRoleDto.UserId, newRoleId = assignRoleDto.RoleId, newRoleName = role.RoleName });
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
            return userRole ?? throw new InvalidOperationException("Không tìm thấy vai trò.");
        }
    }
}