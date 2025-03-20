using Application.Features.LeaveRequests.DTOs;
using Application.Features.LeaveRequests.Interfaces;
using AutoMapper;
using Domain.Models;
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
        private readonly ILeaveRequestRepository _repository;
        private readonly IMapper _mapper;

        public LeaveRequestService(ILeaveRequestRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<LeaveRequestDto>> GetAllAsync()
        {
            var leaveRequests = await _repository.GetAllAsync();
            return _mapper.Map<List<LeaveRequestDto>>(leaveRequests);
        }

        public async Task<LeaveRequestDto?> GetByIdAsync(int id)
        {
            var leaveRequest = await _repository.GetByIdAsync(id);
            return leaveRequest != null ? _mapper.Map<LeaveRequestDto>(leaveRequest) : null;
        }

        public async Task<List<LeaveRequestDto>> GetByTeacherIdAsync(int teacherId)
        {
            var leaveRequests = await _repository.GetByTeacherIdAsync(teacherId);
            return _mapper.Map<List<LeaveRequestDto>>(leaveRequests);
        }

        public async Task AddAsync(CreateLeaveRequestDto leaveRequestDto)
        {
            var leaveRequest = _mapper.Map<LeaveRequest>(leaveRequestDto);
            leaveRequest.RequestDate = DateOnly.FromDateTime(DateTime.UtcNow);
            leaveRequest.Status = "Pending"; // Mặc định là chờ duyệt
            await _repository.AddAsync(leaveRequest);
        }

        public async Task UpdateAsync(LeaveRequestDto leaveRequestDto)
        {
            var leaveRequest = _mapper.Map<LeaveRequest>(leaveRequestDto);
            await _repository.UpdateAsync(leaveRequest);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }

}
