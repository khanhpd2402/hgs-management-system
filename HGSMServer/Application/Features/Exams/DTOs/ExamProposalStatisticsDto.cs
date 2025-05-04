using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Exams.DTOs
{
    public class ExamProposalStatisticsDto
    {
        public int TotalExamProposals { get; set; }
        public int SubmittedCount { get; set; }
        public int PendingCount { get; set; }
        public int WaitingForApprovalCount { get; set; }
    }
}
