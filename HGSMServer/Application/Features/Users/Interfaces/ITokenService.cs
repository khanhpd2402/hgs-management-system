using Application.Features.Users.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Features.Users.Interfaces
{
    public interface ITokenService
    {
        Task<(string tokenString, Dictionary<string, string> tokenPayload)> GenerateTokenAsync(UserDTO user); 
    }
}