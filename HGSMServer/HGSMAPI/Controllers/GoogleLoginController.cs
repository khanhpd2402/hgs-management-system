using Application.Features.Users.DTOs;
using Application.Features.Users.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

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

        [HttpGet("login")]
        public IActionResult Login(string redirectUrl)
        {
            if (string.IsNullOrEmpty(redirectUrl))
            {
                return BadRequest(new { message = "redirectUrl is required." });
            }

            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("Callback", "GoogleLogin", new { redirectUrl }, Request.Scheme)
            };
            return Challenge(properties, "Google");
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string redirectUrl)
        {
            if (string.IsNullOrEmpty(redirectUrl))
            {
                return BadRequest(new { message = "redirectUrl is required." });
            }

            var authenticateResult = await HttpContext.AuthenticateAsync("Google");
            if (!authenticateResult.Succeeded)
            {
                return Redirect($"{redirectUrl}?error=authentication_failed");
            }

            var claims = authenticateResult.Principal?.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return Redirect($"{redirectUrl}?error=email_not_found");
            }

            var existingUser = (await _userService.GetAllUsersAsync())
                .FirstOrDefault(u => u.Email == email);

            if (existingUser == null)
            {
                return Redirect($"{redirectUrl}?error=email_not_in_system");
            }

            // Lấy và kiểm tra role
            var userRole = await GetAndValidateUserRole(existingUser.RoleId);
            if (userRole == null)
            {
                return Redirect($"{redirectUrl}?error=unauthorized_role");
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

            // Chuyển hướng về redirectUrl với token trong query string
            return Redirect($"{redirectUrl}?token={Uri.EscapeDataString(tokenString)}");
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
}