﻿using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Grade
{
    public int GradeId { get; set; }

    public int StudentId { get; set; }

    public int SubjectId { get; set; }

    public decimal Score { get; set; }

    public int ExamId { get; set; }

    public int SemesterId { get; set; }

    public virtual Exam Exam { get; set; } = null!;

    public virtual Semester Semester { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;
}
