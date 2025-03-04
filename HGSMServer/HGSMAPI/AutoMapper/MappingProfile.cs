using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }
    }
}