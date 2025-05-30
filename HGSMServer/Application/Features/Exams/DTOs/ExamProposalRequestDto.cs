﻿using Microsoft.AspNetCore.Http;

namespace Application.Features.Exams.DTOs
{
    public class ExamProposalRequestDto
    {
        public int SubjectId { get; set; }
        public int Grade { get; set; }
        public string Title { get; set; }
        public int SemesterId { get; set; }
        //public string SubjectName { get; set; } 
        public IFormFile File { get; set; } 
    }
}