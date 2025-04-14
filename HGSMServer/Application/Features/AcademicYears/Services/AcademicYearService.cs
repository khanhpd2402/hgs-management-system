using Application.Features.AcademicYears.DTOs;
using Application.Features.AcademicYears.Interfaces;
using AutoMapper;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.AcademicYears.Services
{
    public class AcademicYearService : IAcademicYearService
    {
        private readonly IAcademicYearRepository _repository;
        private readonly ISemesterRepository _semesterRepository;
        private readonly IMapper _mapper;
        private readonly HgsdbContext _context; 

        public AcademicYearService(
            IAcademicYearRepository repository,
            ISemesterRepository semesterRepository,
            IMapper mapper,
            HgsdbContext context)
        {
            _repository = repository;
            _semesterRepository = semesterRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<List<AcademicYearDto>> GetAllAsync()
        {
            var academicYears = await _repository.GetAllAsync();
            return _mapper.Map<List<AcademicYearDto>>(academicYears);
        }

        public async Task<AcademicYearDto?> GetByIdAsync(int id)
        {
            var academicYear = await _repository.GetByIdAsync(id);
            return academicYear != null ? _mapper.Map<AcademicYearDto>(academicYear) : null;
        }

        public async Task AddAsync(CreateAcademicYearDto academicYearDto)
        {
            // Map DTO thành AcademicYear
            var academicYear = _mapper.Map<AcademicYear>(academicYearDto);

            // Kiểm tra dữ liệu AcademicYear
            if (academicYear.StartDate >= academicYear.EndDate)
            {
                throw new ArgumentException("Ngày bắt đầu năm học phải trước ngày kết thúc.");
            }

            // Kiểm tra trùng tên AcademicYear
            var existingAcademicYear = await _repository.GetByNameAsync(academicYear.YearName);
            if (existingAcademicYear != null)
            {
                throw new ArgumentException($"Năm học với tên '{academicYear.YearName}' đã tồn tại.");
            }

            // Tạo hai học kỳ
            var semester1 = new Semester
            {
                SemesterName = "Học kỳ 1",
                StartDate = academicYearDto.Semester1StartDate,
                EndDate = academicYearDto.Semester1EndDate
            };

            var semester2 = new Semester
            {
                SemesterName = "Học kỳ 2",
                StartDate = academicYearDto.Semester2StartDate,
                EndDate = academicYearDto.Semester2EndDate
            };

            // Kiểm tra tính hợp lệ của học kỳ
            ValidateSemesters(academicYear, semester1, semester2);

            // Sử dụng giao dịch của DbContext
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Thêm AcademicYear vào DbContext
                _context.AcademicYears.Add(academicYear);
                await _context.SaveChangesAsync(); // Lưu để lấy AcademicYearId

                // Gán AcademicYearId cho học kỳ
                semester1.AcademicYearId = academicYear.AcademicYearId;
                semester2.AcademicYearId = academicYear.AcademicYearId;

                // Thêm học kỳ vào DbContext
                _context.Semesters.Add(semester1);
                _context.Semesters.Add(semester2);
                await _context.SaveChangesAsync();

                // Cam kết giao dịch
                await transaction.CommitAsync();
            }
            catch
            {
                // Hoàn tác giao dịch nếu có lỗi
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Các phương thức khác giữ nguyên, chỉ cập nhật nếu cần giao dịch
        public async Task UpdateAsync(AcademicYearDto academicYearDto)
        {
            var academicYear = _mapper.Map<AcademicYear>(academicYearDto);

            if (academicYear.StartDate >= academicYear.EndDate)
            {
                throw new ArgumentException("Ngày bắt đầu năm học phải trước ngày kết thúc.");
            }

            var existingAcademicYear = await _repository.GetByNameAsync(academicYear.YearName);
            if (existingAcademicYear != null && existingAcademicYear.AcademicYearId != academicYear.AcademicYearId)
            {
                throw new ArgumentException($"Năm học với tên '{academicYear.YearName}' đã tồn tại.");
            }

            var semesters = await _semesterRepository.GetByAcademicYearIdAsync(academicYear.AcademicYearId);
            if (semesters.Count != 2)
            {
                throw new InvalidOperationException("Mỗi năm học phải có đúng 2 học kỳ (Học kỳ 1 và Học kỳ 2).");
            }

            var semester1 = semesters.FirstOrDefault(s => s.SemesterName == "Học kỳ 1");
            var semester2 = semesters.FirstOrDefault(s => s.SemesterName == "Học kỳ 2");

            if (semester1 == null || semester2 == null)
            {
                throw new InvalidOperationException("Học kỳ 1 hoặc Học kỳ 2 không tồn tại.");
            }

            semester1.StartDate = academicYear.StartDate;
            semester2.EndDate = academicYear.EndDate;

            ValidateSemesters(academicYear, semester1, semester2);

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.AcademicYears.Update(academicYear);
                _context.Semesters.Update(semester1);
                _context.Semesters.Update(semester2);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        private void ValidateSemesters(AcademicYear academicYear, Semester semester1, Semester semester2)
        {
            if (semester1.StartDate < academicYear.StartDate || semester1.EndDate > academicYear.EndDate)
            {
                throw new ArgumentException("Ngày của Học kỳ 1 phải nằm trong khoảng thời gian của năm học.");
            }

            if (semester2.StartDate < academicYear.StartDate || semester2.EndDate > academicYear.EndDate)
            {
                throw new ArgumentException("Ngày của Học kỳ 2 phải nằm trong khoảng thời gian của năm học.");
            }

            if (semester1.StartDate >= semester1.EndDate)
            {
                throw new ArgumentException("Ngày bắt đầu của Học kỳ 1 phải trước ngày kết thúc.");
            }

            if (semester2.StartDate >= semester2.EndDate)
            {
                throw new ArgumentException("Ngày bắt đầu của Học kỳ 2 phải trước ngày kết thúc.");
            }

            if (semester1.EndDate >= semester2.StartDate)
            {
                throw new ArgumentException("Ngày kết thúc của Học kỳ 1 phải trước ngày bắt đầu của Học kỳ 2.");
            }

            if (semester1.StartDate != academicYear.StartDate)
            {
                throw new ArgumentException("Ngày bắt đầu của Học kỳ 1 phải trùng với ngày bắt đầu của năm học.");
            }

            if (semester2.EndDate != academicYear.EndDate)
            {
                throw new ArgumentException("Ngày kết thúc của Học kỳ 2 phải trùng với ngày kết thúc của năm học.");
            }
        }
    }
}