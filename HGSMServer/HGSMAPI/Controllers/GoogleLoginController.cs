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

        [HttpPost("credential")]
        public async Task<IActionResult> Credential([FromBody] GoogleCredentialDto credentialDto)
        {
            if (string.IsNullOrEmpty(credentialDto.Credential))
            {
                Console.WriteLine("Credential is null or empty.");
                return BadRequest(new { message = "Credential không hợp lệ." });
            }

            try
            {
                Console.WriteLine($"Received credential: {credentialDto.Credential}");
                Console.WriteLine($"Configured ClientId: {_configuration["Google:ClientId"]}");

                // Thiết lập validation settings
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _configuration["Google:ClientId"] },
                    // Chỉ thêm HostedDomain nếu muốn giới hạn domain
                    HostedDomain = _configuration["Google:HostedDomain"] // Ví dụ: "fpt.edu.vn"
                };

                // Validate ID token
                var payload = await GoogleJsonWebSignature.ValidateAsync(credentialDto.Credential, settings);
                Console.WriteLine($"Validated payload: Email={payload.Email}, Aud={payload.Audience}, Iss={payload.Issuer}, Exp={payload.ExpirationTimeSeconds}");

                // Kiểm tra email
                if (string.IsNullOrEmpty(payload.Email))
                {
                    Console.WriteLine("Email is missing in token payload.");
                    return BadRequest(new { message = "Không thể lấy email từ Google." });
                }

                // Kiểm tra email trong DB
                var existingUser = (await _userService.GetAllUsersAsync())
                    .FirstOrDefault(u => u.Email == payload.Email);
                if (existingUser == null)
                {
                    Console.WriteLine($"Email {payload.Email} not found in database.");
                    return StatusCode(403, new { message = "Email không có trong hệ thống. Liên hệ cán bộ văn thư để giải quyết." });
                }

                // Kiểm tra role
                var userRole = await GetAndValidateUserRole(existingUser.RoleId);
                if (userRole == null)
                {
                    Console.WriteLine($"Invalid role for user with email {payload.Email}.");
                    return StatusCode(403, new { message = "Bạn không có quyền truy cập vào mục này." });
                }

                // Tạo token
                var (tokenString, tokenPayload) = await _tokenService.GenerateTokenAsync(existingUser);
                Console.WriteLine($"Generated token for user {existingUser.Username}");

                // Đăng nhập bằng cookie
                var claimsIdentity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, existingUser.Username),
                    new Claim(ClaimTypes.Email, existingUser.Email ?? ""),
                    new Claim(ClaimTypes.Role, userRole)
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                Console.WriteLine("User signed in with cookie authentication.");

                return Ok(new
                {
                    token = tokenString,
                    decodedToken = tokenPayload
                });
            }
            catch (InvalidJwtException ex)
            {
                Console.WriteLine($"Invalid JWT: {ex.Message}");
                return BadRequest(new { message = "Token Google không hợp lệ.", details = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
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