using Application.Features.AcademicYears.DTOs;
using Application.Features.AcademicYears.Interfaces;
using AutoMapper;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.AcademicYears.Services
{
    public class AcademicYearService : IAcademicYearService
    {
        private readonly IAcademicYearRepository _repository;
        private readonly IMapper _mapper;

        public AcademicYearService(IAcademicYearRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<AcademicYearDto>> GetAllAsync()
        {
            var academicYears = await _repository.GetAllAsync();
            return _mapper.Map<List<AcademicYearDto>>(academicYears);
        }

        public async Task<AcademicYearDto?> GetByIdAsync(int id)
        {
            var academicYear = await _repository.GetByIdAsync(id);
            return academicYear != null ? _mapper.Map<AcademicYearDto>(academicYear) : null;
        }

        public async Task AddAsync(CreateAcademicYearDto academicYearDto)
        {
            var academicYear = _mapper.Map<AcademicYear>(academicYearDto);
            await _repository.AddAsync(academicYear);
        }

        public async Task UpdateAsync(AcademicYearDto academicYearDto)
        {
            var academicYear = _mapper.Map<AcademicYear>(academicYearDto);
            await _repository.UpdateAsync(academicYear);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }

}
