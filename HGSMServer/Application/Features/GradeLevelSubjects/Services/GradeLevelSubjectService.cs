using Application.Features.GradeLevelSubjects.DTOs;
using Application.Features.GradeLevelSubjects.Interfaces;
using AutoMapper;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.GradeLevelSubjects.Services
{
    public class GradeLevelSubjectService : IGradeLevelSubjectService
    {
        private readonly IGradeLevelSubjectRepository _repository;
        private readonly IMapper _mapper;

        public GradeLevelSubjectService(IGradeLevelSubjectRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GradeLevelSubjectDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<GradeLevelSubjectDto>>(entities);
        }

        public async Task<GradeLevelSubjectDto> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return _mapper.Map<GradeLevelSubjectDto>(entity);
        }

        public async Task<GradeLevelSubjectCreateAndUpdateDto> CreateAsync(GradeLevelSubjectCreateAndUpdateDto dto)
        {
            var entity = new GradeLevelSubject
            {
                GradeLevelId = dto.GradeLevelId,
                SubjectId = dto.SubjectId,
                PeriodsPerWeekHki = dto.PeriodsPerWeekHKI,
                PeriodsPerWeekHkii = dto.PeriodsPerWeekHKII,
                ContinuousAssessmentsHki = dto.ContinuousAssessmentsHKI,
                ContinuousAssessmentsHkii = dto.ContinuousAssessmentsHKII,
                MidtermAssessments = dto.MidtermAssessments,
                FinalAssessments = dto.FinalAssessments
            };
            var createdEntity = await _repository.CreateAsync(entity);
            return _mapper.Map<GradeLevelSubjectCreateAndUpdateDto>(createdEntity);
        }

        public async Task<GradeLevelSubjectCreateAndUpdateDto> UpdateAsync(int id, GradeLevelSubjectCreateAndUpdateDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("GradeLevelSubject not found");

            _mapper.Map(dto, entity);
            await _repository.UpdateAsync(entity);
            return _mapper.Map<GradeLevelSubjectCreateAndUpdateDto>(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
        public async Task<IEnumerable<GradeLevelSubjectDto>> GetBySubjectIdAsync(int subjectId)
        {
            var entities = await _repository.GetBySubjectIdAsync(subjectId);
            return _mapper.Map<IEnumerable<GradeLevelSubjectDto>>(entities);
        }
    }
}
