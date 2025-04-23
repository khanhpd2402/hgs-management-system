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
using Application.Features.GradeLevelSubjects.DTOs;
using Application.Features.GradeLevels.DTOs;
using Application.Features.Periods.DTOs;
using Common.Constants;
using Application.Features.SubstituteTeachings.DTOs;
using Application.Features.TeacherSubjects.DTOs;
using Application.Features.Conducts.DTOs;
using Application.Features.Attendances.DTOs;


namespace HGSMAPI.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Attendance, AttendanceDto>().ReverseMap();
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
            CreateMap<TeacherSubject, TeacherSubjectDto>()
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher.FullName))
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.SubjectName));
            CreateMap<CreateTeacherSubjectDto, TeacherSubject>();
            CreateMap<UpdateTeacherSubjectDto, TeacherSubject>();

            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.StudentClasses.FirstOrDefault().Class.ClassName))
                .ForMember(dest => dest.GradeId, opt => opt.MapFrom(src => src.StudentClasses.FirstOrDefault().Class.GradeLevelId))
                .ForMember(dest => dest.GradeName, opt => opt.MapFrom(src => src.StudentClasses.FirstOrDefault().Class.GradeLevel.GradeName))
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

            CreateMap<Grade, GradeRespondDto>()
                .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentClass.StudentId))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.StudentClass.Student.FullName))
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Assignment.Subject.SubjectName))
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Score))
                .ForMember(dest => dest.AssessmentType, opt => opt.MapFrom(src => src.AssessmentsTypeName))
                .ForMember(dest => dest.TeacherComment, opt => opt.MapFrom(src => src.TeacherComment))
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Assignment.Teacher.FullName));

            CreateMap<GradeBatch, UpdateGradeBatchDto>().ReverseMap();
            CreateMap<GradeBatch, GradeBatchDto>();

            CreateMap<Subject, SubjectDto>();
            CreateMap<Subject, SubjectCreateAndUpdateDto>().ReverseMap();

            CreateMap<AcademicYear, AcademicYearDto>().ReverseMap();
            CreateMap<CreateAcademicYearDto, AcademicYear>();

            CreateMap<Semester, SemesterDto>().ReverseMap();
            CreateMap<CreateSemesterDto, Semester>();

            // Map Create DTO to Entity
            CreateMap<CreateLeaveRequestDto, LeaveRequest>()
                .ForMember(dest => dest.RequestDate, opt => opt.MapFrom(_ => DateOnly.FromDateTime(DateTime.Today)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => AppConstants.Status.PENDING))
                .ForMember(dest => dest.Comment, opt => opt.Ignore()) 
                .ForMember(dest => dest.RequestId, opt => opt.Ignore()) 
                .ForMember(dest => dest.Teacher, opt => opt.Ignore()); 

            CreateMap<UpdateLeaveRequest, LeaveRequest>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<LeaveRequest, LeaveRequestListDto>();

            CreateMap<LeaveRequest, LeaveRequestDetailDto>();

            CreateMap<LessonPlan, LessonPlanResponseDto>()
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher != null ? src.Teacher.FullName : "Unknown"))
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject != null ? src.Subject.SubjectName : "Unknown"))
                .ForMember(dest => dest.ReviewerName, opt => opt.MapFrom(src => src.Reviewer != null ? src.Reviewer.FullName : "N/A"));


            CreateMap<Timetable, TimetableDto>()
                        .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.TimetableDetails));
            CreateMap<TimetableDetail, TimetableDetailDto>()
            .ForMember(dest => dest.PeriodName, opt => opt.MapFrom(src => src.Period.PeriodName))
            .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.SubjectName))
            .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher.FullName))
            .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.Class.ClassName));

            CreateMap<CreateTimetableDto, Timetable>();
            CreateMap<TimetableDetailCreateDto, TimetableDetail>();

            CreateMap<UpdateTimetableDetailsDto, Timetable>();
            CreateMap<TimetableDetailUpdateDto, TimetableDetail>();

            CreateMap<UpdateTimetableInfoDto, Timetable>();
            CreateMap<Timetable, TimetableListDto>().ReverseMap();

            
            CreateMap<ExamProposal, ExamProposalDto>();

            CreateMap<GradeLevelSubject, GradeLevelSubjectDto>()
                .ForMember(dest => dest.GradeLevelName,
                    opt => opt.MapFrom(src => src.GradeLevel.GradeName))
                .ForMember(dest => dest.SubjectName,
                    opt => opt.MapFrom(src => src.Subject.SubjectName));

            CreateMap<GradeLevelSubject, GradeLevelSubjectCreateAndUpdateDto>().ReverseMap();

            CreateMap<GradeLevel, GradeLevelDto>();
            CreateMap<GradeLevel, GradeLevelCreateAndUpdateDto>().ReverseMap();

            CreateMap<Period, PeriodDto>();
            CreateMap<Period, PeriodCreateAndUpdateDto>().ReverseMap();

            CreateMap<SubstituteTeachingCreateDto, SubstituteTeaching>();
            CreateMap<SubstituteTeaching, SubstituteTeachingDto>()
                .ForMember(dest => dest.OriginalTeacherName, opt => opt.MapFrom(src => src.OriginalTeacher.FullName))
                .ForMember(dest => dest.SubstituteTeacherName, opt => opt.MapFrom(src => src.SubstituteTeacher.FullName))
                .ForMember(dest => dest.ClassId, opt => opt.MapFrom(src => src.TimetableDetail.ClassId))
                .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.TimetableDetail.Class.ClassName))
                .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.TimetableDetail.SubjectId))
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.TimetableDetail.Subject.SubjectName))
                .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => src.TimetableDetail.DayOfWeek))
                .ForMember(dest => dest.PeriodId, opt => opt.MapFrom(src => src.TimetableDetail.PeriodId))
                .ForMember(dest => dest.PeriodName, opt => opt.MapFrom(src => src.TimetableDetail.Period.PeriodName));

            CreateMap<CreateConductDto, Conduct>();
            CreateMap<UpdateConductDto, Conduct>();
            CreateMap<Conduct, ConductDto>();
        }
    }
}