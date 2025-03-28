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
        [Authorize(Roles = "Principal,AdministrativeOfficer")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            Console.WriteLine("Fetching all users...");
            var users = await _userService.GetAllUsersAsync();
            Console.WriteLine($"Retrieved {users.Count()} users.");
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Principal,AdministrativeOfficer")]
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
        [Authorize(Roles = "Principal,AdministrativeOfficer")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO userDto)
        {
            if (userDto == null)
            {
                Console.WriteLine("CreateUser: User data is null.");
                return BadRequest(new { message = "User data cannot be null." });
            }

            if (string.IsNullOrEmpty(userDto.Username) || string.IsNullOrEmpty(userDto.PasswordHash))
            {
                Console.WriteLine("CreateUser: Username or PasswordHash is missing.");
                return BadRequest(new { message = "Username and PasswordHash are required." });
            }

            Console.WriteLine($"Creating user with username: {userDto.Username}...");
            await _userService.AddUserAsync(userDto);
            var createdUser = await _userService.GetUserByUsernameAsync(userDto.Username); // Lấy lại user để trả về
            Console.WriteLine($"User created with ID {createdUser.UserId} and username {userDto.Username}.");
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.UserId }, createdUser);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Principal,AdministrativeOfficer")]
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

            if (string.IsNullOrEmpty(userDto.Username))
            {
                Console.WriteLine("UpdateUser: Username is missing.");
                return BadRequest(new { message = "Username is required." });
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
        [Authorize(Roles = "Principal,AdministrativeOfficer")]
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
        [Authorize(Roles = "Principal,AdministrativeOfficer")]
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
                // Lấy user từ UserService để kiểm tra sự tồn tại
                var userDto = await _userService.GetUserByIdAsync(id);
                if (userDto == null)
                {
                    Console.WriteLine($"ChangeStatus: User with ID {id} not found.");
                    return NotFound(new { message = $"User with ID {id} not found." });
                }

                Console.WriteLine($"Changing status of user with ID {id} to {changeStatusDto.Status}...");
                // Cập nhật Status trực tiếp trên đối tượng User trong database
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
        [Authorize(Roles = "Principal,AdministrativeOfficer")]
        public async Task<IActionResult> AdminChangePassword(int id, [FromBody] string newPassword)
        {
            try
            {
                await _userService.AdminChangePasswordAsync(id, newPassword);
                return Ok(new { message = "Password changed successfully by admin.", userId = id });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while changing the password.", error = ex.Message });
            }
        }
    }
}