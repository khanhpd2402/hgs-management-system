using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Users.DTOs
{
    public class LinkStudentToParentDto
    {
        public int StudentId { get; set; }
        public int ParentId { get; set; }
        public string Relationship { get; set; }
    }
}
