using Application.Features.Users.DTOs;
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
            Console.WriteLine($"Lấy ra {users.Count()} người dùng.");
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Hiệu trưởng,Cán bộ văn thư")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            Console.WriteLine($"Fetching user with ID {id}...");
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                Console.WriteLine($"User with ID {id} not found.");
                //return NotFound(new { message = $"User with ID {id} not found." });
                return NotFound(new { message = "Không tìm thấy người dùng." });
            }

            Console.WriteLine($"User with ID {id} found: {user.Username}");
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
                return BadRequest(new { message = "Nhập thông tin không hợp lệ.", errors });
            }

            Console.WriteLine("Creating new user...");
            var createdUser = await _userService.AddUserAsync(userDto);

            Console.WriteLine($"User created with ID {createdUser.UserId} and username {createdUser.Username}.");
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.UserId }, createdUser);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Hiệu trưởng,Cán bộ văn thư")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDTO userDto)
        {
            if (userDto == null)
            {
                Console.WriteLine("UpdateUser: User data is null.");
                return BadRequest(new { message = "Thông tin người dùng không được để trống." });
            }

            if (id != userDto.UserId)
            {
                Console.WriteLine($"UpdateUser: ID mismatch. URL ID: {id}, Body ID: {userDto.UserId}.");
                return BadRequest(new { message = "Id ở URL và Body không khớp." });
            }

            try
            {
                Console.WriteLine($"Updating user with ID {id}...");
                await _userService.UpdateUserAsync(userDto);
                Console.WriteLine($"User with ID {id} updated successfully.");
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"UpdateUser: Error - {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateUser: Unexpected error - {ex.Message}");
                return StatusCode(500, new { message = "Đã có lỗi xảy ra khi cập nhật thông tin người dùng.", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Hiệu trưởng,Cán bộ văn thư")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                Console.WriteLine($"Deleting user with ID {id}...");
                await _userService.DeleteUserAsync(id);
                Console.WriteLine($"User with ID {id} deleted successfully.");
                return StatusCode(200, new { message = "Xóa người dùng thành công."});
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"DeleteUser: Error - {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeleteUser: Unexpected error - {ex.Message}");
                return StatusCode(500, new { message = "Đã có lỗi xảy ra khi xóa người dùng.", error = ex.Message });
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (changePasswordDto == null || string.IsNullOrEmpty(changePasswordDto.OldPassword) || string.IsNullOrEmpty(changePasswordDto.NewPassword))
            {
                Console.WriteLine("ChangePassword: Invalid input data.");
                return BadRequest(new { message = "Mật khẩu cũ và mới không được để trống." });
            }

            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    Console.WriteLine("ChangePassword: User ID not found in token.");
                    return Unauthorized(new { message = "User ID không tìm thấy trong token." });
                }

                var userId = int.Parse(userIdClaim.Value);
                Console.WriteLine($"User with ID {userId} is attempting to change their password...");

                await _userService.ChangePasswordAsync(userId, changePasswordDto);
                Console.WriteLine($"User with ID {userId} changed their password successfully.");
                return Ok(new { message = "Đổi mật khẩu thành công." });
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"ChangePassword: Unauthorized - {ex.Message}");
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"ChangePassword: Error - {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ChangePassword: Unexpected error - {ex.Message}");
                return StatusCode(500, new { message = "Có lỗi xảy ra trong quá trình đổi mật khẩu.", error = ex.Message });
            }
        }

        [HttpPost("{id}/change-status")]
        [Authorize(Roles = "Hiệu trưởng,Cán bộ văn thư\"")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeStatusDto changeStatusDto)
        {
            if (changeStatusDto == null || string.IsNullOrEmpty(changeStatusDto.Status))
            {
                Console.WriteLine("ChangeStatus: Invalid status data.");
                return BadRequest(new { message = "Status is required." });
            }

            if (!new[] { "Hoạt động", "Không hoạt động" }.Contains(changeStatusDto.Status))
            {
                Console.WriteLine($"ChangeStatus: Invalid status value: {changeStatusDto.Status}.");
                return BadRequest(new { message = "Trạng thái phải là'Hoạt động' hoặc 'Không hoạt động'." });
            }

            try
            {
                var userDto = await _userService.GetUserByIdAsync(id);
                if (userDto == null)
                {
                    Console.WriteLine($"ChangeStatus: User with ID {id} not found.");
                    return NotFound(new { message = $"User with ID {id} not found." });
                }

                Console.WriteLine($"Changing status of user with ID {id} to {changeStatusDto.Status}...");
                await _userService.ChangeUserStatusAsync(id, changeStatusDto.Status);
                Console.WriteLine($"Status of user with ID {id} updated to {changeStatusDto.Status}.");
                return Ok(new { message = "Status updated successfully.", userId = id, newStatus = changeStatusDto.Status });
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"ChangeStatus: Error - {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ChangeStatus: Unexpected error - {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while updating the status.", error = ex.Message });
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
                return BadRequest(new { message = "Invalid input data.", errors });
            }

            try
            {
                Console.WriteLine($"Admin is changing password for user with ID {id}...");
                await _userService.AdminChangePasswordAsync(id, adminChangePasswordDto.NewPassword);
                Console.WriteLine($"Password for user with ID {id} changed successfully by admin.");
                return Ok(new { message = "Password changed successfully by admin.", userId = id });
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"AdminChangePassword: Error - {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AdminChangePassword: Unexpected error - {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while changing the password.", error = ex.Message });
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