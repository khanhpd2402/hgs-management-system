using Application.Features.LeaveRequests.DTOs;
using Application.Features.LeaveRequests.Interfaces;
using AutoMapper;
using Common.Constants;
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

        public async Task<LeaveRequestDetailDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<LeaveRequestDetailDto>(entity);
        }

        public async Task<IEnumerable<LeaveRequestListDto>> GetAllAsync(int? teacherId = null, string? status = null)
        {
            var list = await _repository.GetAllAsync(teacherId, status);
            return list.Select(_mapper.Map<LeaveRequestListDto>);
        }


        public async Task<LeaveRequestDetailDto> CreateAsync(CreateLeaveRequestDto dto)
        {
            var entity = _mapper.Map<LeaveRequest>(dto);
            entity.RequestDate = DateOnly.FromDateTime(DateTime.Today);
            entity.Status = AppConstants.Status.PENDING;
            await _repository.AddAsync(entity);
            await _repository.SaveAsync();
            return _mapper.Map<LeaveRequestDetailDto>(entity);
        }

        public async Task<bool> UpdateAsync(UpdateLeaveRequest dto)
        {
            var entity = await _repository.GetByIdAsync(dto.RequestId);
            if (entity == null) return false;

            _mapper.Map(dto, entity);
            _repository.Update(entity);
            await _repository.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null || entity.Status != AppConstants.Status.PENDING)
                return false;

            _repository.Delete(entity);
            await _repository.SaveAsync();
            return true;
        }
    }

}
