using Application.Features.AcademicYears.DTOs;
using Application.Features.Semesters.DTOs;
using Application.Features.Semesters.Interfaces;
using AutoMapper;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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
            try
            {
                var semesters = await _repository.GetByAcademicYearIdAsync(academicYearId);
                return _mapper.Map<List<SemesterDto>>(semesters);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể lấy danh sách học kỳ theo năm học do lỗi hệ thống.", ex);
            }
        }

        public async Task<SemesterDto?> GetByIdAsync(int id)
        {
            var semester = await _repository.GetByIdAsync(id);
            if (semester == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy học kỳ với ID {id}.");
            }
            return _mapper.Map<SemesterDto>(semester);
        }

        public async Task<List<SemesterDto>> GetAllSemester()
        {
            try
            {
                var semesters = await _repository.GetAllAsync();
                return _mapper.Map<List<SemesterDto>>(semesters);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể lấy danh sách học kỳ do lỗi hệ thống.", ex);
            }
        }

        public async Task AddAsync(CreateSemesterDto semesterDto)
        {
            if (semesterDto == null)
            {
                throw new ArgumentNullException(nameof(semesterDto), "Thông tin học kỳ không được để trống.");
            }

            var academicYear = await _academicYearRepository.GetByIdAsync(semesterDto.AcademicYearID);
            if (academicYear == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy năm học với ID {semesterDto.AcademicYearID}.");
            }

            var semester = _mapper.Map<Semester>(semesterDto);

            await ValidateSemester(semester, academicYear);

            try
            {
                await _repository.AddAsync(semester);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể thêm học kỳ do lỗi hệ thống.", ex);
            }
        }

        public async Task UpdateAsync(SemesterDto semesterDto)
        {
            if (semesterDto == null)
            {
                throw new ArgumentNullException(nameof(semesterDto), "Thông tin học kỳ không được để trống.");
            }

            var existingSemester = await _repository.GetByIdAsync(semesterDto.SemesterID);
            if (existingSemester == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy học kỳ với ID {semesterDto.SemesterID} để cập nhật.");
            }

            var academicYear = await _academicYearRepository.GetByIdAsync(semesterDto.AcademicYearID);
            if (academicYear == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy năm học với ID {semesterDto.AcademicYearID}.");
            }

            var semester = _mapper.Map<Semester>(semesterDto);

            await ValidateSemester(semester, academicYear);

            try
            {
                await _repository.UpdateAsync(semester);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể cập nhật học kỳ do lỗi hệ thống.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            var semester = await _repository.GetByIdAsync(id);
            if (semester == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy học kỳ với ID {id} để xóa.");
            }

            try
            {
                await _repository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Không thể xóa học kỳ do lỗi hệ thống.", ex);
            }
        }

        private async Task ValidateSemester(Semester semester, AcademicYear academicYear)
        {
            if (semester.StartDate >= semester.EndDate)
            {
                throw new ArgumentException("Ngày bắt đầu của học kỳ phải trước ngày kết thúc.");
            }

            if (semester.StartDate < academicYear.StartDate || semester.EndDate > academicYear.EndDate)
            {
                throw new ArgumentException("Ngày của học kỳ phải nằm trong khoảng thời gian của năm học.");
            }

            if (semester.SemesterName == "Học kỳ 1" && semester.StartDate != academicYear.StartDate)
            {
                throw new ArgumentException("Ngày bắt đầu của Học kỳ 1 phải trùng với ngày bắt đầu của năm học.");
            }

            if (semester.SemesterName == "Học kỳ 2" && semester.EndDate != academicYear.EndDate)
            {
                throw new ArgumentException("Ngày kết thúc của Học kỳ 2 phải trùng với ngày kết thúc của năm học.");
            }

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