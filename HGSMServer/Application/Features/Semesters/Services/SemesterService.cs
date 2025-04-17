using Application.Features.AcademicYears.DTOs;
using Application.Features.Semesters.DTOs;
using Application.Features.Semesters.Interfaces;
using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Semesters.Services
{
    public class SemesterService : ISemesterService
    {
        private readonly ISemesterRepository _repository;
        private readonly IAcademicYearRepository _academicYearRepository; 
        private readonly IMapper _mapper;

        public SemesterService(
            ISemesterRepository repository,
            IAcademicYearRepository academicYearRepository,
            IMapper mapper)
        {
            _repository = repository;
            _academicYearRepository = academicYearRepository;
            _mapper = mapper;
        }

        public async Task<List<SemesterDto>> GetByAcademicYearIdAsync(int academicYearId)
        {
            var semesters = await _repository.GetByAcademicYearIdAsync(academicYearId);
            return _mapper.Map<List<SemesterDto>>(semesters);
        }

        public async Task<SemesterDto?> GetByIdAsync(int id)
        {
            var semester = await _repository.GetByIdAsync(id);
            return semester != null ? _mapper.Map<SemesterDto>(semester) : null;
        }
        public async Task<List<SemesterDto>> GetAllSemester()
        {
            var semester = await _repository.GetAllAsync();
            return _mapper.Map<List<SemesterDto>>(semester);
        }

        public async Task AddAsync(CreateSemesterDto semesterDto)
        {
            // Lấy thông tin AcademicYear
            var academicYear = await _academicYearRepository.GetByIdAsync(semesterDto.AcademicYearID);
            if (academicYear == null)
            {
                throw new ArgumentException($"Năm học với ID {semesterDto.AcademicYearID} không tồn tại.");
            }

            // Map DTO thành entity Semester
            var semester = _mapper.Map<Semester>(semesterDto);

            // Validate dữ liệu Semester
            await ValidateSemester(semester, academicYear);

            await _repository.AddAsync(semester);
        }

        public async Task UpdateAsync(SemesterDto semesterDto)
        {
            // Lấy thông tin AcademicYear
            var academicYear = await _academicYearRepository.GetByIdAsync(semesterDto.AcademicYearID);
            if (academicYear == null)
            {
                throw new ArgumentException($"Năm học với ID {semesterDto.AcademicYearID} không tồn tại.");
            }

            // Map DTO thành entity Semester
            var semester = _mapper.Map<Semester>(semesterDto);

            // Validate dữ liệu Semester
            await ValidateSemester(semester, academicYear);

            await _repository.UpdateAsync(semester);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        private async Task ValidateSemester(Semester semester, AcademicYear academicYear)
        {
            // Validate: StartDate phải trước EndDate của chính Semester
            if (semester.StartDate >= semester.EndDate)
            {
                throw new ArgumentException("Ngày bắt đầu của học kỳ phải trước ngày kết thúc.");
            }

            // Validate: StartDate và EndDate của học kỳ phải nằm trong khoảng thời gian của AcademicYear
            if (semester.StartDate < academicYear.StartDate || semester.EndDate > academicYear.EndDate)
            {
                throw new ArgumentException("Ngày của học kỳ phải nằm trong khoảng thời gian của năm học.");
            }

            // Validate: Nếu là Học kỳ 1, StartDate phải trùng với StartDate của AcademicYear
            if (semester.SemesterName == "Học kỳ 1" && semester.StartDate != academicYear.StartDate)
            {
                throw new ArgumentException("Ngày bắt đầu của Học kỳ 1 phải trùng với ngày bắt đầu của năm học.");
            }

            // Validate: Nếu là Học kỳ 2, EndDate phải trùng với EndDate của AcademicYear
            if (semester.SemesterName == "Học kỳ 2" && semester.EndDate != academicYear.EndDate)
            {
                throw new ArgumentException("Ngày kết thúc của Học kỳ 2 phải trùng với ngày kết thúc của năm học.");
            }

            // Validate: EndDate của Học kỳ 1 phải trước StartDate của Học kỳ 2
            var otherSemester = (await _repository.GetByAcademicYearIdAsync(semester.AcademicYearId))
                .FirstOrDefault(s => s.SemesterId != semester.SemesterId);

            if (otherSemester != null)
            {
                if (semester.SemesterName == "Học kỳ 1" && otherSemester.SemesterName == "Học kỳ 2")
                {
                    if (semester.EndDate >= otherSemester.StartDate)
                    {
                        throw new ArgumentException("Ngày kết thúc của Học kỳ 1 phải trước ngày bắt đầu của Học kỳ 2.");
                    }
                }
                else if (semester.SemesterName == "Học kỳ 2" && otherSemester.SemesterName == "Học kỳ 1")
                {
                    if (otherSemester.EndDate >= semester.StartDate)
                    {
                        throw new ArgumentException("Ngày kết thúc của Học kỳ 1 phải trước ngày bắt đầu của Học kỳ 2.");
                    }
                }
            }
        }

        
    }
}