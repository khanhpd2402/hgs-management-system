using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Subjects.DTOs
{
    public class SubjectCreateAndUpdateDto
    {
        [Required]
        [StringLength(100)]
        public string SubjectName { get; set; }

        [Required]
        [StringLength(50)]
        public string SubjectCategory { get; set; }

        [Required]
        [RegularExpression(@"^(Tính điểm|Nhận xét)$", ErrorMessage = "TypeOfGrade must be 'Tính điểm' or 'Nhận xét'")]
        public string TypeOfGrade { get; set; }
    }
}
