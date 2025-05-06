using Application.Features.Classes.DTOs;
using Application.Features.Classes.Interfaces;
using AutoMapper;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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
            try
            {
                var classes = await _classRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<ClassDto>>(classes);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể lấy danh sách lớp học do lỗi hệ thống.", ex);
            }
        }

        public async Task<IEnumerable<ClassDto>> GetAllClassesActiveAsync(string? status = null)
        {
            try
            {
                var classes = await _classRepository.GetAllActiveAsync(status);
                return _mapper.Map<IEnumerable<ClassDto>>(classes);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể lấy danh sách lớp học đang hoạt động do lỗi hệ thống.", ex);
            }
        }

        public async Task<ClassDto> GetClassByIdAsync(int id)
        {
            var classEntity = await _classRepository.GetByIdAsync(id);
            if (classEntity == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy lớp học với ID {id}.");
            }
            return _mapper.Map<ClassDto>(classEntity);
        }

        public async Task<ClassDto> CreateClassAsync(ClassDto classDto)
        {
            if (string.IsNullOrEmpty(classDto.ClassName))
            {
                throw new ArgumentException("Tên lớp học không được để trống.");
            }

            try
            {
                var classEntity = _mapper.Map<Class>(classDto);
                var createdEntity = await _classRepository.AddAsync(classEntity);
                return _mapper.Map<ClassDto>(createdEntity);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể tạo lớp học do lỗi hệ thống.", ex);
            }
        }

        public async Task<ClassDto> UpdateClassAsync(int id, ClassDto classDto)
        {
            var existingClass = await _classRepository.GetByIdAsync(id);
            var existingClass = await _classRepository.GetByIdAsync(id);
            var allclass = await _classRepository.GetAllAsync();
            foreach (var classEntity in allclass) {
                if (classDto.ClassName == classEntity.ClassName) {
                throw new KeyNotFoundException($"Khong the trung ten lop da ton tai.");
    }
}
            if (existingClass == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy lớp học với ID {id} để cập nhật.");
            }

            if (string.IsNullOrEmpty(classDto.ClassName))
            {
                throw new ArgumentException("Tên lớp học không được để trống.");
            }

            try
            {
                _mapper.Map(classDto, existingClass);
                var updatedEntity = await _classRepository.UpdateAsync(existingClass);
                return _mapper.Map<ClassDto>(updatedEntity);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể cập nhật lớp học do lỗi hệ thống.", ex);
            }
        }

        public async Task DeleteClassAsync(int id)
        {
            var existingClass = await _classRepository.GetByIdAsync(id);
            if (existingClass == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy lớp học với ID {id} để xóa.");
            }

            try
            {
                await _classRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể xóa lớp học do lỗi hệ thống.", ex);
            }
        }
    }
}
