using Application.Features.Role.Interfaces;
using Application.Features.Users.DTOs;
using Application.Features.Users.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;

        public RolesController(IRoleService roleService, IUserService userService)
        {
            _roleService = roleService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null) return NotFound();
            return Ok(role);
        }

        [HttpPost]
        [Authorize(Roles = "Hiệu trưởng,Cán bộ văn thư")]
        public async Task<IActionResult> AddRole([FromBody] string roleName)
        {
            var newRole = await _roleService.AddRoleAsync(roleName);
            return CreatedAtAction(nameof(GetRoleById), new { id = newRole.RoleID }, newRole);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] string roleName)
        {
            var originalRole = await _roleService.GetRoleByIdAsync(id);
            if (originalRole == null)
                return NotFound();

            var updatedRole = await _roleService.UpdateRoleAsync(id, roleName);
            if (updatedRole == null)
                return NotFound();

            // Nếu vai trò liên quan đến "Trưởng bộ môn", cập nhật IsHeadOfDepartment cho người dùng
            if (originalRole.RoleName.Equals("Trưởng bộ môn", StringComparison.OrdinalIgnoreCase) ||
                roleName.Equals("Trưởng bộ môn", StringComparison.OrdinalIgnoreCase))
            {
                var usersWithRole = await _userService.GetAllUsersAsync();
                foreach (var user in usersWithRole.Where(u => u.RoleId == id))
                {
                    var updateUserDto = new UpdateUserDTO
                    {
                        UserId = user.UserId,
                        Username = user.Username,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        RoleId = id
                    };
                    await _userService.UpdateUserAsync(updateUserDto); // Gọi UpdateUserAsync để cập nhật IsHeadOfDepartment
                }
            }

            return Ok(updatedRole);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var success = await _roleService.DeleteRoleAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }

}
