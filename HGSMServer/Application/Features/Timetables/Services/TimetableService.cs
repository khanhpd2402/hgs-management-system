﻿using Application.Features.Timetables.DTOs;
using Application.Features.Timetables.Interfaces;
using AutoMapper;
using ClosedXML.Excel;
using Domain.Models; // Đảm bảo namespace đúng với model EF sinh ra
using Infrastructure.Repositories.Implementtations;
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

        public async Task<Timetable> CreateTimetableAsync(CreateTimetableDto dto)
        {
            // Map và tạo Timetable
            var timetable = _mapper.Map<Timetable>(dto);
            var createdTimetable = await _repository.CreateTimetableAsync(timetable);

            // Map và thêm các TimetableDetails
            foreach (var detailDto in dto.Details)
            {
                var timetableDetail = _mapper.Map<TimetableDetail>(detailDto);
                timetableDetail.TimetableId = createdTimetable.TimetableId; // Gán TimetableId
                await _repository.AddTimetableDetailAsync(timetableDetail);
                createdTimetable.TimetableDetails.Add(timetableDetail);
            }

            return createdTimetable;
        }

        public async Task<IEnumerable<TimetableDto>> GetTimetableByStudentAsync(int studentId, int? semesterId = null, DateOnly? effectiveDate = null)
        {
            var timetables = await _repository.GetByStudentIdAsync(studentId, semesterId, effectiveDate);
            return _mapper.Map<IEnumerable<TimetableDto>>(timetables);
        }

        public async Task<IEnumerable<TimetableDto>> GetTimetableByTeacherAsync(int teacherId, int? semesterId = null, DateOnly? effectiveDate = null)
        {
            var timetables = await _repository.GetByTeacherIdAsync(teacherId, semesterId, effectiveDate);
            return _mapper.Map<IEnumerable<TimetableDto>>(timetables);
        }
        public async Task<IEnumerable<TimetableDto>> GetTimetableByClassAsync(int classId, int? semesterId = null, DateOnly? effectiveDate = null)
        {
            var timetables = await _repository.GetByClassIdAsync(classId, semesterId, effectiveDate);
            return _mapper.Map<IEnumerable<TimetableDto>>(timetables);
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