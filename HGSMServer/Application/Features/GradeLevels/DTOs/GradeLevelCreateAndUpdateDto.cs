using System.ComponentModel.DataAnnotations;

namespace Application.Features.GradeLevels.DTOs
{
    public class GradeLevelCreateAndUpdateDto
    {
        [Required]
        [StringLength(20)]
        public string GradeName { get; set; }
    }
}
