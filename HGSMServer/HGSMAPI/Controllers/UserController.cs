using Application.Features.Users.DTOs;
using Application.Features.Users.Interfaces;
using Microsoft.AspNetCore.Authorization; // Thêm namespace này
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        // GET: api/User
        [HttpGet]
        [Authorize] // Yêu cầu đăng nhập, nhưng không giới hạn role cụ thể
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        [Authorize] // Yêu cầu đăng nhập
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = $"User with ID {id} not found." });

            return Ok(user);
        }

        // POST: api/User
        [HttpPost]
        [Authorize(Policy = "AdminOfficerOnly")] // Chỉ AdministrativeOfficer (RoleID = 5) được phép tạo user
        public async Task<IActionResult> CreateUser([FromBody] UserDTO userDto)
        {
            if (userDto == null)
                return BadRequest(new { message = "User data cannot be null." });

            if (string.IsNullOrEmpty(userDto.Username) || string.IsNullOrEmpty(userDto.PasswordHash))
                return BadRequest(new { message = "Username and PasswordHash are required." });

            // Kiểm tra RoleID hợp lệ (từ 1 đến 6)
            if (userDto.RoleId < 1 || userDto.RoleId > 6)
                return BadRequest(new { message = "Invalid RoleID. Must be between 1 and 6." });

            await _userService.AddUserAsync(userDto);
            return CreatedAtAction(nameof(GetUser), new { id = userDto.UserId }, userDto);
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOfficerOnly")] // Chỉ AdministrativeOfficer (RoleID = 5) được phép cập nhật
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDTO userDto)
        {
            if (userDto == null)
                return BadRequest(new { message = "User data cannot be null." });

            if (id != userDto.UserId)
                return BadRequest(new { message = "User ID in URL does not match the ID in the body." });

            if (string.IsNullOrEmpty(userDto.Username) || string.IsNullOrEmpty(userDto.PasswordHash))
                return BadRequest(new { message = "Username and PasswordHash are required." });

            // Kiểm tra RoleID hợp lệ (từ 1 đến 6)
            if (userDto.RoleId < 1 || userDto.RoleId > 6)
                return BadRequest(new { message = "Invalid RoleID. Must be between 1 and 6." });

            try
            {
                await _userService.UpdateUserAsync(userDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOfficerOnly")] // Chỉ AdministrativeOfficer (RoleID = 5) được phép xóa
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}