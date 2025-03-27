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
        private readonly ISubjectRepository _subjectRepository;
        private readonly IMapper _mapper;

        public SubjectService(ISubjectRepository subjectRepository, IMapper mapper)
        {
            _subjectRepository = subjectRepository;
            _mapper = mapper;
        }

        public async Task<List<SubjectDto>> GetAllSubjectsAsync()
        {
            var subjects = await _subjectRepository.GetAllSubjectsAsync();
            return _mapper.Map<List<SubjectDto>>(subjects);
        }

        public async Task<SubjectDto?> GetSubjectByIdAsync(int id)
        {
            var subject = await _subjectRepository.GetSubjectByIdAsync(id);
            return subject != null ? _mapper.Map<SubjectDto>(subject) : null;
        }

        public async Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto createDto)
        {
            var subject = _mapper.Map<Subject>(createDto);
            var createdSubject = await _subjectRepository.CreateSubjectAsync(subject);
            return _mapper.Map<SubjectDto>(createdSubject);
        }

        public async Task<bool> UpdateSubjectAsync(int id, UpdateSubjectDto updateDto)
        {
            var existingSubject = await _subjectRepository.GetSubjectByIdAsync(id);
            if (existingSubject == null) return false;

            _mapper.Map(updateDto, existingSubject);
            return await _subjectRepository.UpdateSubjectAsync(existingSubject);
        }

        public async Task<bool> DeleteSubjectAsync(int id)
        {
            return await _subjectRepository.DeleteSubjectAsync(id);
        }
    }
}
