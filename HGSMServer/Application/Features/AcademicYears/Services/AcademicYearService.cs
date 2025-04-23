// File: AcademicYearService.cs
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

        // Constructor và các hàm GetAllAsync, GetByIdAsync, AddAsync, DeleteAsync giữ nguyên như trước
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
            var academicYear = _mapper.Map<AcademicYear>(academicYearDto);

            if (academicYear.StartDate >= academicYear.EndDate)
            {
                throw new ArgumentException("Ngày bắt đầu năm học phải trước ngày kết thúc.");
            }

            var existingAcademicYear = await _repository.GetByNameAsync(academicYear.YearName);
            if (existingAcademicYear != null)
            {
                throw new ArgumentException($"Năm học với tên '{academicYear.YearName}' đã tồn tại.");
            }

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

            ValidateSemesters(academicYear, semester1, semester2);

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.AcademicYears.Add(academicYear);
                await _context.SaveChangesAsync();

                semester1.AcademicYearId = academicYear.AcademicYearId;
                semester2.AcademicYearId = academicYear.AcademicYearId;

                _context.Semesters.Add(semester1);
                _context.Semesters.Add(semester2);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // ---- Sửa đổi hàm UpdateAsync ----
        public async Task UpdateAsync(UpdateAcademicYearDto academicYearDto) // Thay đổi tham số
        {
            // Tìm năm học hiện có bằng Id từ DTO
            var academicYear = await _context.AcademicYears
                                             .FirstOrDefaultAsync(ay => ay.AcademicYearId == academicYearDto.AcademicYearId);

            if (academicYear == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy năm học với ID {academicYearDto.AcademicYearId}.");
            }

            // Kiểm tra ràng buộc ngày tháng của năm học từ DTO
            if (academicYearDto.StartDate >= academicYearDto.EndDate)
            {
                throw new ArgumentException("Ngày bắt đầu năm học phải trước ngày kết thúc.");
            }

            // Kiểm tra trùng tên (chỉ kiểm tra với các năm học khác)
            var existingAcademicYear = await _repository.GetByNameAsync(academicYearDto.YearName);
            if (existingAcademicYear != null && existingAcademicYear.AcademicYearId != academicYearDto.AcademicYearId)
            {
                throw new ArgumentException($"Năm học với tên '{academicYearDto.YearName}' đã tồn tại.");
            }

            // Tìm các học kỳ hiện có của năm học này
            var semesters = await _semesterRepository.GetByAcademicYearIdAsync(academicYearDto.AcademicYearId);
            var semester1 = semesters.FirstOrDefault(s => s.SemesterName == "Học kỳ 1");
            var semester2 = semesters.FirstOrDefault(s => s.SemesterName == "Học kỳ 2");

            if (semester1 == null || semester2 == null)
            {
                // Xử lý trường hợp không tìm thấy đủ 2 học kỳ
                throw new InvalidOperationException($"Không tìm thấy đầy đủ Học kỳ 1 và Học kỳ 2 cho năm học ID {academicYearDto.AcademicYearId}.");
            }
            academicYear.YearName = academicYearDto.YearName;
            academicYear.StartDate = academicYearDto.StartDate;
            academicYear.EndDate = academicYearDto.EndDate;

            // Cập nhật thông tin học kỳ
            semester1.StartDate = academicYearDto.Semester1StartDate;
            semester1.EndDate = academicYearDto.Semester1EndDate;
            semester2.StartDate = academicYearDto.Semester2StartDate;
            semester2.EndDate = academicYearDto.Semester2EndDate;
       
            ValidateSemesters(academicYear, semester1, semester2);

            // Thực hiện cập nhật trong transaction
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
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
            // Nên dùng transaction ở đây để đảm bảo xóa cả năm học và học kỳ liên quan
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var semestersToDelete = await _semesterRepository.GetByAcademicYearIdAsync(id);
                if (semestersToDelete.Any())
                {
                    _context.Semesters.RemoveRange(semestersToDelete);
                }

                var academicYearToDelete = await _context.AcademicYears.FindAsync(id);
                if (academicYearToDelete != null)
                {
                    _context.AcademicYears.Remove(academicYearToDelete);
                }
                else
                {
                    // Nếu không tìm thấy năm học, có thể rollback hoặc bỏ qua tùy logic
                    await transaction.RollbackAsync();
                    throw new KeyNotFoundException($"Không tìm thấy năm học với ID {id} để xóa.");
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            // await _repository.DeleteAsync(id); // Lệnh này có thể không đủ nếu không xử lý transaction và xóa học kỳ
        }


        // Hàm ValidateSemesters giữ nguyên như trước, nhưng cần đảm bảo logic phù hợp
        // với việc có thể cập nhật ngày bắt đầu/kết thúc học kỳ độc lập hơn
        private void ValidateSemesters(AcademicYear academicYear, Semester semester1, Semester semester2)
        {
            // --- Kiểm tra các ràng buộc ngày tháng ---
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