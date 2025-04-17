using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Conducts.DTOs
{
    public class UpdateConductDto
    {
        public string ConductType { get; set; }
        public string? Note { get; set; }
    }

}
