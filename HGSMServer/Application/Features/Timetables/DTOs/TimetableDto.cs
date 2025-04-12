﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Timetables.DTOs
{
    public class TimetableDto
    {
        public int TimetableId { get; set; }
        public int SemesterId { get; set; }
        public DateOnly EffectiveDate { get; set; }
        public string Status { get; set; }
        public List<TimetableDetailDto> Details { get; set; }
    }

    public class TimetableDetailDto
    {
        public int TimetableDetailId { get; set; }
        public int TimetableId { get; set; }
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }
        public DateOnly Date { get; set; }
        public int PeriodId { get; set; }
        public string PeriodName { get; set; } 
    }

}
