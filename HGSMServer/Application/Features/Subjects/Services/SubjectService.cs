using Application.Features.Subjects.DTOs;
using Application.Features.Subjects.Interfaces;
using AutoMapper;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Subjects.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _repository;
        private readonly IMapper _mapper;

        public SubjectService(ISubjectRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SubjectDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<SubjectDto>>(entities);
        }

        public async Task<SubjectDto> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Subject with ID {id} not found.");
            }
            return _mapper.Map<SubjectDto>(entity);
        }

        public async Task<SubjectDto> CreateAsync(SubjectCreateAndUpdateDto dto)
        {
            var entity = _mapper.Map<Subject>(dto);
            var createdEntity = await _repository.CreateAsync(entity);
            return _mapper.Map<SubjectDto>(createdEntity);
        }

        public async Task<SubjectDto> UpdateAsync(int id, SubjectCreateAndUpdateDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"Subject with ID {id} not found.");

            _mapper.Map(dto, entity); // map ngược vào entity
            await _repository.UpdateAsync(entity);

            return _mapper.Map<SubjectDto>(entity);
        }


        public async Task DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Subject with ID {id} not found.");
            }
            await _repository.DeleteAsync(id);
        }
    }
}
