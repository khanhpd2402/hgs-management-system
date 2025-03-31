using Application.Features.Timetables.DTOs;
using Application.Features.Timetables.Interfaces;
using AutoMapper;
using ClosedXML.Excel;
using Domain.Models; // Đảm bảo namespace đúng với model EF sinh ra
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Timetables.Services
{
    public class TimetableService : ITimetableService
    {
        private readonly ITimetableRepository _repository;
        private readonly IMapper _mapper;

        public TimetableService(ITimetableRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<TimetableDto> AddAsync(TimetableDto timetableDto)
        {
            var timetable = _mapper.Map<Timetable>(timetableDto);
            var result = await _repository.AddAsync(timetable);
            return _mapper.Map<TimetableDto>(result);
        }

        public async Task<IEnumerable<TimetableDetailDto>> GetTimetableByStudentAsync(int studentId, int? semesterId = null, DateOnly? effectiveDate = null)
        {
            var details = await _repository.GetByStudentIdAsync(studentId, semesterId, effectiveDate);
            return _mapper.Map<IEnumerable<TimetableDetailDto>>(details);
        }

        public async Task<IEnumerable<TimetableDetailDto>> GetTimetableByTeacherAsync(int teacherId, int? semesterId = null, DateOnly? effectiveDate = null)
        {
            var details = await _repository.GetByTeacherIdAsync(teacherId, semesterId, effectiveDate);
            return _mapper.Map<IEnumerable<TimetableDetailDto>>(details);
        }

        public async Task<bool> UpdateDetailAsync(TimetableDetailDto detailDto)
        {
            var detail = _mapper.Map<TimetableDetail>(detailDto);
            return await _repository.UpdateDetailAsync(detail);
        }

        public async Task<bool> DeleteDetailAsync(int detailId)
        {
            return await _repository.DeleteDetailAsync(detailId);
        }

        public async Task<bool> IsConflictAsync(TimetableDetailDto detailDto)
        {
            var detail = _mapper.Map<TimetableDetail>(detailDto);
            return await _repository.IsConflictAsync(detail);
        }
    }

}