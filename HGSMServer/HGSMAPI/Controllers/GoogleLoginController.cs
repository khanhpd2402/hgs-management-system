﻿using Application.Features.Users.DTOs;
using Application.Features.Users.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json; // Thêm namespace này để xử lý JSON

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleLoginController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public GoogleLoginController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
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
            // Authen with scheme "Google"
            var authenticateResult = await HttpContext.AuthenticateAsync("Google");
            if (!authenticateResult.Succeeded)
            {
                return BadRequest(new { message = "Google authentication failed." });
            }

            // Calaim user infor from Google
            var claims = authenticateResult.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { message = "Unable to retrieve email from Google." });
            }

            // Find user in database
            var existingUser = (await _userService.GetAllUsersAsync())
                .FirstOrDefault(u => u.Email == email);

            // if user not exist
            if (existingUser == null)
            {
                return Unauthorized(new { message = "Email not found in the system. Please register first." });
            }

            // if user exist, cont
            UserDTO userDto = existingUser;

            // Create JWT token
            var (tokenString, tokenPayload) = GenerateJwtToken(userDto);

            // Login by cookie 
            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, userDto.Username),
                new Claim(ClaimTypes.Email, userDto.Email)
            }, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            
            // Return token as JSON
            return Ok(new
            {
                token = tokenString, // Origin token
                decodedToken = tokenPayload // Decoded token
            });
        }

        private (string tokenString, object tokenPayload) GenerateJwtToken(UserDTO user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // decode token
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(tokenString);
            var payload = jwtToken.Payload;

           
            // change payload to dictionary to return as JSON
            var payloadDict = payload.ToDictionary(
                claim => claim.Key,
                claim => claim.Value?.ToString()
            );

            return (tokenString, payloadDict);
        }
    }
}