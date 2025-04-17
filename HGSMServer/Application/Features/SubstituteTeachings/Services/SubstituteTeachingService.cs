using Application.Features.SubstituteTeachings.DTOs;
using Application.Features.SubstituteTeachings.Interfaces;
using AutoMapper;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;

namespace Application.Features.SubstituteTeachings.Services
{
    public class SubstituteTeachingService : ISubstituteTeachingService
    {
        private readonly ISubstituteTeachingRepository _repository;
        private readonly IMapper _mapper;

        public SubstituteTeachingService(ISubstituteTeachingRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SubstituteTeachingDto> CreateAsync(SubstituteTeachingCreateDto dto)
        {

            if (dto.OriginalTeacherId == dto.SubstituteTeacherId)
                throw new InvalidOperationException("Original teacher and substitute teacher cannot be the same.");

            if (dto.Date < DateOnly.FromDateTime(DateTime.Today))
                throw new InvalidOperationException("Substitute teaching date cannot be in the past.");

            var entity = _mapper.Map<SubstituteTeaching>(dto);
            var createdEntity = await _repository.CreateAsync(entity);
            return _mapper.Map<SubstituteTeachingDto>(createdEntity);
        }

        public async Task<SubstituteTeachingDto> GetByIdAsync(int substituteId)
        {
            var entity = await _repository.GetByIdAsync(substituteId);
            if (entity == null)
                throw new KeyNotFoundException($"SubstituteTeaching with ID {substituteId} not found.");

            return _mapper.Map<SubstituteTeachingDto>(entity);
        }

        public async Task<IEnumerable<SubstituteTeachingDto>> GetAllAsync(int? timetableDetailId = null, int? teacherId = null, DateOnly? date = null)
        {
            var entities = await _repository.GetAllAsync(timetableDetailId, teacherId, date);
            return _mapper.Map<IEnumerable<SubstituteTeachingDto>>(entities);
        }

        public async Task<SubstituteTeachingDto> UpdateAsync(SubstituteTeachingUpdateDto dto)
        {
            var existingEntity = await _repository.GetByIdAsync(dto.SubstituteId);
            if (existingEntity == null)
                throw new KeyNotFoundException($"SubstituteTeaching with ID {dto.SubstituteId} not found.");

            if (dto.OriginalTeacherId == dto.SubstituteTeacherId)
                throw new InvalidOperationException("Original teacher and substitute teacher cannot be the same.");

            if (dto.Date < DateOnly.FromDateTime(DateTime.Today))
                throw new InvalidOperationException("Substitute teaching date cannot be in the past.");

            _mapper.Map(dto, existingEntity);
            await _repository.UpdateAsync(existingEntity);
            return _mapper.Map<SubstituteTeachingDto>(existingEntity);
        }

        public async Task DeleteAsync(int substituteId)
        {
            var entity = await _repository.GetByIdAsync(substituteId);
            if (entity == null)
                throw new KeyNotFoundException($"SubstituteTeaching with ID {substituteId} not found.");

            await _repository.DeleteAsync(substituteId);
        }
    }
}
