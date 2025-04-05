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
using Application.Features.LessonPlans.DTOs;
using Application.Features.Exams.DTOs;
using Application.Features.Timetables.DTOs;

namespace HGSMAPI.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Ánh xạ cho TeacherDetailDto sang Teacher
            CreateMap<TeacherDetailDto, Teacher>()
                .ForMember(dest => dest.User, opt => opt.Ignore());

            // Ánh xạ Teacher sang TeacherDetailDto
            CreateMap<Teacher, TeacherDetailDto>()
                .ForMember(dest => dest.TeacherId, opt => opt.MapFrom(src => src.TeacherId))
                .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => src.Dob)) // Gán trực tiếp DateOnly
                .ForMember(dest => dest.HiringDate, opt => opt.MapFrom(src => src.HiringDate)) // Gán trực tiếp DateOnly?
                .ForMember(dest => dest.PermanentEmploymentDate, opt => opt.MapFrom(src => src.PermanentEmploymentDate)) // Gán trực tiếp DateOnly?
                .ForMember(dest => dest.SchoolJoinDate, opt => opt.MapFrom(src => src.SchoolJoinDate)); // Gán trực tiếp DateOnly

            CreateMap<TeacherListDto, Teacher>()
                .ForMember(dest => dest.TeacherId, opt => opt.Ignore());

            CreateMap<Teacher, TeacherListDto>()
                .ForMember(dest => dest.TeacherId, opt => opt.MapFrom(src => src.TeacherId));

            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.StudentClasses.FirstOrDefault().Class.ClassName))
                .ForMember(dest => dest.Grade, opt => opt.MapFrom(src => src.StudentClasses.FirstOrDefault().Class.GradeLevel))
                .ForMember(dest => dest.Parent, opt => opt.Ignore());

            CreateMap<Parent, ParentDto>()
                .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentId))
                .ForMember(dest => dest.FullNameFather, opt => opt.MapFrom(src => src.FullNameFather))
                .ForMember(dest => dest.YearOfBirthFather, opt => opt.MapFrom(src => src.YearOfBirthFather))
                .ForMember(dest => dest.OccupationFather, opt => opt.MapFrom(src => src.OccupationFather))
                .ForMember(dest => dest.PhoneNumberFather, opt => opt.MapFrom(src => src.PhoneNumberFather))
                .ForMember(dest => dest.EmailFather, opt => opt.MapFrom(src => src.EmailFather))
                .ForMember(dest => dest.IdcardNumberFather, opt => opt.MapFrom(src => src.IdcardNumberFather))
                .ForMember(dest => dest.FullNameMother, opt => opt.MapFrom(src => src.FullNameMother))
                .ForMember(dest => dest.YearOfBirthMother, opt => opt.MapFrom(src => src.YearOfBirthMother))
                .ForMember(dest => dest.OccupationMother, opt => opt.MapFrom(src => src.OccupationMother))
                .ForMember(dest => dest.PhoneNumberMother, opt => opt.MapFrom(src => src.PhoneNumberMother))
                .ForMember(dest => dest.EmailMother, opt => opt.MapFrom(src => src.EmailMother))
                .ForMember(dest => dest.IdcardNumberMother, opt => opt.MapFrom(src => src.IdcardNumberMother))
                .ForMember(dest => dest.FullNameGuardian, opt => opt.MapFrom(src => src.FullNameGuardian))
                .ForMember(dest => dest.YearOfBirthGuardian, opt => opt.MapFrom(src => src.YearOfBirthGuardian))
                .ForMember(dest => dest.OccupationGuardian, opt => opt.MapFrom(src => src.OccupationGuardian))
                .ForMember(dest => dest.PhoneNumberGuardian, opt => opt.MapFrom(src => src.PhoneNumberGuardian))
                .ForMember(dest => dest.EmailGuardian, opt => opt.MapFrom(src => src.EmailGuardian))
                .ForMember(dest => dest.IdcardNumberGuardian, opt => opt.MapFrom(src => src.IdcardNumberGuardian))
                .ReverseMap();

            CreateMap<CreateStudentDto, Student>();

            CreateMap<UpdateStudentDto, Student>()
                .ForMember(dest => dest.StudentId, opt => opt.Ignore());

            CreateMap<Class, ClassDto>().ReverseMap();
            CreateMap<GradeBatch, GradeBatchDto>().ReverseMap();

            CreateMap<Grade, GradeRespondDto>()
                .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentClass.StudentId))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.StudentClass.Student.FullName))
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Assignment.Subject.SubjectName))
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score))
                .ForMember(dest => dest.AssessmentType, opt => opt.MapFrom(src => src.AssessmentsTypeName))
                .ForMember(dest => dest.TeacherComment, opt => opt.MapFrom(src => src.TeacherComment))
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Assignment.Teacher.FullName));

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

            CreateMap<LessonPlan, LessonPlanResponseDto>()
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher != null ? src.Teacher.FullName : "Unknown"))
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject != null ? src.Subject.SubjectName : "Unknown"))
                .ForMember(dest => dest.ReviewerName, opt => opt.MapFrom(src => src.Reviewer != null ? src.Reviewer.FullName : "N/A"));

            // Map Timetable -> TimetableDto và ngược lại
            CreateMap<Timetable, TimetableDto>()
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.TimetableDetails))
                .ReverseMap()
                .ForMember(dest => dest.Semester, opt => opt.Ignore()) // Ignore navigation property
                .ForMember(dest => dest.TimetableDetails, opt => opt.MapFrom(src => src.Details));

            // Map TimetableDetail -> TimetableDetailDto và ngược lại
            CreateMap<TimetableDetail, TimetableDetailDto>()
                .ForMember(dest => dest.Shift, opt => opt.MapFrom(src => (int)src.Shift)) // byte -> int
                .ForMember(dest => dest.Period, opt => opt.MapFrom(src => (int)src.Period)) // byte -> int
                .ReverseMap()
                .ForMember(dest => dest.Shift, opt => opt.MapFrom(src => (byte)src.Shift)) // int -> byte
                .ForMember(dest => dest.Period, opt => opt.MapFrom(src => (byte)src.Period)) // int -> byte
                .ForMember(dest => dest.Class, opt => opt.Ignore())    // Ignore navigation property
                .ForMember(dest => dest.Subject, opt => opt.Ignore())  // Ignore navigation property
                .ForMember(dest => dest.Teacher, opt => opt.Ignore())  // Ignore navigation property
                .ForMember(dest => dest.Timetable, opt => opt.Ignore()); // Ignore navigation property
            // Map từ CreateTimetableDto sang Timetable
            CreateMap<CreateTimetableDto, Timetable>()
                .ForMember(dest => dest.TimetableId, opt => opt.Ignore())
                .ForMember(dest => dest.Semester, opt => opt.Ignore())
                .ForMember(dest => dest.TimetableDetails, opt => opt.Ignore());

            CreateMap<CreateTimetableDetailDto, TimetableDetail>()
                .ForMember(dest => dest.TimetableDetailId, opt => opt.Ignore()) // Ignore vì ID do DB sinh
                .ForMember(dest => dest.TimetableId, opt => opt.Ignore())       // Ignore vì gán thủ công
                .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => src.DayOfWeek)) // Cả hai đều là byte, không cần cast
                .ForMember(dest => dest.Shift, opt => opt.MapFrom(src => (byte)src.Shift))   // Chuyển int sang byte
                .ForMember(dest => dest.Period, opt => opt.MapFrom(src => (byte)src.Period)) // Chuyển int sang byte
                .ForMember(dest => dest.Class, opt => opt.Ignore())    // Ignore navigation property
                .ForMember(dest => dest.Subject, opt => opt.Ignore())  // Ignore navigation property
                .ForMember(dest => dest.Teacher, opt => opt.Ignore())  // Ignore navigation property
                .ForMember(dest => dest.Timetable, opt => opt.Ignore()); // Ignore navigation property
        
        CreateMap<Question, QuestionDto>()
                .ForMember(dest => dest.MathContent, opt => opt.MapFrom(src => src.MathContent));
            CreateMap<QuestionDto, Question>()
                .ForMember(dest => dest.MathContent, opt => opt.MapFrom(src => src.MathContent));
            CreateMap<ExamProposal, ExamProposalDto>();
        }
    }
}