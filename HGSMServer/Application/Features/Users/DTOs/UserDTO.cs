namespace Application.Features.Users.DTOs
{
    public class UserDTO
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public int RoleId { get; set; }

        public string? Status { get; set; }
    }
}