using Application.Features.Classes.DTOs;
using Application.Features.Grades.DTOs;
using Application.Features.GradeBatchs.DTOs;
using Application.Features.Students.DTOs;
using Application.Features.Teachers.DTOs;
using AutoMapper;
using Domain.Models;
namespace HGSMAPI.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TeacherDetailDto, Teacher>()
     .ForMember(dest => dest.User, opt => opt.Ignore()); // Nếu User không cần map

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
            .FirstOrDefault()));

            CreateMap<Teacher, TeacherExportDto>();
            CreateMap<Class, ClassDto>();
            CreateMap<ClassDto, Class>();
            CreateMap<GradeBatch, GradeBatchDto>().ReverseMap();
            CreateMap<GradeDto, Grade>().ReverseMap();
            CreateMap<Grade, GradeRespondDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Student.FullName))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.Student.Dob));
        }
    }
}
