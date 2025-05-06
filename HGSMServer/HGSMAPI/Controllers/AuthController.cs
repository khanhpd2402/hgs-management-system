using Application.Features.Users.DTOs;
using Application.Features.Users.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
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
            try
            {
                if (loginDto == null || string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
                {
                    Console.WriteLine("Invalid login input data.");
                    return BadRequest("Vui lòng điền đầy đủ tài khoản và mật khẩu.");
                }

                Console.WriteLine("Attempting to login...");
                var user = await _userService.GetUserByUsernameAsync(loginDto.Username);
                if (user == null)
                {
                    Console.WriteLine("User not found.");
                    return Unauthorized("Tài khoản không hợp lệ.");
                }

                Console.WriteLine("Checking user status...");
                if (user.Status == "Không hoạt động")
                {
                    Console.WriteLine("User account is inactive.");
                    return Unauthorized("Tài khoản của bạn không hoạt động. Liên hệ cán bộ văn thư để giải quyết.");
                }

                Console.WriteLine("Verifying password...");
                if (!VerifyPassword(loginDto.Password, user.PasswordHash))
                {
                    Console.WriteLine("Password verification failed.");
                    return Unauthorized("Mật khẩu không đúng.");
                }

                Console.WriteLine("Fetching user role...");
                var userRole = await GetAndValidateUserRole(user.RoleId);
                if (string.IsNullOrEmpty(userRole))
                {
                    Console.WriteLine("Role not found for user.");
                    return StatusCode(500, "Vai trò không hợp lệ.");
                }

                Console.WriteLine("Fetching teacher information...");
                var teacher = await _teacherService.GetTeacherByIdAsync(user.UserId);
                string effectiveRole = userRole;
                if (teacher != null && teacher.IsHeadOfDepartment == true)
                {
                    effectiveRole = "Trưởng bộ môn";
                    Console.WriteLine("User is HeadOfDepartment.");
                }

                Console.WriteLine("Generating token...");
                var (tokenString, tokenPayload) = await _tokenService.GenerateTokenAsync(user);

                Console.WriteLine("Setting up claims identity...");
                var claimsIdentity = new ClaimsIdentity(new[]
                {
                    new Claim("sub", user.UserId.ToString()),
                    new Claim("email", user.Email ?? ""),
                    new Claim("name", user.Username),
                    new Claim("role", effectiveRole)
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                Console.WriteLine("Signing in user...");
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                Console.WriteLine("Saving user session...");
                var userSessionData = new
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Role = effectiveRole
                };
                HttpContext.Session.SetString("UserSession", JsonSerializer.Serialize(userSessionData));

                Console.WriteLine("Login successful.");
                return Ok(new
                {
                    token = tokenString,
                    decodedToken = tokenPayload
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error during login: {ex.Message}");
                return StatusCode(500, "Lỗi khi đăng nhập." + ex.Message);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                Console.WriteLine("Logging out user...");
                HttpContext.Session.Clear();
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok("Đăng xuất thành công.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during logout: {ex.Message}");
                return StatusCode(500, "Lỗi khi đăng xuất.");
            }
        }

        [HttpPost("assign-role")]
        [Authorize(Roles = "Hiệu trưởng,Cán bộ văn thư")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto assignRoleDto)
        {
            try
            {
                if (assignRoleDto == null || assignRoleDto.UserId <= 0 || assignRoleDto.RoleId <= 0)
                {
                    Console.WriteLine("Invalid role assignment data.");
                    return BadRequest("Dữ liệu không hợp lệ.");
                }

                Console.WriteLine("Checking user permission...");
                var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
                if (string.IsNullOrEmpty(currentUserRole) || !new[] { "Hiệu trưởng", "Cán bộ văn thư" }.Contains(currentUserRole))
                {
                    Console.WriteLine("User does not have permission to assign roles.");
                    return Unauthorized("Bạn không có quyền phân công vai trò người dùng.");
                }

                Console.WriteLine("Validating role...");
                var role = await _roleService.GetRoleByIdAsync(assignRoleDto.RoleId);
                if (role == null)
                {
                    Console.WriteLine("Role not found.");
                    return BadRequest("Vai trò không tồn tại.");
                }

                var validRoleNames = new[] { "Cán bộ văn thư", "Trưởng bộ môn", "Phụ huynh", "Hiệu trưởng", "Giáo viên", "Hiệu phó" };
                if (!validRoleNames.Contains(role.RoleName))
                {
                    Console.WriteLine("Invalid role name.");
                    return BadRequest("Vai trò không tồn tại.");
                }

                Console.WriteLine("Fetching user...");
                var userToUpdate = await _userService.GetUserByIdAsync(assignRoleDto.UserId);
                if (userToUpdate == null)
                {
                    Console.WriteLine("User not found.");
                    return NotFound("Không tìm thấy người dùng.");
                }

                Console.WriteLine("Updating user role...");
                var updateUserDto = new UpdateUserDTO
                {
                    UserId = userToUpdate.UserId,
                    Username = userToUpdate.Username,
                    Email = userToUpdate.Email,
                    PhoneNumber = userToUpdate.PhoneNumber,
                    RoleId = assignRoleDto.RoleId
                };
                await _userService.UpdateUserAsync(updateUserDto);

                return Ok("Phân công vai trò người dùng thành công.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error assigning role: {ex.Message}");
                return StatusCode(500, "Lỗi khi phân công vai trò người dùng.");
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
            try
            {
                var userRole = await _userService.GetRoleNameByRoleIdAsync(roleId);
                return userRole ?? throw new InvalidOperationException("Không tìm thấy vai trò.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating user role: {ex.Message}");
                throw;
            }
        }
    }
}