using Application.Features.Role.DTOs;
using Application.Features.Role.Interfaces;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Role.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<List<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllRolesAsync();
            return roles.Select(r => new RoleDto { RoleID = r.RoleId, RoleName = r.RoleName }).ToList();
        }

        public async Task<RoleDto> GetRoleByIdAsync(int roleId)
        {
            var role = await _roleRepository.GetRoleByIdAsync(roleId);
            if (role == null) return null;
            return new RoleDto { RoleID = role.RoleId, RoleName = role.RoleName };
        }

        public async Task<RoleDto> AddRoleAsync(string roleName)
        {
            var newRole = new Domain.Models.Role { RoleName = roleName };
            var role = await _roleRepository.AddRoleAsync(newRole);
            return new RoleDto { RoleID = role.RoleId, RoleName = role.RoleName };
        }

        public async Task<RoleDto> UpdateRoleAsync(int roleId, string roleName)
        {
            var role = await _roleRepository.GetRoleByIdAsync(roleId);
            if (role == null) return null;

            role.RoleName = roleName;
            var updatedRole = await _roleRepository.UpdateRoleAsync(role);
            return new RoleDto { RoleID = updatedRole.RoleId, RoleName = updatedRole.RoleName };
        }

        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            return await _roleRepository.DeleteRoleAsync(roleId);
        }
    }

}
