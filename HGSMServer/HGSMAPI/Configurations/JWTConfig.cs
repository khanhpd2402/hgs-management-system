using Domain.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HGSMAPI.Configurations
{
    public class JWTConfig
    {
        public static string CreateToken(User u, IConfiguration configuration)
        {
            var secretKey = configuration["JWT:SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            string role = u.Role.RoleName;
            var claims = new Claim[] {
                        new(ClaimTypes.Role, role),
                        new("UserId", u.UserId.ToString())
                    };
            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: credentials
                );
            var tokenHandler = new JwtSecurityTokenHandler();
            string access_token = tokenHandler.WriteToken(token);

            return access_token;

        }

        public static ClaimsPrincipal ValidateToken(string accessToken, IConfiguration configuration)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"])),
                    ValidateIssuer = false, // Set this to true and provide the valid issuer if you want to validate the issuer
                    ValidateAudience = false, // Set this to true and provide the valid audience if you want to validate the audience
                };

                // Validate the token
                ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(accessToken, validationParameters, out var validatedToken);
                return claimsPrincipal;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }
}