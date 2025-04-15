using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.HomeRooms.DTOs
{
    public class UpdateHomeroomDto
    {
        public int HomeroomAssignmentId { get; set; }
        public int TeacherId { get; set; } 
        public string Status { get; set; }
    }
}
