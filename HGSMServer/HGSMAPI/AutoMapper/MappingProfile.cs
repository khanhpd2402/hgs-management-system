using Application.Features.Classes.DTOs;
using Application.Features.Grades.DTOs;
using Application.Features.GradeBatchs.DTOs;
using Application.Features.Students.DTOs;
using Application.Features.Teachers.DTOs;
using AutoMapper;
using Domain.Models;
using Application.Features.Subjects.DTOs;
using Application.Features.Parents.DTOs;
using Application.Features.AcademicYears.DTOs;
using Application.Features.Semesters.DTOs;
using Application.Features.LeaveRequests.DTOs;
namespace HGSMAPI.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TeacherDetailDto, Teacher>()
     .ForMember(dest => dest.User, opt => opt.Ignore());

            CreateMap<Teacher, TeacherDetailDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User != null ? src.User.UserId : (int?)null));
            CreateMap<Student, StudentDto>().ReverseMap();
            CreateMap<Teacher, TeacherListDto>()
                .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => src.Dob))
                .ForMember(dest => dest.HiringDate, opt => opt.MapFrom(src => src.HiringDate.HasValue ? src.HiringDate.Value : (DateOnly?)null))
                .ForMember(dest => dest.PermanentEmploymentDate, opt => opt.MapFrom(src => src.PermanentEmploymentDate.HasValue ? src.PermanentEmploymentDate.Value : (DateOnly?)null))
                .ForMember(dest => dest.SchoolJoinDate, opt => opt.MapFrom(src => src.SchoolJoinDate))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User != null ? src.User.Email ?? string.Empty : string.Empty))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User != null ? src.User.PhoneNumber ?? string.Empty : string.Empty)).ReverseMap();

            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.ClassName,
                    opt => opt.MapFrom(src => src.StudentClasses
                        .Select(sc => sc.Class.ClassName)
                        .FirstOrDefault()))
                .ForMember(dest => dest.Grade,
                    opt => opt.MapFrom(src => src.StudentClasses
                        .Select(sc => sc.Class.Grade)
                        .FirstOrDefault()))
                .ForMember(dest => dest.Parents,
                    opt => opt.MapFrom(src => src.StudentParents)) // Ánh xạ từ StudentParents
                .ReverseMap();

            // Thêm ánh xạ cho StudentParent
            CreateMap<StudentParent, ParentDto>()
                .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.Parent.ParentId))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Parent.FullName))
                .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => src.Parent.Dob))
                .ForMember(dest => dest.Occupation, opt => opt.MapFrom(src => src.Parent.Occupation))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Parent.User != null ? src.Parent.User.PhoneNumber : null))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Parent.User != null ? src.Parent.User.Email : null))
                .ForMember(dest => dest.Relationship, opt => opt.MapFrom(src => src.Relationship)) // Thêm Relationship
                .ReverseMap();
            CreateMap<CreateStudentDto, Student>();

            CreateMap<UpdateStudentDto, Student>()
                .ForMember(dest => dest.StudentId, opt => opt.Ignore());

            CreateMap<Class, ClassDto>().ReverseMap();
            CreateMap<GradeBatch, GradeBatchDto>().ReverseMap();

            CreateMap<GradeDto, Grade>().ReverseMap();
            CreateMap<Grade, GradeRespondDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Student.FullName))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.Student.Dob));

            CreateMap<Subject, SubjectDto>();
            CreateMap<CreateSubjectDto, Subject>();
            CreateMap<UpdateSubjectDto, Subject>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<AcademicYear, AcademicYearDto>().ReverseMap();
            CreateMap<CreateAcademicYearDto, AcademicYear>();

            CreateMap<Semester, SemesterDto>().ReverseMap();
            CreateMap<CreateSemesterDto, Semester>();

            CreateMap<LeaveRequest, LeaveRequestDto>().ReverseMap();
            CreateMap<CreateLeaveRequestDto, LeaveRequest>();
        }
    }
}

