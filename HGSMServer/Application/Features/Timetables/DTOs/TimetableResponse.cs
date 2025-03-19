namespace Application.Features.Timetables.DTOs
{
    public class TimetableRequest
    {
        public int Semester { get; set; }
        public string SchoolYear { get; set; } = null!;
        public ExternalConstraints Constraints { get; set; } = new ExternalConstraints();
    }

    public class ExternalConstraints
    {
        // THCS thường học cả sáng và chiều, nhưng có thể tắt nếu không cần
        public bool HasMorningShift { get; set; } = true;
        // Số tiết tối đa của giáo viên (THCS thường 18-19 tiết/tuần)
        public int DefaultTeacherPeriods { get; set; } = 19;
        public int PrincipalPeriods { get; set; } = 2;
        public int VicePrincipalPeriods { get; set; } = 4;
        public int HeadOfDepartmentReduction { get; set; } = 3;
        public int DeputyHeadReduction { get; set; } = 1;
        public int UnionChairReduction { get; set; } = 3;
        public int TeamLeaderReduction { get; set; } = 10;
        // Ràng buộc môn học THCS
        public bool DoubleLiteratureDay { get; set; } = true; // Ngữ văn 2 tiết liền
        public bool NoPEInLastPeriod { get; set; } = true; // Thể dục không tiết 5
        public int ThursdayPeriodsSemester1 { get; set; } = 4; // Thứ 5 HK1: 4 tiết
        public bool MathLiteratureMaxTwoGrades { get; set; } = true; // Toán, Văn không dạy 3 khối
                                                                     // Hoạt động trải nghiệm (THCS thường có chào cờ và sinh hoạt lớp)
        public bool HasFlagCeremony { get; set; } = true; // Chào cờ thứ 2
        public bool HasClassMeeting { get; set; } = true; // Sinh hoạt lớp thứ 7
    }
    public class ScheduleResponseDto
    {
        public Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<PeriodDto>>>>> ScheduleData { get; set; } = new();

        public class PeriodDto
        {
            public int Period { get; set; }
            public string SubjectId { get; set; } = null!;
            public string TeacherId { get; set; } = null!;
        }
    }
}
