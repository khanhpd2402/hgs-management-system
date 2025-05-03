using Application.Features.Users.Interfaces;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleLoginController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;

        public GoogleLoginController(IUserService userService, ITokenService tokenService, IConfiguration configuration)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        // Endpoint mới để nhận credential từ frontend
        [HttpPost("credential")]
        public async Task<IActionResult> Credential([FromBody] GoogleCredentialDto credentialDto)
        {
            if (string.IsNullOrEmpty(credentialDto.Credential))
            {
                return BadRequest(new { message = "Credential không hợp lệ." });
            }

            try
            {
                // Validate Google ID token
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _configuration["Google:ClientId"] } // Lấy Client ID từ appsettings.json
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(credentialDto.Credential, settings);
                var email = payload.Email;

                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest(new { message = "Không thể lấy email từ Google." });
                }

                // Kiểm tra email trong DB
                var existingUser = (await _userService.GetAllUsersAsync())
                    .FirstOrDefault(u => u.Email == email);

                if (existingUser == null)
                {
                    return StatusCode(403, new { message = "Email không có trong hệ thống. Liên hệ cán bộ văn thư để giải quyết." });
                }

                // Kiểm tra role
                var userRole = await GetAndValidateUserRole(existingUser.RoleId);
                if (userRole == null)
                {
                    return StatusCode(403, new { message = "Bạn không có quyền truy cập vào mục này." });
                }

                // Tạo token
                var (tokenString, tokenPayload) = await _tokenService.GenerateTokenAsync(existingUser);

                // Đăng nhập bằng cookie
                var claimsIdentity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, existingUser.Username),
                    new Claim(ClaimTypes.Email, existingUser.Email ?? ""),
                    new Claim(ClaimTypes.Role, userRole)
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return Ok(new
                {
                    token = tokenString,
                    decodedToken = tokenPayload
                });
            }
            catch (InvalidJwtException)
            {
                return BadRequest(new { message = "Token Google không hợp lệ." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi khi xử lý đăng nhập: {ex.Message}" });
            }
        }

        private async Task<string> GetAndValidateUserRole(int roleId)
        {
            var userRole = await _userService.GetRoleNameByRoleIdAsync(roleId);
            if (string.IsNullOrEmpty(userRole))
            {
                return null;
            }

            var allowedRoles = new[] { "Hiệu trưởng", "Hiệu phó", "Trưởng bộ môn", "Giáo viên", "Cán bộ văn thư" };
            return allowedRoles.Contains(userRole) ? userRole : null;
        }
    }

    public class GoogleCredentialDto
    {
        public string Credential { get; set; }
    }
}