//using Application.Features.Teachers.DTOs;
//using Application.Features.Teachers.Interfaces;
//using AutoMapper;
//using Domain.Models;
//using Infrastructure.Repositories.Interfaces;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Application.Features.Teachers.Services
//{
//    public class TeacherService : ITeacherService
//    {
//        private readonly ITeacherRepository _teacherRepository;
//        private readonly IMapper _mapper;

//        public TeacherService(ITeacherRepository teacherRepository, IMapper mapper)
//        {
//            _teacherRepository = teacherRepository;
//            _mapper = mapper;
//        }

//        public async Task<IEnumerable<TeacherListDto>> GetAllTeachersAsync()
//        {
//            var teachers = await _teacherRepository.GetAllAsync();
//            return _mapper.Map<IEnumerable<TeacherListDto>>(teachers);
//        }

//        public async Task<TeacherDetailDto?> GetTeacherByIdAsync(int id)
//        {
//            var teacher = await _teacherRepository.GetByIdAsync(id);
//            return teacher == null ? null : _mapper.Map<TeacherDetailDto>(teacher);
//        }

//        public async Task AddTeacherAsync(TeacherDetailDto teacherDto)
//        {
//            var teacher = _mapper.Map<Teacher>(teacherDto);
//            await _teacherRepository.AddAsync(teacher);
//        }

//        public async Task UpdateTeacherAsync(int id, TeacherDetailDto teacherDto)
//        {
//            var teacher = _mapper.Map<Teacher>(teacherDto);
//            teacher.TeacherId = id; // Giữ nguyên ID khi update
//            await _teacherRepository.UpdateAsync(teacher);
//        }

//        public async Task DeleteTeacherAsync(int id)
//        {
//            await _teacherRepository.DeleteAsync(id);
//        }
//        /// <summary>
//        /// Xuất danh sách giáo viên ra file Excel
//        /// </summary>
//        public async Task<byte[]> ExportTeachersToExcelAsync()
//        {
//            var teachers = await _teacherRepository.GetAllAsync();

//            //using var package = new ExcelPackage();
//            //var worksheet = package.Workbook.Worksheets.Add("Teachers");

//            // Header
//            worksheet.Cells[1, 1].Value = "ID";
//            worksheet.Cells[1, 2].Value = "Họ và Tên";
//            worksheet.Cells[1, 3].Value = "Ngày sinh";
//            worksheet.Cells[1, 4].Value = "Giới tính";
//            worksheet.Cells[1, 5].Value = "Email";
//            worksheet.Cells[1, 6].Value = "Số điện thoại";

//            // Dữ liệu
//            int row = 2;
//            foreach (var teacher in teachers)
//            {
//                worksheet.Cells[row, 1].Value = teacher.TeacherId;
//                worksheet.Cells[row, 2].Value = teacher.FullName;
//                worksheet.Cells[row, 3].Value = teacher.Dob.ToString("yyyy-MM-dd");
//                worksheet.Cells[row, 4].Value = teacher.Gender;
//                worksheet.Cells[row, 5].Value = teacher.User?.Email;
//                worksheet.Cells[row, 6].Value = teacher.User?.PhoneNumber;
//                row++;
//            }

//            return package.GetAsByteArray();
//        }

//        /// <summary>
//        /// Nhập danh sách giáo viên từ file Excel
//        /// </summary>
//        public async Task ImportTeachersFromExcelAsync(IFormFile file)
//        {
//            using var stream = new MemoryStream();
//            await file.CopyToAsync(stream);
//            using var package = new ExcelPackage(stream);

//            var worksheet = package.Workbook.Worksheets[0]; // Lấy sheet đầu tiên
//            int rowCount = worksheet.Dimension.Rows;

//            var teachers = new List<Teacher>();

//            for (int row = 2; row <= rowCount; row++)
//            {
//                var teacher = new Teacher
//                {
//                    FullName = worksheet.Cells[row, 2].Value.ToString(),
//                    Dob = DateOnly.Parse(worksheet.Cells[row, 3].Value.ToString()),
//                    Gender = worksheet.Cells[row, 4].Value.ToString(),
//                    User = new User
//                    {
//                        Email = worksheet.Cells[row, 5].Value.ToString(),
//                        PhoneNumber = worksheet.Cells[row, 6].Value.ToString(),
//                        RoleId = 2 // Mặc định giáo viên
//                    }
//                };
//                teachers.Add(teacher);
//            }

//            foreach (var teacher in teachers)
//            {
//                await _teacherRepository.AddAsync(teacher);
//            }
//        }
//    }
//}