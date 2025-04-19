using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Conducts.DTOs
{
    public class ConductDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int SemesterId { get; set; }
        public string ConductType { get; set; }
        public string? Note { get; set; }
    }

}
