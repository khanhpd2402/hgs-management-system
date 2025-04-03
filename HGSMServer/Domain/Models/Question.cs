using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Question
{
    public int QuestionId { get; set; }

    public int SubjectId { get; set; }

    public int Grade { get; set; }

    public string Content { get; set; } = null!;

    public string? Options { get; set; }

    public string? CorrectAnswer { get; set; }

    public string? Difficulty { get; set; }

    public string? QuestionType { get; set; }

    public string? ImageUrl { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? MathContent { get; set; }

    public virtual Teacher CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<ExamProposalQuestion> ExamProposalQuestions { get; set; } = new List<ExamProposalQuestion>();

    public virtual Subject Subject { get; set; } = null!;
}
