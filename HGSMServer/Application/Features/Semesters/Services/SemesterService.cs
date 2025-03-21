using Application.Features.Semesters.DTOs;
using Application.Features.Semesters.Interfaces;
using AutoMapper;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Semesters.Services
{
    public class SemesterService : ISemesterService
    {
        private readonly ISemesterRepository _repository;
        private readonly IMapper _mapper;

        public SemesterService(ISemesterRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<SemesterDto>> GetByAcademicYearIdAsync(int academicYearId)
        {
            var semesters = await _repository.GetByAcademicYearIdAsync(academicYearId);
            return _mapper.Map<List<SemesterDto>>(semesters);
        }

        public async Task<SemesterDto?> GetByIdAsync(int id)
        {
            var semester = await _repository.GetByIdAsync(id);
            return semester != null ? _mapper.Map<SemesterDto>(semester) : null;
        }

        public async Task AddAsync(CreateSemesterDto semesterDto)
        {
            var semester = _mapper.Map<Semester>(semesterDto);
            await _repository.AddAsync(semester);
        }

        public async Task UpdateAsync(SemesterDto semesterDto)
        {
            var semester = _mapper.Map<Semester>(semesterDto);
            await _repository.UpdateAsync(semester);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }

}
