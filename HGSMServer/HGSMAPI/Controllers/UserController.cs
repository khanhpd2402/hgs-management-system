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
            Console.WriteLine($"Retrieved {users.Count()} users.");
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
                return NotFound(new { message = $"User with ID {id} not found." });
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
                return BadRequest(new { message = "Invalid input data.", errors });
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
                return BadRequest(new { message = "User data cannot be null." });
            }

            if (id != userDto.UserId)
            {
                Console.WriteLine($"UpdateUser: ID mismatch. URL ID: {id}, Body ID: {userDto.UserId}.");
                return BadRequest(new { message = "User ID in URL does not match the ID in the body." });
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
                return StatusCode(500, new { message = "An error occurred while updating the user.", error = ex.Message });
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
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"DeleteUser: Error - {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeleteUser: Unexpected error - {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while deleting the user.", error = ex.Message });
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (changePasswordDto == null || string.IsNullOrEmpty(changePasswordDto.OldPassword) || string.IsNullOrEmpty(changePasswordDto.NewPassword))
            {
                Console.WriteLine("ChangePassword: Invalid input data.");
                return BadRequest(new { message = "Old password and new password are required." });
            }

            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    Console.WriteLine("ChangePassword: User ID not found in token.");
                    return Unauthorized(new { message = "User ID not found in token." });
                }

                var userId = int.Parse(userIdClaim.Value);
                Console.WriteLine($"User with ID {userId} is attempting to change their password...");

                await _userService.ChangePasswordAsync(userId, changePasswordDto);
                Console.WriteLine($"User with ID {userId} changed their password successfully.");
                return Ok(new { message = "Password changed successfully." });
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
                return StatusCode(500, new { message = "An error occurred while changing the password.", error = ex.Message });
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

            if (!new[] { "Active", "Deactive" }.Contains(changeStatusDto.Status))
            {
                Console.WriteLine($"ChangeStatus: Invalid status value: {changeStatusDto.Status}.");
                return BadRequest(new { message = "Status must be either 'Active' or 'Deactive'." });
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

    // DTO bổ sung nếu chưa có trong mã nguồn trước
    public class ChangeStatusDto
    {
        public string Status { get; set; }
    }

    public class AdminChangePasswordDto
    {
        public string NewPassword { get; set; }
    }
}