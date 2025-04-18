﻿using Application.Features.Users.DTOs;
using Application.Features.Users.Interfaces;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly IRoleRepository _roleRepository;
    private readonly ITeacherRepository _teacherRepository; 

    public TokenService(
        IConfiguration configuration,
        IRoleRepository roleRepository,
        ITeacherRepository teacherRepository)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _teacherRepository = teacherRepository ?? throw new ArgumentNullException(nameof(teacherRepository));
    }

    public async Task<(string tokenString, Dictionary<string, string> tokenPayload)> GenerateTokenAsync(UserDTO user)
    {
        var role = await _roleRepository.GetByIdAsync(user.RoleId);
        if (role == null)
            throw new ArgumentException($"Role with ID {user.RoleId} not found.");
        string userRole = role.RoleName;

        var claims = new List<Claim>
        {
            new Claim("sub", user.UserId.ToString()),
            new Claim("email", user.Email ?? ""),
            new Claim("name", user.Username),
            new Claim("role", userRole),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Kiểm tra vai trò và lấy TeacherID nếu không phải Phụ huynh
        if (userRole != "Phụ huynh")
        {
            var teacher = await _teacherRepository.GetByUserIdAsync(user.UserId);
            if (teacher != null)
            {
                claims.Add(new Claim("teacherId", teacher.TeacherId.ToString()));
            }
        }

        var secretKey = _configuration["JWT:SecretKey"];
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("JWT SecretKey is not configured.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(tokenString);
        var payload = jwtToken.Payload;

        var payloadDict = new Dictionary<string, string>();
        foreach (var claim in payload)
        {
            payloadDict[claim.Key] = claim.Value?.ToString() ?? string.Empty;
        }

        return (tokenString, payloadDict);
    }
}