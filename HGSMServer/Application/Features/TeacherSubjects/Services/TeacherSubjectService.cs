using Application.Features.TeacherSubjects.DTOs;
using Application.Features.TeacherSubjects.Interfaces;
using AutoMapper;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.TeacherSubjects.Services
{
    public class TeacherSubjectService : ITeacherSubjectService
    {
        private readonly ITeacherSubjectRepository _repository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IMapper _mapper;

        public TeacherSubjectService(
            ITeacherSubjectRepository repository,
            ITeacherRepository teacherRepository,
            ISubjectRepository subjectRepository,
            IMapper mapper)
        {
            _repository = repository;
            _teacherRepository = teacherRepository;
            _subjectRepository = subjectRepository;
            _mapper = mapper;
        }

        public async Task<List<TeacherSubjectDto>> GetAllAsync()
        {
            var teacherSubjects = await _repository.GetAllAsync();
            return _mapper.Map<List<TeacherSubjectDto>>(teacherSubjects);
        }

        public async Task<TeacherSubjectDto> GetByIdAsync(int id)
        {
            var teacherSubject = await _repository.GetByIdAsync(id);
            if (teacherSubject == null)
            {
                throw new ArgumentException("TeacherSubject does not exist.");
            }
            return _mapper.Map<TeacherSubjectDto>(teacherSubject);
        }

        public async Task<List<TeacherSubjectDto>> GetByTeacherIdAsync(int teacherId)
        {
            var teacher = await _teacherRepository.GetByIdAsync(teacherId);
            if (teacher == null)
            {
                throw new ArgumentException("Teacher does not exist.");
            }

            var teacherSubjects = await _repository.GetByTeacherIdAsync(teacherId);
            return _mapper.Map<List<TeacherSubjectDto>>(teacherSubjects);
        }

        public async Task<List<TeacherSubjectDto>> GetBySubjectIdAsync(int subjectId)
        {
            var subject = await _subjectRepository.GetByIdAsync(subjectId);
            if (subject == null)
            {
                throw new ArgumentException("Subject does not exist.");
            }

            var teacherSubjects = await _repository.GetAllAsync();
            var filteredTeacherSubjects = teacherSubjects
                .Where(ts => ts.SubjectId == subjectId)
                .ToList();

            return _mapper.Map<List<TeacherSubjectDto>>(filteredTeacherSubjects);
        }

        public async Task CreateAsync(CreateTeacherSubjectDto dto)
        {
            var teacher = await _teacherRepository.GetByIdAsync(dto.TeacherId);
            if (teacher == null)
            {
                throw new ArgumentException("Teacher does not exist.");
            }

            var subject = await _subjectRepository.GetByIdAsync(dto.SubjectId);
            if (subject == null)
            {
                throw new ArgumentException("Subject does not exist.");
            }

            var existingTeacherSubject = await _repository.GetByTeacherIdAsync(dto.TeacherId);
            if (existingTeacherSubject.Any(ts => ts.SubjectId == dto.SubjectId))
            {
                throw new ArgumentException("Teacher is already assigned to this subject.");
            }

            var teacherSubject = _mapper.Map<TeacherSubject>(dto);
            await _repository.AddAsync(teacherSubject);
        }

        public async Task UpdateAsync(UpdateTeacherSubjectDto dto)
        {
            var teacher = await _teacherRepository.GetByIdAsync(dto.TeacherId);
            if (teacher == null)
            {
                throw new ArgumentException("Teacher does not exist.");
            }

            var existingTeacherSubjects = await _repository.GetByTeacherIdAsync(dto.TeacherId);
            var newSubjectIds = dto.Subjects.Select(s => s.SubjectId).ToList();

            // Xóa các môn học không còn trong danh sách mới
            var subjectsToRemove = existingTeacherSubjects
                .Where(ts => ts.SubjectId.HasValue && !newSubjectIds.Contains(ts.SubjectId.Value))
                .ToList();

            foreach (var subjectToRemove in subjectsToRemove)
            {
                await _repository.DeleteAsync(subjectToRemove.Id);
            }

            // Thêm hoặc cập nhật các môn học trong danh sách mới
            foreach (var subjectInfo in dto.Subjects)
            {
                var subject = await _subjectRepository.GetByIdAsync(subjectInfo.SubjectId);
                if (subject == null)
                {
                    throw new ArgumentException("Subject does not exist.");
                }

                var teacherSubject = existingTeacherSubjects
                    .FirstOrDefault(ts => ts.TeacherId == dto.TeacherId && ts.SubjectId == subjectInfo.SubjectId);

                if (teacherSubject != null)
                {
                    // Cập nhật IsMainSubject nếu môn học đã tồn tại
                    teacherSubject.IsMainSubject = subjectInfo.IsMainSubject;
                    await _repository.UpdateAsync(teacherSubject);
                }
                else
                {
                    // Thêm môn học mới
                    var newTeacherSubject = new TeacherSubject
                    {
                        TeacherId = dto.TeacherId,
                        SubjectId = subjectInfo.SubjectId,
                        IsMainSubject = subjectInfo.IsMainSubject
                    };
                    await _repository.AddAsync(newTeacherSubject);
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            var teacherSubject = await _repository.GetByIdAsync(id);
            if (teacherSubject == null)
            {
                throw new ArgumentException("TeacherSubject does not exist.");
            }

            await _repository.DeleteAsync(id);
        }

        public async Task DeleteByTeacherIdAsync(int teacherId)
        {
            var teacher = await _teacherRepository.GetByIdAsync(teacherId);
            if (teacher == null)
            {
                throw new ArgumentException("Teacher does not exist.");
            }

            var teacherSubjects = await _repository.GetByTeacherIdAsync(teacherId);
            if (!teacherSubjects.Any())
            {
                throw new ArgumentException("No teacher subjects found for this teacher.");
            }

            await _repository.DeleteByTeacherIdAsync(teacherId);
        }
    }
}