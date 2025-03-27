using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.DTOs
{
    public class CreateUserDTO
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string PasswordHash { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "RoleId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "RoleId must be a positive integer.")]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "FullName is required.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "School join date is required.")]
        public DateTime SchoolJoinDate { get; set; }
    }
}
