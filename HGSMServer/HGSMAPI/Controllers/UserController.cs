﻿using Application.Features.Users.DTOs;
using Application.Features.Users.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HGSMAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpGet]
        [Authorize(Roles = "Hiệu trưởng,Cán bộ văn thư")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            Console.WriteLine("Fetching all users...");
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Hiệu trưởng,Cán bộ văn thư")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            Console.WriteLine("Fetching user...");
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                Console.WriteLine("User not found.");
                return NotFound("Không tìm thấy người dùng.");
            }

            return Ok(user);
        }

        [HttpPost]
        [Authorize(Roles = "Hiệu trưởng,Cán bộ văn thư")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO userDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                Console.WriteLine($"Validation errors: {string.Join(", ", errors)}");
                return BadRequest("Nhập thông tin không hợp lệ.");
            }

            Console.WriteLine("Creating new user...");
            var createdUser = await _userService.AddUserAsync(userDto);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.UserId }, createdUser);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Hiệu trưởng,Cán bộ văn thư")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDTO userDto)
        {
            if (userDto == null)
            {
                Console.WriteLine("User data is null.");
                return BadRequest("Thông tin người dùng không được để trống.");
            }

            if (id != userDto.UserId)
            {
                Console.WriteLine("ID mismatch.");
                return BadRequest("ID không khớp.");
            }

            try
            {
                Console.WriteLine("Updating user...");
                await _userService.UpdateUserAsync(userDto);
                return Ok("Cập nhật thành công");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error updating user: {ex.Message}");
                return NotFound("Không tìm thấy người dùng.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error updating user: {ex.Message}");
                return StatusCode(500, "Lỗi khi cập nhật thông tin người dùng.");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Hiệu trưởng,Cán bộ văn thư")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                Console.WriteLine("Deleting user...");
                await _userService.DeleteUserAsync(id);
                return Ok("Xóa người dùng thành công.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error deleting user: {ex.Message}");
                return NotFound("Không tìm thấy người dùng.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error deleting user: {ex.Message}");
                return StatusCode(500, "Lỗi khi xóa người dùng.");
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (changePasswordDto == null || string.IsNullOrEmpty(changePasswordDto.OldPassword) || string.IsNullOrEmpty(changePasswordDto.NewPassword))
            {
                Console.WriteLine("Invalid password input data.");
                return BadRequest("Mật khẩu cũ và mới không được để trống.");
            }

            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    Console.WriteLine("User ID not found in token.");
                    return Unauthorized("Không tìm thấy ID người dùng trong token.");
                }

                var userId = int.Parse(userIdClaim.Value);
                Console.WriteLine("Attempting to change password...");
                await _userService.ChangePasswordAsync(userId, changePasswordDto);
                return Ok("Đổi mật khẩu thành công.");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Unauthorized password change: {ex.Message}");
                return Unauthorized("Không có quyền đổi mật khẩu.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error changing password: {ex.Message}");
                return BadRequest("Lỗi khi đổi mật khẩu.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error changing password: {ex.Message}");
                return StatusCode(500, "Lỗi khi đổi mật khẩu.");
            }
        }

        [HttpPost("{id}/change-status")]
        [Authorize(Roles = "Hiệu trưởng,Cán bộ văn thư")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeStatusDto changeStatusDto)
        {
            if (changeStatusDto == null || string.IsNullOrEmpty(changeStatusDto.Status))
            {
                Console.WriteLine("Invalid status data.");
                return BadRequest("Trạng thái không được để trống.");
            }

            if (!new[] { "Hoạt động", "Không hoạt động" }.Contains(changeStatusDto.Status))
            {
                Console.WriteLine("Invalid status value.");
                return BadRequest("Trạng thái phải là 'Hoạt động' hoặc 'Không hoạt động'.");
            }

            try
            {
                var userDto = await _userService.GetUserByIdAsync(id);
                if (userDto == null)
                {
                    Console.WriteLine("User not found for status change.");
                    return NotFound("Không tìm thấy người dùng.");
                }

                Console.WriteLine("Changing user status...");
                await _userService.ChangeUserStatusAsync(id, changeStatusDto.Status);
                return Ok("Cập nhật trạng thái thành công.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error changing status: {ex.Message}");
                return BadRequest("Lỗi khi cập nhật trạng thái. " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error changing status: {ex.Message}");
                return StatusCode(500, "Lỗi khi cập nhật trạng thái.");
            }
        }

        [HttpPost("{id}/admin-change-password")]
        [Authorize(Roles = "Hiệu trưởng,Cán bộ văn thư")]
        public async Task<IActionResult> AdminChangePassword(int id, [FromBody] AdminChangePasswordDto adminChangePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                Console.WriteLine($"Validation errors: {string.Join(", ", errors)}");
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            try
            {
                Console.WriteLine("Admin changing password...");
                await _userService.AdminChangePasswordAsync(id, adminChangePasswordDto.NewPassword);
                return Ok("Đổi mật khẩu thành công bởi admin.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error admin changing password: {ex.Message}");
                return BadRequest("Lỗi khi đổi mật khẩu.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error admin changing password: {ex.Message}");
                return StatusCode(500, "Lỗi khi đổi mật khẩu.");
            }
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            try
            {
                await _userService.ForgotPasswordAsync(dto.Email);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống." });
            }
        }
    }


    public class ChangeStatusDto
    {
        public string Status { get; set; }
    }

    public class AdminChangePasswordDto
    {
        public string NewPassword { get; set; }
    }
}