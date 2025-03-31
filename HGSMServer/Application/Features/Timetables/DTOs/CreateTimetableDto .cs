﻿namespace Application.Features.Timetables.DTOs
{
    public class CreateTimetableDto
    {
        public int SemesterId { get; set; }
        public DateOnly EffectiveDate { get; set; }
        public List<TimetableDetailDto> Details { get; set; } = new();
    }

    public class CreateTimetableDetailDto
    {
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public int Shift { get; set; }
        public int Period { get; set; }
    }

}
