namespace Application.Features.Exams.DTOs
{
    public class ExamProposalDto
    {
        public int ProposalId { get; set; }
        public int SubjectId { get; set; }
        public int Grade { get; set; }
        public string Title { get; set; }
        public int SemesterId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string FileUrl { get; set; } 
    }
}