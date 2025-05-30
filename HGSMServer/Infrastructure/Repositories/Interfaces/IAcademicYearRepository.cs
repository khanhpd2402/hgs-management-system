﻿using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IAcademicYearRepository
    {
        Task<List<AcademicYear>> GetAllAsync();
        Task<AcademicYear?> GetByIdAsync(int id);
        Task<AcademicYear?> GetByNameAsync(string name);
        Task AddAsync(AcademicYear academicYear);
        Task UpdateAsync(AcademicYear academicYear);
        Task DeleteAsync(int id);
        Task<AcademicYear?> GetCurrentAcademicYearAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }

}
