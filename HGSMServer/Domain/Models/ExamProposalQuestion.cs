using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class ExamProposalQuestion
{
    public int Id { get; set; }

    public int ProposalId { get; set; }

    public int QuestionId { get; set; }

    public int OrderNumber { get; set; }

    public virtual ExamProposal Proposal { get; set; } = null!;

    public virtual Question Question { get; set; } = null!;
}
