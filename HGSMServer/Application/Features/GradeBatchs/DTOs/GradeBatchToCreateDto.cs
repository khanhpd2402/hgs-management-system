using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.GradeBatchs.DTOs
{
    public class GradeBatchToCreateDto
    {        
            public GradeBatchDto GradeBatch { get; set; }
            public List<int> SubjectIds { get; set; }
            public List<string> AssessmentTypes { get; set; }        
    }
}
