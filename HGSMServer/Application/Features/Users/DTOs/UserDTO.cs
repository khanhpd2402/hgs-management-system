using System;

namespace Application.Features.Users.DTOs
{
    public class UserDTO
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string? PasswordHash { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public int RoleId { get; set; }
        public string? RoleName { get; set; }

        private string? _status;
        public string? Status
        {
            get => _status;
            set
            {
                if (value != null && value != "Hoạt động" && value != "Không hoạt động")
                    throw new ArgumentException("Status phải là 'Hoạt động' hoặc 'Không hoạt động'.");
                _status = value;
            }
        }

        public string? FullName { get; set; } 
    }
}