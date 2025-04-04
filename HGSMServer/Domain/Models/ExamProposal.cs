using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class ExamProposal
{
    public int ProposalId { get; set; }

    public int SubjectId { get; set; }

    public int Grade { get; set; }

    public string Title { get; set; } = null!;

    public int SemesterId { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }
}
