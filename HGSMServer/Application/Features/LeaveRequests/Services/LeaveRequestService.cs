using Application.Features.LeaveRequests.DTOs;
using Application.Features.LeaveRequests.DTOs.Application.Features.LeaveRequests.DTOs;
using Application.Features.LeaveRequests.Interfaces;
using AutoMapper;
using Common.Constants;
using Domain.Models;
using Infrastructure.Repositories.Implementtations;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.LeaveRequests.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly ITimetableDetailRepository _timetableDetailRepository;
        private readonly IMapper _mapper;

        public LeaveRequestService(
            ILeaveRequestRepository leaveRequestRepository,
            ITeacherRepository teacherRepository,
            ITimetableDetailRepository timetableDetailRepository,
            IMapper mapper)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _teacherRepository = teacherRepository;
            _timetableDetailRepository = timetableDetailRepository;
            _mapper = mapper;
        }

        public async Task<LeaveRequestDetailDto?> GetByIdAsync(int id)
        {
            var entity = await _leaveRequestRepository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<LeaveRequestDetailDto>(entity);
        }

        public async Task<IEnumerable<LeaveRequestListDto>> GetAllAsync(int? teacherId = null, string? status = null)
        {
            var list = await _leaveRequestRepository.GetAllAsync(teacherId, status);
            return list.Select(_mapper.Map<LeaveRequestListDto>);
        }


        public async Task<LeaveRequestDetailDto> CreateAsync(CreateLeaveRequestDto dto)
        {
            var entity = _mapper.Map<LeaveRequest>(dto);
            entity.RequestDate = DateOnly.FromDateTime(DateTime.Today);
            entity.Status = AppConstants.Status.PENDING;
            await _leaveRequestRepository.AddAsync(entity);
            await _leaveRequestRepository.SaveAsync();
            return _mapper.Map<LeaveRequestDetailDto>(entity);
        }

        public async Task<bool> UpdateAsync(UpdateLeaveRequest dto)
        {
            var entity = await _leaveRequestRepository.GetByIdAsync(dto.RequestId);
            if (entity == null) return false;

            _mapper.Map(dto, entity);
            _leaveRequestRepository.Update(entity);
            await _leaveRequestRepository.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _leaveRequestRepository.GetByIdAsync(id);
            if (entity == null || entity.Status != AppConstants.Status.PENDING)
                return false;

            _leaveRequestRepository.Delete(entity);
            await _leaveRequestRepository.SaveAsync();
            return true;
        }

        public async Task<List<AvailableSubstituteTeacherDto>> FindAvailableSubstituteTeachersAsync(FindSubstituteTeacherRequestDto request)
        {
            if (request == null || request.OriginalTeacherId <= 0 || request.TimetableDetailId <= 0)
                throw new ArgumentException("Invalid request data: OriginalTeacherId and TimetableDetailId are required.");

            var timetableDetail = await _timetableDetailRepository.GetByIdAsync(request.TimetableDetailId);
            if (timetableDetail == null)
                throw new KeyNotFoundException("Timetable detail not found.");

            if (timetableDetail.TeacherId != request.OriginalTeacherId)
                throw new InvalidOperationException("The specified teacher is not scheduled for this timetable detail.");

            var subjectId = timetableDetail.SubjectId;
            var potentialTeachers = await _teacherRepository.GetTeachersBySubjectIdAsync(subjectId);

            potentialTeachers = potentialTeachers
                .Where(t => t.TeacherId != request.OriginalTeacherId)
                .ToList();

            var availableTeachers = new List<AvailableSubstituteTeacherDto>();
            foreach (var teacher in potentialTeachers)
            {
                var teacherSchedule = await _timetableDetailRepository.GetByTeacherAndTimeAsync(
                    teacher.TeacherId, timetableDetail.DayOfWeek, timetableDetail.PeriodId, timetableDetail.TimetableId);

                if (teacherSchedule == null)
                {
                    availableTeachers.Add(new AvailableSubstituteTeacherDto
                    {
                        TeacherId = teacher.TeacherId,
                        FullName = teacher.FullName
                    });
                }
            }

            return availableTeachers;
        }
        public async Task<List<AvailableSubstituteTeacherDto>> CheckAvailableTeachersAsync(FindSubstituteTeacherRequestDto request)
        {
            if (request == null || request.TimetableDetailId <= 0)
                throw new ArgumentException("Invalid request data: TimetableDetailId is required.");

            var timetableDetail = await _timetableDetailRepository.GetByIdAsync(request.TimetableDetailId);
            if (timetableDetail == null)
                throw new KeyNotFoundException("Timetable detail not found.");

            var allTeachers = await _teacherRepository.GetAllAsync();

            var availableTeachers = new List<AvailableSubstituteTeacherDto>();
            foreach (var teacher in allTeachers)
            {
                var teacherSchedule = await _timetableDetailRepository.GetByTeacherAndTimeAsync(
                    teacher.TeacherId, timetableDetail.DayOfWeek, timetableDetail.PeriodId, timetableDetail.TimetableId);

                if (teacherSchedule == null)
                {
                    availableTeachers.Add(new AvailableSubstituteTeacherDto
                    {
                        TeacherId = teacher.TeacherId,
                        FullName = teacher.FullName
                    });
                }
            }

            return availableTeachers;
        }
    }
}
