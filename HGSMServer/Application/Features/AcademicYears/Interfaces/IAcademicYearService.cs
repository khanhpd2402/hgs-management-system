﻿// File: IAcademicYearService.cs
using Application.Features.AcademicYears.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.AcademicYears.Interfaces
{
    public interface IAcademicYearService
    {
        Task<List<AcademicYearDto>> GetAllAsync();
        Task<AcademicYearDto?> GetByIdAsync(int id);
        Task AddAsync(CreateAcademicYearDto academicYearDto); 
        Task UpdateAsync(UpdateAcademicYearDto academicYearDto);
        Task DeleteAsync(int id);
    }
}