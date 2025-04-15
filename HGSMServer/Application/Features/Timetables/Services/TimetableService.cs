using Application.Features.Timetables.DTOs;
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
            var timetable = _mapper.Map<Timetable>(dto);
            var createdTimetable = await _repository.CreateTimetableAsync(timetable);

            foreach (var detailDto in dto.Details)
            {
                var timetableDetail = _mapper.Map<TimetableDetail>(detailDto);
                timetableDetail.TimetableId = createdTimetable.TimetableId;
                await _repository.AddTimetableDetailAsync(timetableDetail);
                createdTimetable.TimetableDetails.Add(timetableDetail);
            }

            return createdTimetable;
        }

        public async Task<IEnumerable<TimetableDto>> GetTimetableByStudentAsync(int studentId, int semesterId)
        {
            var timetables = await _repository.GetByStudentIdAsync(studentId, semesterId);
            return _mapper.Map<IEnumerable<TimetableDto>>(timetables);
        }

        public async Task<IEnumerable<TimetableDto>> GetTimetableByTeacherAsync(int teacherId)
        {
            var timetables = await _repository.GetByTeacherIdAsync(teacherId);
            return _mapper.Map<IEnumerable<TimetableDto>>(timetables);
        }

        public async Task<IEnumerable<TimetableListDto>> GetTimetablesForPrincipalAsync(int semesterId, string? status = null)
        {
            var timetables = await _repository.GetTimetablesForPrincipalAsync(semesterId, status);
            return _mapper.Map<IEnumerable<TimetableListDto>>(timetables);
        }

        public async Task<IEnumerable<TimetableListDto>> GetTimetablesBySemesterAsync(int semesterId)
        {
            var timetables = await _repository.GetTimetablesBySemesterAsync(semesterId);
            return _mapper.Map<IEnumerable<TimetableListDto>>(timetables);
        }
        public async Task<TimetableDto> UpdateTimetableInfoAsync(UpdateTimetableInfoDto dto)
        {
            var timetable = await _repository.GetByIdAsync(dto.TimetableId);
            if (timetable == null)
            {
                throw new KeyNotFoundException($"Timetable with ID {dto.TimetableId} not found.");
            }

            timetable.SemesterId = dto.SemesterId;
            timetable.EffectiveDate = dto.EffectiveDate;
            timetable.Status = dto.Status;

            await _repository.UpdateTimetableAsync(timetable);

            var updatedTimetable = await _repository.GetByIdAsync(dto.TimetableId);
            return _mapper.Map<TimetableDto>(updatedTimetable);
        }
        public async Task<bool> UpdateMultipleDetailsAsync(UpdateTimetableDetailsDto dto)
        {
            var timetable = await _repository.GetByIdAsync(dto.TimetableId);
            if (timetable == null)
            {
                throw new KeyNotFoundException($"Timetable with ID {dto.TimetableId} not found.");
            }

            var existingDetails = timetable.TimetableDetails.ToDictionary(td => td.TimetableDetailId);

            var detailsToUpdate = new List<TimetableDetail>();
            foreach (var detailDto in dto.Details)
            {
                if (!existingDetails.TryGetValue(detailDto.TimetableDetailId, out var detail))
                {
                    throw new KeyNotFoundException($"TimetableDetail with ID {detailDto.TimetableDetailId} not found in Timetable {dto.TimetableId}.");
                }

                detail.ClassId = detailDto.ClassId;
                detail.SubjectId = detailDto.SubjectId;
                detail.TeacherId = detailDto.TeacherId;
                detail.DayOfWeek = detailDto.DayOfWeek;
                detail.PeriodId = detailDto.PeriodId;

                if (await _repository.IsConflictAsync(detail))
                {
                    throw new InvalidOperationException($"Conflict detected for timetable detail ID {detail.TimetableDetailId} on {detail.DayOfWeek} at period {detail.PeriodId}.");
                }

                detailsToUpdate.Add(detail);
            }

            return await _repository.UpdateMultipleDetailsAsync(detailsToUpdate);
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