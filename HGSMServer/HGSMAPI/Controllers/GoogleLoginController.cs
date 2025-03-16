using Application.Features.Users.DTOs;
using Application.Features.Users.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Security.Claims;

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
        public IActionResult Login()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("Callback", "GoogleLogin", null, Request.Scheme)
            };
            return Challenge(properties, "Google");
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync("Google");
            if (!authenticateResult.Succeeded)
            {
                return BadRequest(new { message = "Google authentication failed." });
            }

            var claims = authenticateResult.Principal?.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { message = "Unable to retrieve email from Google." });
            }

            var existingUser = (await _userService.GetAllUsersAsync())
                .FirstOrDefault(u => u.Email == email);

            if (existingUser == null)
            {
                return StatusCode(403, new { message = "Access denied. Please contact your admin before logging in." });
            }

            // Lấy và kiểm tra role
            var userRole = await GetAndValidateUserRole(existingUser.RoleId);
            if (userRole == null)
            {
                return StatusCode(403, new { message = "Access denied. Insufficient permissions." });
            }

            // Tạo token
            var (tokenString, tokenPayload) = await _tokenService.GenerateTokenAsync(existingUser, userRole);

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

        private async Task<string> GetAndValidateUserRole(int roleId)
        {
            var userRole = await _userService.GetRoleNameByRoleIdAsync(roleId);
            if (string.IsNullOrEmpty(userRole))
            {
                return null;
            }

            var allowedRoles = new[] { "Principal", "VicePrincipal", "HeadOfDepartment", "AdministrativeOfficer" };
            return allowedRoles.Contains(userRole) ? userRole : null;
        }
    }
}