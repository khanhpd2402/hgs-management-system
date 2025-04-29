using Application.Features.Role.Interfaces;
using Application.Features.Users.DTOs;
using Application.Features.Users.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
            try
            {
                Console.WriteLine("Fetching all roles...");
                var roles = await _roleService.GetAllRolesAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching roles: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy danh sách vai trò.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            try
            {
                Console.WriteLine("Fetching role...");
                var role = await _roleService.GetRoleByIdAsync(id);
                if (role == null)
                {
                    Console.WriteLine("Role not found.");
                    return NotFound("Không tìm thấy vai trò.");
                }
                return Ok(role);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching role: {ex.Message}");
                return StatusCode(500, "Lỗi khi lấy thông tin vai trò.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Hiệu trưởng,Cán bộ văn thư")]
        public async Task<IActionResult> AddRole([FromBody] string roleName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    Console.WriteLine("Invalid role name.");
                    return BadRequest("Tên vai trò không được để trống.");
                }

                Console.WriteLine("Adding role...");
                var newRole = await _roleService.AddRoleAsync(roleName);
                return CreatedAtAction(nameof(GetRoleById), new { id = newRole.RoleID }, newRole);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding role: {ex.Message}");
                return BadRequest("Lỗi khi thêm vai trò.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] string roleName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    Console.WriteLine("Invalid role name.");
                    return BadRequest("Tên vai trò không được để trống.");
                }

                Console.WriteLine("Updating role...");
                var originalRole = await _roleService.GetRoleByIdAsync(id);
                if (originalRole == null)
                {
                    Console.WriteLine("Role not found.");
                    return NotFound("Không tìm thấy vai trò.");
                }

                var updatedRole = await _roleService.UpdateRoleAsync(id, roleName);
                if (updatedRole == null)
                {
                    Console.WriteLine("Role update failed.");
                    return NotFound("Không tìm thấy vai trò.");
                }

                if (originalRole.RoleName.Equals("Trưởng bộ môn", StringComparison.OrdinalIgnoreCase) ||
                    roleName.Equals("Trưởng bộ môn", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Updating users with role...");
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
                        await _userService.UpdateUserAsync(updateUserDto);
                    }
                }

                return Ok(updatedRole);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating role: {ex.Message}");
                return StatusCode(500, "Lỗi khi cập nhật vai trò.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            try
            {
                Console.WriteLine("Deleting role...");
                var success = await _roleService.DeleteRoleAsync(id);
                if (!success)
                {
                    Console.WriteLine("Role not found.");
                    return NotFound("Không tìm thấy vai trò.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting role: {ex.Message}");
                return StatusCode(500, "Lỗi khi xóa vai trò.");
            }
        }
    }
}