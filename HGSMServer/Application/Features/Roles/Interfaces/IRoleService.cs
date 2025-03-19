using Application.Features.Role.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Role.Interfaces
{
    public interface IRoleService
    {
        Task<List<RoleDto>> GetAllRolesAsync();
        Task<RoleDto> GetRoleByIdAsync(int roleId);
        Task<RoleDto> AddRoleAsync(string roleName);
        Task<RoleDto> UpdateRoleAsync(int roleId, string roleName);
        Task<bool> DeleteRoleAsync(int roleId);

    }

}
