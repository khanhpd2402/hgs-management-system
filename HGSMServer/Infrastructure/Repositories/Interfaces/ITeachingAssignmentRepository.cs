﻿using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface ITeachingAssignmentRepository
    {
        Task<TeachingAssignment> GetAssignmentByClassSubjectTeacherAsync(int classId, int subjectId, int semesterId);
        Task<IEnumerable<TeachingAssignment>> GetBySemesterIdAsync(int semesterId);
    }
}
