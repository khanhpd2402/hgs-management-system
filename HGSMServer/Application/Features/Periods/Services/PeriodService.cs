using Application.Features.Periods.DTOs;
using Application.Features.Periods.Interfaces;
using AutoMapper;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;


namespace Application.Features.Periods.Services
{
    public class PeriodService : IPeriodService
    {
        private readonly IPeriodRepository _repository;
        private readonly IMapper _mapper;

        public PeriodService(IPeriodRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PeriodDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<PeriodDto>>(entities);
        }

        public async Task<PeriodDto> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return _mapper.Map<PeriodDto>(entity);
        }

        public async Task<PeriodCreateAndUpdateDto> CreateAsync(PeriodCreateAndUpdateDto dto)
        {
            if (dto.EndTime <= dto.StartTime)
                throw new ArgumentException("EndTime must be after StartTime");
            var entity = new Period
            {
                PeriodName = dto.PeriodName,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Shift = dto.Shift
            };

            var createdEntity = await _repository.CreateAsync(entity);
            return _mapper.Map<PeriodCreateAndUpdateDto>(createdEntity);
        }

        public async Task<PeriodCreateAndUpdateDto> UpdateAsync(int id, PeriodCreateAndUpdateDto dto)
        {
            if (dto.EndTime <= dto.StartTime)
                throw new ArgumentException("EndTime must be after StartTime");
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Period not found");

            entity.PeriodName = dto.PeriodName;
            entity.StartTime = dto.StartTime;
            entity.EndTime = dto.EndTime;
            entity.Shift = dto.Shift;

            await _repository.UpdateAsync(entity);
            return _mapper.Map<PeriodCreateAndUpdateDto>(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
