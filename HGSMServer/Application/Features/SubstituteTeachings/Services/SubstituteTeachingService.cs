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

        public async Task<SubstituteTeachingDto> CreateOrUpdateAsync(SubstituteTeachingCreateDto dto)
        {
            if (dto.OriginalTeacherId == dto.SubstituteTeacherId)
                throw new InvalidOperationException("Original teacher and substitute teacher cannot be the same.");

            if (dto.Date < DateOnly.FromDateTime(DateTime.Today))
                throw new InvalidOperationException("Substitute teaching date cannot be in the past.");

            // Check xem tiết học đã có người dạy thay chưa
            var existing = await _repository.GetByTimetableDetailAndDateAsync(dto.TimetableDetailId, dto.Date);
            if (existing == null)
            {
                var entity = _mapper.Map<SubstituteTeaching>(dto);
                entity.CreatedAt = DateTime.Now;
                var created = await _repository.CreateAsync(entity);
                return _mapper.Map<SubstituteTeachingDto>(created);
            }
            else
            {
                // Cho phép cập nhật người dạy thay + ghi chú
                existing.SubstituteTeacherId = dto.SubstituteTeacherId;
                existing.Note = dto.Note;
                await _repository.UpdateAsync(existing);
                return _mapper.Map<SubstituteTeachingDto>(existing);
            }
        }


        public async Task<SubstituteTeachingDto> GetByIdAsync(int substituteId)
        {
            var entity = await _repository.GetByIdAsync(substituteId);
            if (entity == null)
                throw new KeyNotFoundException($"SubstituteTeaching with ID {substituteId} not found.");

            return _mapper.Map<SubstituteTeachingDto>(entity);
        }

        public async Task<IEnumerable<SubstituteTeachingDto>> GetAllAsync(int? timetableDetailId = null, int? OriginalTeacherId = null, int? SubstituteTeacherId = null, DateOnly? date = null)
        {
            var entities = await _repository.GetAllAsync(timetableDetailId, OriginalTeacherId, SubstituteTeacherId ,date);
            return _mapper.Map<IEnumerable<SubstituteTeachingDto>>(entities);
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
