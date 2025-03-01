using Application.Features.Students.DTOs;
using Application.Features.Students.Interfaces;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Students.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;

        public StudentService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<IEnumerable<StudentDTO>> GetAllStudentsAsync()
        {
            var students = await _studentRepository.GetAllAsync();
            return students.Select(s => new StudentDTO
            {
                StudentId = s.StudentId,
                FullName = s.FullName,
                Dob = s.Dob,
                Gender = s.Gender,
                GradeLevel = s.GradeLevel,
                ClassId = s.ClassId,
                Status = s.Status
            }).ToList();
        }

        public async Task<StudentDTO?> GetStudentByIdAsync(int id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null) return null;

            return new StudentDTO
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                Dob = student.Dob,
                Gender = student.Gender,
                GradeLevel = student.GradeLevel,
                ClassId = student.ClassId,
                Status = student.Status
            };
        }

        public async Task AddStudentAsync(StudentDTO studentDto)
        {
            var student = new Student
            {
                FullName = studentDto.FullName,
                Dob = studentDto.Dob,
                Gender = studentDto.Gender,
                GradeLevel = studentDto.GradeLevel,
                ClassId = studentDto.ClassId,
                Status = studentDto.Status
            };

            await _studentRepository.AddAsync(student);
        }

        public async Task UpdateStudentAsync(StudentDTO studentDto)
        {
            var student = await _studentRepository.GetByIdAsync(studentDto.StudentId);
            if (student == null) return;

            student.FullName = studentDto.FullName;
            student.Dob = studentDto.Dob;
            student.Gender = studentDto.Gender;
            student.GradeLevel = studentDto.GradeLevel;
            student.ClassId = studentDto.ClassId;
            student.Status = studentDto.Status;

            await _studentRepository.UpdateAsync(student);
        }

        public async Task DeleteStudentAsync(int id)
        {
            await _studentRepository.DeleteAsync(id);
        }
    }
}