﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.DTOs
{
    public class CreateUserDTO
    {
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int RoleId { get; set; }
    }
}
