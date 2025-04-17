using Application.Features.Conducts.DTOs;
using Application.Features.Conducts.Interfaces;
using AutoMapper;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Conducts.Services
{
    public class ConductService : IConductService
    {
        private readonly IConductRepository _conductRepository;
        private readonly IMapper _mapper;

        public ConductService(IConductRepository conductRepository, IMapper mapper)
        {
            _conductRepository = conductRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ConductDto>> GetAllAsync()
        {
            var conducts = await _conductRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ConductDto>>(conducts);
        }

        public async Task<ConductDto> GetByIdAsync(int id)
        {
            var conduct = await _conductRepository.GetByIdAsync(id);
            return _mapper.Map<ConductDto>(conduct);
        }

        public async Task<ConductDto> CreateAsync(CreateConductDto dto)
        {
            var conduct = _mapper.Map<Conduct>(dto);
            var createdConduct = await _conductRepository.CreateAsync(conduct);
            return _mapper.Map<ConductDto>(createdConduct);
        }

        public async Task<ConductDto> UpdateAsync(int id, UpdateConductDto dto)
        {
            var existingConduct = await _conductRepository.GetByIdAsync(id);
            if (existingConduct == null)
            {
                throw new KeyNotFoundException($"Conduct with ID {id} not found.");
            }

            _mapper.Map(dto, existingConduct);
            await _conductRepository.UpdateAsync(id, existingConduct);

            return _mapper.Map<ConductDto>(existingConduct);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _conductRepository.DeleteAsync(id);
        }
    }
}
