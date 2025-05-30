﻿namespace Application.Features.Timetables.DTOs
{
    public class CreateTimetableDetailRequest
    {
        public int TimetableId { get; set; }
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }
        public string DayOfWeek { get; set; } = null!; 
        public int PeriodId { get; set; }
    }

}
