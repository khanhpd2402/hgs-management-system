using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.Students.DTOs; 

namespace Application.Features.StudentClass.DTOs
{
    public class BulkTransferResultDto
    {
        public List<StudentDto> SuccessfullyTransferredStudents { get; set; } = new List<StudentDto>();
        public List<SkippedStudentDto> SkippedStudents { get; set; } = new List<SkippedStudentDto>();
        public int TotalStudentsProcessed { get; set; }
        public int SuccessfulCount { get; set; }

        public int SkippedCount { get; set; }
    }

   
    //DTO chứa thông tin học sinh bị bỏ qua và lý do.
   
    public class SkippedStudentDto
    {
        public int StudentId { get; set; }
        public string FullName { get; set; }
        public string Reason { get; set; }
    }
}
