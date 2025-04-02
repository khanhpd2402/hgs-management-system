namespace Application.Features.Exams.DTOs
{
    public class QuestionDto
    {
        public int QuestionId { get; set; }
        public int SubjectId { get; set; }
        public int Grade { get; set; }
        public string Content { get; set; }
        public string? MathContent { get; set; } 
        public string? Options { get; set; }
        public string? CorrectAnswer { get; set; }
        public string Difficulty { get; set; }
        public string QuestionType { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}