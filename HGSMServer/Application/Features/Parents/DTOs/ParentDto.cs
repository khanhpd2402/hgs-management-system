using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Parents.DTOs
{
    public class ParentDto
    {
        public int ParentId { get; set; }
        public string FullName { get; set; } = null!;
        public DateOnly? Dob { get; set; }
        public string? Occupation { get; set; }
        public string Relationship { get; set; } = null!;
        public string? PhoneNumber { get; set; }  // Số điện thoại
        public string? Email { get; set; }        // Email
    }

}
