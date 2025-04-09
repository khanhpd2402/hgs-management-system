using Application.Features.Classes.DTOs;
using Application.Features.Classes.Interfaces;
using AutoMapper;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Classes.Services
{
    public class ClassService : IClassService
    {
        private readonly IClassRepository _classRepository;
        private readonly IMapper _mapper;

        public ClassService(IClassRepository classRepository, IMapper mapper)
        {
            _classRepository = classRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ClassDto>> GetAllClassesAsync()
        {
            var classes = await _classRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ClassDto>>(classes);
        }

        public async Task<ClassDto> GetClassByIdAsync(int id)
        {
            var classEntity = await _classRepository.GetByIdAsync(id);
            return _mapper.Map<ClassDto>(classEntity);
        }

        public async Task<ClassDto> CreateClassAsync(ClassDto classDto)
        {
            if (string.IsNullOrEmpty(classDto.ClassName))
                throw new ArgumentException("Class name is required");

            var classEntity = _mapper.Map<Class>(classDto);
            var createdEntity = await _classRepository.AddAsync(classEntity);
            return _mapper.Map<ClassDto>(createdEntity);
        }

        public async Task<ClassDto> UpdateClassAsync(int id, ClassDto classDto)
        {
            var existingClass = await _classRepository.GetByIdAsync(id);

            // Map DTO vào entity
            _mapper.Map(classDto, existingClass);

            var updatedEntity = await _classRepository.UpdateAsync(existingClass);
            return _mapper.Map<ClassDto>(updatedEntity);
        }

        public async Task DeleteClassAsync(int id)
        {
            await _classRepository.DeleteAsync(id);
        }
    }
}
