namespace Application.Features.Grades.DTOs
{
    public class GradeSummaryDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string SubjectName { get; set; }

        public double? Semester1Average { get; set; } // Điểm trung bình HK1
        public double? Semester2Average { get; set; } // Điểm trung bình HK2
        public double? YearAverage { get; set; }      // Điểm trung bình cả năm
    }

}
//namespace Application.Features.Grades.DTOs
//{
//    public class GradeSummaryEachSubjectNameDto
//    {
//        public int StudentId { get; set; }
//        public string StudentName { get; set; }
//        public string SubjectName { get; set; }

//        public double? Semester1Average { get; set; } // Điểm trung bình HK1
//        public double? Semester2Average { get; set; } // Điểm trung bình HK2
//        public double? YearAverage { get; set; }      // Điểm trung bình cả năm
//    }
//    public class GradeSummaryDto
//    {
//        public int StudentId { get; set; }
//        public double? totalSemester1Average { get; set; }
//        public double? totalSemester2Average { get; set; }
//        public double? totalYearAverage { get; set; }
//        public List<GradeSummaryEachSubjectNameDto>? gradeSummaryEachSubjectNameDtos { get; set; }
//    }
//}
