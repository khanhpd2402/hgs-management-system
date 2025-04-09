using Application.Features.GradeLevels.DTOs;
using Application.Features.GradeLevels.Interfaces;
using AutoMapper;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.GradeLevels.Services
{
    public class GradeLevelService : IGradeLevelService
    {
        private readonly IGradeLevelRepository _repository;
        private readonly IMapper _mapper;

        public GradeLevelService(IGradeLevelRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GradeLevelDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<GradeLevelDto>>(entities);
        }

        public async Task<GradeLevelDto> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return _mapper.Map<GradeLevelDto>(entity);
        }

        public async Task<GradeLevelCreateAndUpdateDto> CreateAsync(GradeLevelCreateAndUpdateDto dto)
        {
            var entity = new GradeLevel
            {
                GradeName = dto.GradeName
            };
            var createdEntity = await _repository.CreateAsync(entity);
            return new GradeLevelCreateAndUpdateDto
            {
                GradeName = createdEntity.GradeName
            };
        }

        public async Task<GradeLevelCreateAndUpdateDto> UpdateAsync(int id, GradeLevelCreateAndUpdateDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("GradeLevel not found");

            entity.GradeName = dto.GradeName;
            await _repository.UpdateAsync(entity);
            return new GradeLevelCreateAndUpdateDto
            {
                GradeName = entity.GradeName
            };
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
