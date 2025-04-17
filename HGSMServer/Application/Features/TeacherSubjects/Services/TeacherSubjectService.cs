using Application.Features.TeacherSubjects.DTOs;
using Application.Features.TeacherSubjects.Interfaces;
using AutoMapper;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
                throw new ArgumentException($"TeacherSubject with Id {id} does not exist.");
            }
            return _mapper.Map<TeacherSubjectDto>(teacherSubject);
        }

        public async Task<List<TeacherSubjectDto>> GetByTeacherIdAsync(int teacherId)
        {
            var teacher = await _teacherRepository.GetByIdAsync(teacherId);
            if (teacher == null)
            {
                throw new ArgumentException($"Teacher with Id {teacherId} does not exist.");
            }

            var teacherSubjects = await _repository.GetAllAsync();
            var filteredTeacherSubjects = teacherSubjects
                .Where(ts => ts.TeacherId == teacherId)
                .ToList();

            return _mapper.Map<List<TeacherSubjectDto>>(filteredTeacherSubjects);
        }

        public async Task CreateAsync(CreateTeacherSubjectDto dto)
        {
            var teacher = await _teacherRepository.GetByIdAsync(dto.TeacherId);
            if (teacher == null)
            {
                throw new ArgumentException($"Teacher with Id {dto.TeacherId} does not exist.");
            }

            var subject = await _subjectRepository.GetByIdAsync(dto.SubjectId);
            if (subject == null)
            {
                throw new ArgumentException($"Subject with Id {dto.SubjectId} does not exist.");
            }

            var existingTeacherSubject = await _repository.GetAllAsync();
            if (existingTeacherSubject.Any(ts => ts.TeacherId == dto.TeacherId && ts.SubjectId == dto.SubjectId))
            {
                throw new ArgumentException($"Teacher with Id {dto.TeacherId} is already assigned to Subject with Id {dto.SubjectId}.");
            }

            var teacherSubject = _mapper.Map<TeacherSubject>(dto);
            await _repository.AddAsync(teacherSubject);
        }

        public async Task UpdateAsync(UpdateTeacherSubjectDto dto)
        {
            var teacherSubject = await _repository.GetByIdAsync(dto.Id);
            if (teacherSubject == null)
            {
                throw new ArgumentException($"TeacherSubject with Id {dto.Id} does not exist.");
            }

            var teacher = await _teacherRepository.GetByIdAsync(dto.TeacherId);
            if (teacher == null)
            {
                throw new ArgumentException($"Teacher with Id {dto.TeacherId} does not exist.");
            }

            var subject = await _subjectRepository.GetByIdAsync(dto.SubjectId);
            if (subject == null)
            {
                throw new ArgumentException($"Subject with Id {dto.SubjectId} does not exist.");
            }

            var existingTeacherSubject = await _repository.GetAllAsync();
            if (existingTeacherSubject.Any(ts => ts.Id != dto.Id && ts.TeacherId == dto.TeacherId && ts.SubjectId == dto.SubjectId))
            {
                throw new ArgumentException($"Teacher with Id {dto.TeacherId} is already assigned to Subject with Id {dto.SubjectId}.");
            }

            _mapper.Map(dto, teacherSubject);
            await _repository.UpdateAsync(teacherSubject);
        }

        public async Task DeleteAsync(int id)
        {
            var teacherSubject = await _repository.GetByIdAsync(id);
            if (teacherSubject == null)
            {
                throw new ArgumentException($"TeacherSubject with Id {id} does not exist.");
            }

            await _repository.DeleteAsync(id);
        }
    }
}