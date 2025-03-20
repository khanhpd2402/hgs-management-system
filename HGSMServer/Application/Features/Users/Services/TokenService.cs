using Application.Features.Users.DTOs;
using Application.Features.Users.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<(string tokenString, Dictionary<string, string> tokenPayload)> GenerateTokenAsync(UserDTO user, string userRole)
        {
            var claims = new List<Claim>
            {
                new Claim("sub", user.UserId.ToString()), // Sử dụng "sub" thay vì NameIdentifier
                new Claim("email", user.Email ?? ""),
                new Claim("name", user.Username),
                new Claim("role", userRole) // Sử dụng "role" thay vì ClaimTypes.Role
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

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(tokenString);
            var payload = jwtToken.Payload;

            var payloadDict = payload.ToDictionary(claim => claim.Key, claim => claim.Value?.ToString());

            return (tokenString, payloadDict);
        }
    }
}