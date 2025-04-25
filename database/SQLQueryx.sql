USE [master]
GO
/****** Object:  Database [HGSDB]    Script Date: 4/22/2025 2:30:52 AM ******/
CREATE DATABASE [HGSDB]
 CONTAINMENT = NONE
GO
ALTER DATABASE [HGSDB] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [HGSDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [HGSDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [HGSDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [HGSDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [HGSDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [HGSDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [HGSDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [HGSDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [HGSDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [HGSDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [HGSDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [HGSDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [HGSDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [HGSDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [HGSDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [HGSDB] SET  ENABLE_BROKER 
GO
ALTER DATABASE [HGSDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [HGSDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [HGSDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [HGSDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [HGSDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [HGSDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [HGSDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [HGSDB] SET RECOVERY FULL 
GO
ALTER DATABASE [HGSDB] SET  MULTI_USER 
GO
ALTER DATABASE [HGSDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [HGSDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [HGSDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [HGSDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [HGSDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [HGSDB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'HGSDB', N'ON'
GO
ALTER DATABASE [HGSDB] SET QUERY_STORE = ON
GO
ALTER DATABASE [HGSDB] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [HGSDB]
GO
/****** Object:  Table [dbo].[AcademicYears]    Script Date: 4/22/2025 2:30:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AcademicYears](
	[AcademicYearID] [int] IDENTITY(1,1) NOT NULL,
	[YearName] [varchar](9) NOT NULL,
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[AcademicYearID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Attendances]    Script Date: 4/22/2025 2:30:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Attendances](
	[AttendanceID] [int] IDENTITY(1,1) NOT NULL,
	[StudentID] [int] NOT NULL,
	[TimetableDetailId] [int] NOT NULL,
	[Status] [nvarchar](1) NOT NULL,
	[Note] [nvarchar](255) NULL,
	[Date] [date] NOT NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[AttendanceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Classes]    Script Date: 4/22/2025 2:30:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Classes](
	[ClassID] [int] IDENTITY(1,1) NOT NULL,
	[ClassName] [nvarchar](50) NOT NULL,
	[GradeLevelId] [int] NOT NULL,
	[Status] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[ClassID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Conducts]    Script Date: 4/22/2025 2:30:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Conducts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StudentId] [int] NOT NULL,
	[SemesterId] [int] NOT NULL,
	[ConductType] [nvarchar](50) NOT NULL,
	[Note] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ExamProposals]    Script Date: 4/22/2025 2:30:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExamProposals](
	[ProposalID] [int] IDENTITY(1,1) NOT NULL,
	[SubjectID] [int] NOT NULL,
	[Grade] [int] NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[SemesterID] [int] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NULL,
	[Comment] [nvarchar](max) NULL,
	[FileUrl] [nvarchar](500) NULL,
	[Status] [nvarchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ProposalID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GradeBatches]    Script Date: 4/22/2025 2:30:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GradeBatches](
	[BatchID] [int] IDENTITY(1,1) NOT NULL,
	[BatchName] [nvarchar](255) NOT NULL,
	[SemesterID] [int] NOT NULL,
	[StartDate] [date] NULL,
	[EndDate] [date] NULL,
	[Status] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[BatchID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GradeLevels]    Script Date: 4/22/2025 2:30:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GradeLevels](
	[GradeLevelId] [int] IDENTITY(1,1) NOT NULL,
	[GradeName] [nvarchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[GradeLevelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GradeLevelSubjects]    Script Date: 4/22/2025 2:30:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GradeLevelSubjects](
	[GradeLevelSubjectID] [int] IDENTITY(1,1) NOT NULL,
	[GradeLevelID] [int] NOT NULL,
	[SubjectID] [int] NOT NULL,
	[PeriodsPerWeek_HKI] [int] NOT NULL,
	[PeriodsPerWeek_HKII] [int] NOT NULL,
	[ContinuousAssessments_HKI] [int] NOT NULL,
	[ContinuousAssessments_HKII] [int] NOT NULL,
	[MidtermAssessments] [int] NOT NULL,
	[FinalAssessments] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[GradeLevelSubjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Grades]    Script Date: 4/22/2025 2:30:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Grades](
	[GradeID] [int] IDENTITY(1,1) NOT NULL,
	[BatchID] [int] NULL,
	[StudentClassID] [int] NULL,
	[AssignmentID] [int] NULL,
	[Score] [nvarchar](10) NULL,
	[AssessmentsTypeName] [nvarchar](100) NOT NULL,
	[TeacherComment] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[GradeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HomeroomAssignments]    Script Date: 4/22/2025 2:30:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HomeroomAssignments](
	[HomeroomAssignmentID] [int] IDENTITY(1,1) NOT NULL,
	[TeacherID] [int] NOT NULL,
	[ClassID] [int] NOT NULL,
	[SemesterID] [int] NOT NULL,
	[Status] [nvarchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[HomeroomAssignmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LeaveRequests]    Script Date: 4/22/2025 2:30:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LeaveRequests](
	[RequestID] [int] IDENTITY(1,1) NOT NULL,
	[TeacherID] [int] NOT NULL,
	[RequestDate] [date] NOT NULL,
	[LeaveFromDate] [date] NOT NULL,
	[LeaveToDate] [date] NOT NULL,
	[Reason] [nvarchar](max) NULL,
	[Comment] [nvarchar](max) NULL,
	[Status] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[RequestID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LessonPlans]    Script Date: 4/22/2025 2:30:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LessonPlans](
	[PlanID] [int] IDENTITY(1,1) NOT NULL,
	[TeacherID] [int] NOT NULL,
	[SubjectID] [int] NOT NULL,
	[PlanContent] [nvarchar](max) NOT NULL,
	[Status] [nvarchar](20) NULL,
	[SemesterID] [int] NOT NULL,
	[Title] [nvarchar](255) NULL,
	[AttachmentUrl] [nvarchar](500) NULL,
	[Feedback] [nvarchar](max) NULL,
	[SubmittedDate] [datetime] NULL,
	[ReviewedDate] [datetime] NULL,
	[ReviewerId] [int] NULL,
	[EndDate] [datetime] NULL,
	[StartDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[PlanID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Notifications]    Script Date: 4/22/2025 2:30:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notifications](
	[NotificationID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](1000) NOT NULL,
	[Message] [nvarchar](max) NOT NULL,
	[CreateDate] [datetime] NULL,
	[IsActive] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[NotificationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Parents]    Script Date: 4/22/2025 2:30:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Parents](
	[ParentID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[FullNameFather] [nvarchar](100) NULL,
	[YearOfBirthFather] [date] NULL,
	[OccupationFather] [nvarchar](100) NULL,
	[PhoneNumberFather] [nvarchar](15) NULL,
	[EmailFather] [nvarchar](100) NULL,
	[IdcardNumberFather] [nvarchar](50) NULL,
	[FullNameMother] [nvarchar](100) NULL,
	[YearOfBirthMother] [date] NULL,
	[OccupationMother] [nvarchar](100) NULL,
	[PhoneNumberMother] [nvarchar](15) NULL,
	[EmailMother] [nvarchar](100) NULL,
	[IdcardNumberMother] [nvarchar](50) NULL,
	[FullNameGuardian] [nvarchar](100) NULL,
	[YearOfBirthGuardian] [date] NULL,
	[OccupationGuardian] [nvarchar](100) NULL,
	[PhoneNumberGuardian] [nvarchar](15) NULL,
	[EmailGuardian] [nvarchar](100) NULL,
	[IdcardNumberGuardian] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ParentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Periods]    Script Date: 4/22/2025 2:30:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Periods](
	[PeriodId] [int] IDENTITY(1,1) NOT NULL,
	[PeriodName] [nvarchar](50) NOT NULL,
	[StartTime] [time](7) NOT NULL,
	[EndTime] [time](7) NOT NULL,
	[Shift] [tinyint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PeriodId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 4/22/2025 2:30:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[RoleID] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Semesters]    Script Date: 4/22/2025 2:30:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Semesters](
	[SemesterID] [int] IDENTITY(1,1) NOT NULL,
	[AcademicYearID] [int] NOT NULL,
	[SemesterName] [nvarchar](20) NOT NULL,
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SemesterID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StudentClasses]    Script Date: 4/22/2025 2:30:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StudentClasses](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[StudentID] [int] NOT NULL,
	[ClassID] [int] NOT NULL,
	[AcademicYearID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Students]    Script Date: 4/22/2025 2:30:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Students](
	[StudentID] [int] IDENTITY(1,1) NOT NULL,
	[FullName] [nvarchar](100) NOT NULL,
	[DOB] [date] NOT NULL,
	[Gender] [nvarchar](10) NOT NULL,
	[AdmissionDate] [date] NOT NULL,
	[EnrollmentType] [nvarchar](50) NULL,
	[Ethnicity] [nvarchar](50) NULL,
	[PermanentAddress] [nvarchar](255) NULL,
	[BirthPlace] [nvarchar](255) NULL,
	[Religion] [nvarchar](50) NULL,
	[RepeatingYear] [bit] NULL,
	[IDCardNumber] [nvarchar](20) NULL,
	[Status] [nvarchar](50) NULL,
	[ParentID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[StudentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Subjects]    Script Date: 4/22/2025 2:30:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Subjects](
	[SubjectID] [int] IDENTITY(1,1) NOT NULL,
	[SubjectName] [nvarchar](100) NOT NULL,
	[SubjectCategory] [nvarchar](50) NOT NULL,
	[TypeOfGrade] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SubjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SubstituteTeachings]    Script Date: 4/22/2025 2:30:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubstituteTeachings](
	[SubstituteId] [int] IDENTITY(1,1) NOT NULL,
	[TimetableDetailId] [int] NOT NULL,
	[OriginalTeacherId] [int] NOT NULL,
	[SubstituteTeacherId] [int] NOT NULL,
	[Date] [date] NOT NULL,
	[Note] [nvarchar](255) NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[SubstituteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Teachers]    Script Date: 4/22/2025 2:30:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Teachers](
	[TeacherID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NULL,
	[FullName] [nvarchar](100) NOT NULL,
	[DOB] [date] NOT NULL,
	[Gender] [nvarchar](10) NOT NULL,
	[Ethnicity] [nvarchar](50) NULL,
	[Religion] [nvarchar](50) NULL,
	[MaritalStatus] [nvarchar](50) NULL,
	[IDCardNumber] [nvarchar](20) NULL,
	[InsuranceNumber] [nvarchar](20) NULL,
	[EmploymentType] [nvarchar](100) NULL,
	[Position] [nvarchar](100) NULL,
	[Department] [nvarchar](100) NULL,
	[IsHeadOfDepartment] [bit] NULL,
	[EmploymentStatus] [nvarchar](50) NULL,
	[RecruitmentAgency] [nvarchar](255) NULL,
	[HiringDate] [date] NULL,
	[PermanentEmploymentDate] [date] NULL,
	[SchoolJoinDate] [date] NOT NULL,
	[PermanentAddress] [nvarchar](255) NULL,
	[Hometown] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[TeacherID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TeacherSubjects]    Script Date: 4/22/2025 2:30:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TeacherSubjects](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TeacherID] [int] NULL,
	[SubjectID] [int] NULL,
	[IsMainSubject] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TeachingAssignments]    Script Date: 4/22/2025 2:30:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TeachingAssignments](
	[AssignmentID] [int] IDENTITY(1,1) NOT NULL,
	[TeacherID] [int] NOT NULL,
	[SubjectID] [int] NOT NULL,
	[ClassID] [int] NOT NULL,
	[SemesterID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[AssignmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TimetableDetails]    Script Date: 4/22/2025 2:30:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TimetableDetails](
	[TimetableDetailId] [int] IDENTITY(1,1) NOT NULL,
	[TimetableId] [int] NOT NULL,
	[ClassId] [int] NOT NULL,
	[SubjectId] [int] NOT NULL,
	[TeacherId] [int] NOT NULL,
	[DayOfWeek] [nvarchar](20) NOT NULL,
	[PeriodId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TimetableDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Timetables]    Script Date: 4/22/2025 2:30:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Timetables](
	[TimetableId] [int] IDENTITY(1,1) NOT NULL,
	[SemesterId] [int] NOT NULL,
	[EffectiveDate] [date] NOT NULL,
	[EndDate] [date] NULL,
	[Status] [nvarchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TimetableId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 4/22/2025 2:30:52 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[PasswordHash] [nvarchar](255) NOT NULL,
	[Email] [nvarchar](100) NULL,
	[PhoneNumber] [nvarchar](15) NULL,
	[RoleID] [int] NOT NULL,
	[Status] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[AcademicYears] ON 
GO
INSERT [dbo].[AcademicYears] ([AcademicYearID], [YearName], [StartDate], [EndDate]) VALUES (1, N'2026-2027', CAST(N'2026-09-05' AS Date), CAST(N'2027-05-03' AS Date))
GO
INSERT [dbo].[AcademicYears] ([AcademicYearID], [YearName], [StartDate], [EndDate]) VALUES (2, N'2023-2024', CAST(N'2023-08-01' AS Date), CAST(N'2024-07-31' AS Date))
GO
INSERT [dbo].[AcademicYears] ([AcademicYearID], [YearName], [StartDate], [EndDate]) VALUES (3, N'2025-2026', CAST(N'2025-08-01' AS Date), CAST(N'2026-05-25' AS Date))
GO
INSERT [dbo].[AcademicYears] ([AcademicYearID], [YearName], [StartDate], [EndDate]) VALUES (7, N'2024-2025', CAST(N'2024-09-05' AS Date), CAST(N'2025-05-30' AS Date))
GO
SET IDENTITY_INSERT [dbo].[AcademicYears] OFF
GO
SET IDENTITY_INSERT [dbo].[Classes] ON 
GO
INSERT [dbo].[Classes] ([ClassID], [ClassName], [GradeLevelId], [Status]) VALUES (4, N'ádf', 1, N'Hoạt động')
GO
INSERT [dbo].[Classes] ([ClassID], [ClassName], [GradeLevelId], [Status]) VALUES (13, N'6A', 1, N'Hoạt động')
GO
INSERT [dbo].[Classes] ([ClassID], [ClassName], [GradeLevelId], [Status]) VALUES (14, N'6B', 1, N'Hoạt động')
GO
INSERT [dbo].[Classes] ([ClassID], [ClassName], [GradeLevelId], [Status]) VALUES (15, N'7A', 2, N'Hoạt động')
GO
INSERT [dbo].[Classes] ([ClassID], [ClassName], [GradeLevelId], [Status]) VALUES (16, N'7B', 2, N'Hoạt động')
GO
INSERT [dbo].[Classes] ([ClassID], [ClassName], [GradeLevelId], [Status]) VALUES (17, N'7C', 2, N'Hoạt động')
GO
INSERT [dbo].[Classes] ([ClassID], [ClassName], [GradeLevelId], [Status]) VALUES (18, N'8A', 3, N'Hoạt động')
GO
INSERT [dbo].[Classes] ([ClassID], [ClassName], [GradeLevelId], [Status]) VALUES (19, N'8B', 3, N'Hoạt động')
GO
INSERT [dbo].[Classes] ([ClassID], [ClassName], [GradeLevelId], [Status]) VALUES (20, N'9A', 4, N'Hoạt động')
GO
INSERT [dbo].[Classes] ([ClassID], [ClassName], [GradeLevelId], [Status]) VALUES (21, N'9B', 4, N'Hoạt động')
GO
INSERT [dbo].[Classes] ([ClassID], [ClassName], [GradeLevelId], [Status]) VALUES (36, N'6avb', 1, N'Hoạt động')
GO
SET IDENTITY_INSERT [dbo].[Classes] OFF
GO
SET IDENTITY_INSERT [dbo].[ExamProposals] ON 
GO
INSERT [dbo].[ExamProposals] ([ProposalID], [SubjectID], [Grade], [Title], [SemesterID], [CreatedBy], [CreatedDate], [Comment], [FileUrl], [Status]) VALUES (1, 89, 8, N'hehehe', 7, 2, CAST(N'2025-04-22T01:33:39.923' AS DateTime), N'như lồn', NULL, N'Từ chối')
GO
SET IDENTITY_INSERT [dbo].[ExamProposals] OFF
GO
SET IDENTITY_INSERT [dbo].[GradeLevels] ON 
GO
INSERT [dbo].[GradeLevels] ([GradeLevelId], [GradeName]) VALUES (1, N'string')
GO
INSERT [dbo].[GradeLevels] ([GradeLevelId], [GradeName]) VALUES (2, N'Khối 7')
GO
INSERT [dbo].[GradeLevels] ([GradeLevelId], [GradeName]) VALUES (3, N'Khối 8')
GO
INSERT [dbo].[GradeLevels] ([GradeLevelId], [GradeName]) VALUES (4, N'Khối 9')
GO
SET IDENTITY_INSERT [dbo].[GradeLevels] OFF
GO
SET IDENTITY_INSERT [dbo].[GradeLevelSubjects] ON 
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (1, 1, 78, 4, 5, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (2, 2, 78, 4, 5, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (3, 3, 78, 4, 5, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (4, 4, 78, 4, 5, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (5, 1, 79, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (6, 2, 79, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (9, 1, 80, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (10, 2, 80, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (11, 3, 80, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (12, 4, 80, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (13, 1, 81, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (14, 2, 81, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (15, 3, 81, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (16, 4, 81, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (17, 1, 82, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (18, 2, 82, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (19, 3, 82, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (20, 4, 82, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (21, 1, 84, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (22, 2, 84, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (23, 3, 84, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (24, 4, 84, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (27, 3, 85, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (28, 4, 85, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (29, 1, 95, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (30, 2, 95, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (31, 3, 95, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (32, 4, 95, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (33, 1, 93, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (34, 2, 93, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (35, 3, 93, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (36, 4, 93, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (37, 1, 92, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (38, 2, 92, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (39, 3, 92, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (40, 4, 92, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (41, 1, 91, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (42, 2, 91, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (43, 3, 91, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (44, 4, 91, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (47, 3, 89, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (48, 4, 89, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (49, 1, 88, 4, 5, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (50, 2, 88, 4, 6, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (51, 3, 88, 4, 7, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (52, 1, 90, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (53, 2, 90, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (54, 3, 90, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (55, 4, 90, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (56, 4, 88, 4, 8, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (61, 1, 98, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (62, 2, 98, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (63, 3, 98, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (64, 4, 98, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (69, 1, 100, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (70, 2, 100, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (71, 3, 100, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (72, 4, 100, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (73, 1, 101, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (74, 2, 101, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (75, 3, 101, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (76, 4, 101, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (77, 1, 102, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (78, 2, 102, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (79, 3, 102, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (80, 4, 102, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (81, 1, 103, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (82, 2, 103, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (83, 3, 103, 4, 4, 4, 4, 1, 1)
GO
INSERT [dbo].[GradeLevelSubjects] ([GradeLevelSubjectID], [GradeLevelID], [SubjectID], [PeriodsPerWeek_HKI], [PeriodsPerWeek_HKII], [ContinuousAssessments_HKI], [ContinuousAssessments_HKII], [MidtermAssessments], [FinalAssessments]) VALUES (84, 4, 103, 4, 4, 4, 4, 1, 1)
GO
SET IDENTITY_INSERT [dbo].[GradeLevelSubjects] OFF
GO
SET IDENTITY_INSERT [dbo].[HomeroomAssignments] ON 
GO
INSERT [dbo].[HomeroomAssignments] ([HomeroomAssignmentID], [TeacherID], [ClassID], [SemesterID], [Status]) VALUES (13, 3, 36, 1, N'Không Hoạt động')
GO
INSERT [dbo].[HomeroomAssignments] ([HomeroomAssignmentID], [TeacherID], [ClassID], [SemesterID], [Status]) VALUES (14, 6, 36, 8, N'Hoạt động')
GO
INSERT [dbo].[HomeroomAssignments] ([HomeroomAssignmentID], [TeacherID], [ClassID], [SemesterID], [Status]) VALUES (16, 4, 21, 8, N'Hoạt động')
GO
INSERT [dbo].[HomeroomAssignments] ([HomeroomAssignmentID], [TeacherID], [ClassID], [SemesterID], [Status]) VALUES (17, 7, 20, 4, N'Hoạt động')
GO
INSERT [dbo].[HomeroomAssignments] ([HomeroomAssignmentID], [TeacherID], [ClassID], [SemesterID], [Status]) VALUES (18, 4, 21, 7, N'Hoạt động')
GO
INSERT [dbo].[HomeroomAssignments] ([HomeroomAssignmentID], [TeacherID], [ClassID], [SemesterID], [Status]) VALUES (19, 6, 36, 7, N'Hoạt động')
GO
SET IDENTITY_INSERT [dbo].[HomeroomAssignments] OFF
GO
SET IDENTITY_INSERT [dbo].[LessonPlans] ON 
GO
INSERT [dbo].[LessonPlans] ([PlanID], [TeacherID], [SubjectID], [PlanContent], [Status], [SemesterID], [Title], [AttachmentUrl], [Feedback], [SubmittedDate], [ReviewedDate], [ReviewerId], [EndDate], [StartDate]) VALUES (1, 1, 1, N'...', N'Chờ duyệt', 7, N'Đề cương cuối kì', NULL, NULL, CAST(N'2025-04-19T23:59:17.017' AS DateTime), NULL, NULL, CAST(N'2025-05-19T00:00:00.000' AS DateTime), CAST(N'2025-04-19T00:00:00.000' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[LessonPlans] OFF
GO
SET IDENTITY_INSERT [dbo].[Parents] ON 
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (1, 23, N'Vũ Văn Duy', CAST(N'1977-09-10' AS Date), N'Làm ruộng', N'0463733021', N'', N'734992760411', N'Bùi Thị Hà', CAST(N'1978-10-06' AS Date), N'Làm ruộng', N'0667330791', N'', N'405437678091', N'', NULL, N'', N'', N'', NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (2, 24, N'Nguyễn Văn Thao', CAST(N'1986-02-17' AS Date), N'Làm ruộng', N'0338733875', N'', N'580081617832', N'Phạm Thị Hương', CAST(N'1987-05-03' AS Date), N'Làm ruộng', N'0122130262', N'', N'739498817920', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (3, 25, N'Trần Văn Tuyến', CAST(N'1990-03-30' AS Date), N'Làm ruộng', N'0460990840', N'', N'621566754579', N'Tạ Thị Mai', CAST(N'1990-10-18' AS Date), N'Làm ruộng', N'0378233319', N'', N'381059294939', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (4, 26, N'Phạm Văn Dương', CAST(N'1972-12-20' AS Date), N'Làm ruộng', N'0346191382', N'', N'360606217384', N'Trần Thị Oanh', CAST(N'1981-08-14' AS Date), N'Làm ruộng', N'0542169022', N'', N'818896174430', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (5, 27, N'Nguyễn Văn Việt', CAST(N'1982-11-18' AS Date), N'Làm ruộng', N'0553425449', N'', N'371753209829', N'Đỗ Thị Minh Thư', CAST(N'1982-08-07' AS Date), N'Làm ruộng', N'0572377294', N'', N'236580604314', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (6, 28, N'Nguyễn Văn Bắc', CAST(N'1975-11-03' AS Date), N'Làm ruộng', N'0286966168', N'', N'797266089916', N'Trần Thị Nhiên', CAST(N'1985-04-08' AS Date), N'Công nhân', N'0659456336', N'', N'633692109584', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (7, 29, N'Nguyễn Văn Duy', CAST(N'1979-10-08' AS Date), N'Làm ruộng', N'0189365249', N'', N'112615877389', N'Phạm Thị Bích', CAST(N'1988-07-15' AS Date), N'Làm ruộng', N'0933857637', N'', N'960613209009', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (8, 30, N'Nguyễn Văn Hoàng', CAST(N'1977-05-17' AS Date), N'Làm ruộng', N'0866854286', N'', N'784651231765', N'Phạm Thị Thoa', CAST(N'1989-12-31' AS Date), N'Làm ruộng', N'0697488451', N'', N'318833065032', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (9, 31, N'Vũ Văn Hạnh', CAST(N'1975-07-07' AS Date), N'Công nhân', N'0999078339', N'', N'833041018247', N'Nguyễn Thị Hồng', CAST(N'1979-06-13' AS Date), N'Công nhân', N'0264841860', N'', N'945974034070', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (10, 32, N'Lê Văn Đông', CAST(N'1971-11-04' AS Date), N'Kĩ sư', N'0352082693', N'', N'738134109973', N'Nguyễn Thị Lan', CAST(N'1989-02-07' AS Date), N'Kinh doanh', N'0431794083', N'', N'202899515628', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (11, 33, N'Phạm Văn Thuyên', CAST(N'1983-11-07' AS Date), N'Làm ruộng', N'0512790089', N'', N'140817481279', N'Nguyễn Thị Nhung', CAST(N'1970-09-29' AS Date), N'Làm ruộng', N'0792650061', N'', N'985827904939', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (12, 34, N'Nguyễn Văn Hưng', CAST(N'1989-11-14' AS Date), N'Làm ruộng', N'0365256524', N'', N'472629427909', N'Phạm Thị Dung', CAST(N'1974-10-10' AS Date), N'Làm ruộng', N'0218694186', N'', N'669678235054', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (13, 35, N'Nguyễn Văn Giang', CAST(N'1980-10-26' AS Date), N'Làm ruộng', N'0422841352', N'', N'876357406377', N'Nguyễn Thị Dịu', CAST(N'1985-04-13' AS Date), N'Làm ruộng', N'0133475631', N'', N'638970750570', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (14, 36, N'Nguyễn Văn Điệp', CAST(N'1978-03-11' AS Date), N'Làm ruộng', N'0448884475', N'', N'811432015895', N'Phạm Thị Mai', CAST(N'1978-01-10' AS Date), N'Làm ruộng', N'0354139840', N'', N'912312686443', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (15, 37, N'Vương Quốc Triều', CAST(N'1978-02-18' AS Date), N'Làm ruộng', N'0843112283', N'', N'436182552576', N'Trần Thị Lan', CAST(N'1972-12-08' AS Date), N'Làm ruộng', N'0865871578', N'', N'617378431558', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (16, 38, N'Nguyễn Văn Luyến', CAST(N'1972-03-22' AS Date), N'Làm ruộng', N'0437107324', N'', N'965757846832', N'Hoàng Thị Yến', CAST(N'1970-11-04' AS Date), N'Làm ruộng', N'0703253269', N'', N'320656538009', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (17, 39, N'Nguyễn Văn Thanh', CAST(N'1980-03-31' AS Date), N'Làm ruộng', N'0843836802', N'', N'884301251173', N'Phạm Thị Thoa', CAST(N'1986-11-18' AS Date), N'Làm ruộng', N'0392127484', N'', N'874146193265', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (18, 40, N'Nguyễn Văn Thanh', CAST(N'1979-01-14' AS Date), N'Công nhân', N'0201254570', N'', N'150613176822', N'Lê Thị Tám', CAST(N'1970-08-31' AS Date), N'Công nhân', N'0966196978', N'', N'167455303668', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (19, 41, N'Nguyễn Văn Dũng', CAST(N'1976-10-28' AS Date), N'Làm ruộng', N'0564932662', N'', N'954600983858', N'Trần Thị Thu', CAST(N'1981-03-23' AS Date), N'Làm ruộng', N'0490662080', N'', N'494943743944', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (20, 42, N'Lê Xuân Duy', CAST(N'1987-03-04' AS Date), N'Làm ruộng', N'0959854197', N'', N'427616810798', N'Trần Thị Nhạn', CAST(N'1986-09-03' AS Date), N'Làm ruộng', N'0370996022', N'', N'783981108665', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (21, 43, N'Vũ Văn Quang', CAST(N'1970-02-11' AS Date), N'Làm ruộng', N'0435972160', N'', N'572381585836', N'Vũ Thị Lan', CAST(N'1980-08-08' AS Date), N'Làm ruộng', N'0587556451', N'', N'321110957860', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (22, 44, N'Trần Minh Tá', CAST(N'1973-12-28' AS Date), N'Làm ruộng', N'0868720567', N'', N'790400493144', N'Nguyễn Thị Oanh', CAST(N'1984-04-15' AS Date), N'Làm ruộng', N'0308938395', N'', N'440745580196', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (23, 45, N'Nguyễn Văn Hiến', CAST(N'1982-09-05' AS Date), N'Làm ruộng', N'0455918461', N'', N'148154073953', N'Nguyễn Thị Hoa', CAST(N'1973-09-20' AS Date), N'Làm ruộng', N'0200352865', N'', N'457519203424', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (24, 46, N'Bùi Văn Tới', CAST(N'1975-01-26' AS Date), N'Công nhân', N'0331745433', N'', N'633212423324', N'Phạm Thị Thủy Đông', CAST(N'1973-12-26' AS Date), N'Làm ruộng', N'0153178548', N'', N'573915863037', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (25, 47, N'Nguyễn Văn Giới', CAST(N'1986-10-11' AS Date), N'Làm ruộng', N'0910752552', N'', N'521830099821', N'Hoàng Thị Ánh Ngọc', CAST(N'1975-12-06' AS Date), N'Làm ruộng', N'0232244223', N'', N'343323296308', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (26, 48, N'Nguyễn Hữu Luận', CAST(N'1979-07-25' AS Date), N'Giáo viên', N'0298017084', N'', N'368348896503', N'Bùi Thị Nhung', CAST(N'1970-07-26' AS Date), N'Làm ruộng', N'0126169025', N'', N'623058760166', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (27, 49, N'Nguyễn Văn Thùy', CAST(N'1981-03-07' AS Date), N'Làm ruộng', N'0732115823', N'', N'660427027940', N'Nguyễn Ngọc Lan', CAST(N'1983-07-10' AS Date), N'Làm ruộng', N'0219779139', N'', N'288729077577', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (28, 50, N'Nguyễn Thành Chung', CAST(N'1977-07-11' AS Date), N'Làm ruộng', N'0435892605', N'', N'683039069175', N'Phạm Thị Thúy', CAST(N'1984-11-27' AS Date), N'Làm ruộng', N'0106545233', N'', N'170837998390', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (29, 51, N'Trần Hữu Phước', CAST(N'1981-06-18' AS Date), N'Làm ruộng', N'0725093263', N'', N'337413638830', N'Tạ Thị Tuyết', CAST(N'1985-04-12' AS Date), N'Làm ruộng', N'0562207645', N'', N'905901616811', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (30, 52, N'Phạm Văn Triều', CAST(N'1971-07-07' AS Date), N'Làm ruộng', N'0699648272', N'', N'351407849788', N'Nguyễn Thị Hằng', CAST(N'1990-07-29' AS Date), N'Làm ruộng', N'0591088330', N'', N'200289618968', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (31, 53, N'Đới Văn Thạnh', CAST(N'1990-12-31' AS Date), N'Làm ruộng', N'0218262296', N'', N'846821457147', N'Nguyễn Thị Thương', CAST(N'1988-10-13' AS Date), N'Làm ruộng', N'0985592514', N'', N'687139040231', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (32, 54, N'Bùi Văn Giang', CAST(N'1987-05-25' AS Date), N'Làm ruộng', N'0129333066', N'', N'842141914367', N'Mai Thị Lệ', CAST(N'1982-01-31' AS Date), N'Làm ruộng', N'0552391242', N'', N'910300493240', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (33, 55, N'Nguyễn Thành An', CAST(N'1990-04-06' AS Date), N'Làm ruộng', N'0214132231', N'', N'630246704816', N'Trần Thị Khuê', CAST(N'1988-04-01' AS Date), N'Làm ruộng', N'0219811218', N'', N'308436983823', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (34, 56, N'Phạm Văn Viện', CAST(N'1984-01-11' AS Date), N'Làm ruộng', N'0449450314', N'', N'987483847141', N'Nguyễn Thị Huyền', CAST(N'1972-12-03' AS Date), N'Làm ruộng', N'0657852804', N'', N'954914963245', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (35, 57, N'Trần Văn Thái', CAST(N'1984-02-04' AS Date), N'Làm ruộng', N'0243099337', N'', N'919867879152', N'Nguyễn Thị Thêu', CAST(N'1987-08-22' AS Date), N'Làm ruộng', N'0291573506', N'', N'861568671464', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (36, 58, N'Vũ Văn Phúc', CAST(N'1976-05-18' AS Date), N'Công nhân', N'0657092452', N'', N'304179406166', N'Mai Thị Thủy', CAST(N'1975-06-20' AS Date), N'Công nhân', N'0740625023', N'', N'497094607353', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (37, 59, N'Phạm Văn Tới', CAST(N'1976-04-25' AS Date), N'Làm ruộng', N'0649042552', N'', N'725603955984', N'Hà Thị Hiền', CAST(N'1977-04-19' AS Date), N'Làm ruộng', N'0161579602', N'', N'542805618047', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (38, 60, N'Bùi Văn Thiên', CAST(N'1976-05-13' AS Date), N'Làm ruộng', N'0515153920', N'', N'982002913951', N'Nguyễn Thị Tâm', CAST(N'1982-04-15' AS Date), N'Làm ruộng', N'0438138473', N'', N'792556130886', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (39, 61, N'Phạm Văn Quận', CAST(N'1987-05-23' AS Date), N'Làm ruộng', N'0659684807', N'', N'319538217782', N'Đoàn Thị Hà', CAST(N'1978-03-23' AS Date), N'Làm ruộng', N'0558944040', N'', N'851403957605', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (40, 62, N'Nguyễn Văn Thanh', CAST(N'1983-05-30' AS Date), N'Làm ruộng', N'0491247177', N'', N'580485773086', N'Phạm Thị Hiền', CAST(N'1976-08-29' AS Date), N'Làm ruộng', N'0438830804', N'', N'444312381744', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (41, 63, N'Vũ Văn Trưởng', CAST(N'1983-07-07' AS Date), N'Công nhân', N'0819844979', N'', N'195732706785', N'Nguyễn Thị Nhường', CAST(N'1974-08-21' AS Date), N'Công nhân', N'0691010111', N'', N'275981396436', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (42, 64, N'Trần Văn Điệp', CAST(N'1982-09-14' AS Date), N'Công nhân', N'0307946836', N'', N'999473106861', N'Nguyễn Thị Xuân', CAST(N'1972-10-12' AS Date), N'Công nhân', N'0202703392', N'', N'393541491031', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (43, 65, N'Phan Văn Sinh', CAST(N'1988-04-21' AS Date), N'Công nhân', N'0590705472', N'', N'708558303117', N'Trần Thị Hồng', CAST(N'1984-03-02' AS Date), N'Làm ruộng', N'0131679946', N'', N'471909791231', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (44, 66, N'Vũ Đình Việt', CAST(N'1988-04-12' AS Date), N'Làm ruộng', N'0294986939', N'', N'114133524894', N'Trần Thị Vòng', CAST(N'1981-12-14' AS Date), N'Làm ruộng', N'0846331095', N'', N'237409138679', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (45, 67, N'Vũ Khắc Sinh', CAST(N'1970-04-30' AS Date), N'Làm ruộng', N'0143220418', N'', N'617665427923', N'Vũ Thị Mai', CAST(N'1982-03-06' AS Date), N'Làm ruộng', N'0516385215', N'', N'657848352193', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (46, 68, N'Nguyễn Văn Tùng', CAST(N'1985-07-19' AS Date), N'Làm ruộng', N'0959036338', N'', N'190047013759', N'Trần Thị Thanh Hương', CAST(N'1976-09-01' AS Date), N'Công nhân', N'0507950150', N'', N'189468944072', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (47, 69, N'Vũ Đình Hạnh', CAST(N'1975-07-25' AS Date), N'Làm ruộng', N'0298789507', N'', N'192720371484', N'Trần Thị Hạnh', CAST(N'1982-11-18' AS Date), N'Làm Ruộng', N'0996823459', N'', N'284542471170', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (48, 70, N'Lưu Văn Tuệ', CAST(N'1988-02-16' AS Date), N'Làm ruộng', N'0911677217', N'', N'818995952606', N'Phan Thị Trà', CAST(N'1984-12-25' AS Date), N'Làm Ruộng', N'0333249187', N'', N'723265981674', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (49, 71, N'Trịnh Văn Chính', CAST(N'1978-01-14' AS Date), N'Làm ruộng', N'0195322436', N'', N'356032246351', N'Nguyễn Thị Hiền', CAST(N'1970-02-25' AS Date), N'Làm Ruộng', N'0533585041', N'', N'554013174772', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (50, 72, N'Đào Văn Thái', CAST(N'1984-06-13' AS Date), N'Làm ruộng', N'0721211731', N'', N'141084253787', N'Phạm Thị Xuân', CAST(N'1971-06-07' AS Date), N'Làm Ruộng', N'0747488367', N'', N'264768421649', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (51, 73, N'Nguyễn Hoàng Đan', CAST(N'1989-12-19' AS Date), N'Làm ruộng', N'0581818991', N'', N'366195565462', N'Phạm Thị Thu Thúy', CAST(N'1977-06-06' AS Date), N'Làm Ruộng', N'0529795724', N'', N'992262202501', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (52, 74, N'Nguyễn Quang Trình', CAST(N'1981-08-03' AS Date), N'Làm ruộng', N'0678921771', N'', N'443809628486', N'Vũ Thị Lương', CAST(N'1983-08-01' AS Date), N'Làm Ruộng', N'0304571652', N'', N'524252295494', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (53, 75, N'Phạm Văn Hợp', CAST(N'1973-04-01' AS Date), N'Làm ruộng', N'0169589751', N'', N'370873445272', N'Trần Thị Trâm', CAST(N'1982-04-09' AS Date), N'Làm Ruộng', N'0633899873', N'', N'103617709875', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (54, 76, N'Trần Văn Bắc', CAST(N'1973-06-17' AS Date), N'Công nhân', N'0495406854', N'', N'953713977336', N'Vũ Thị Lý', CAST(N'1986-04-29' AS Date), N'Công nhân', N'0628697192', N'', N'483264386653', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (55, 77, N'Nguyễn Văn Du', CAST(N'1981-10-02' AS Date), N'Làm ruộng', N'0593242412', N'', N'981846433877', N'Nguyễn Thị Phúc', CAST(N'1983-10-15' AS Date), N'Làm Ruộng', N'0564709287', N'', N'359845596551', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (56, 78, N'Phan Văn Luyến', CAST(N'1978-01-10' AS Date), N'Làm ruộng', N'0167625141', N'', N'461236906051', N'Nguyễn Thị Hải Yến', CAST(N'1989-02-11' AS Date), N'Làm Ruộng', N'0489210844', N'', N'777288246154', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (57, 79, N'Nguyễn Văn Đồng', CAST(N'1982-07-19' AS Date), N'Làm ruộng', N'0659558743', N'', N'350451964139', N'Nguyễn Thị Thu Mịn', CAST(N'1985-01-09' AS Date), N'Làm Ruộng', N'0336842650', N'', N'902151459455', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (58, 80, N'Nguyễn Văn Tiền', CAST(N'1976-11-24' AS Date), N'Làm ruộng', N'0582262575', N'', N'244397366046', N'Nguyễn Thị Cúc', CAST(N'1986-02-10' AS Date), N'Làm Ruộng', N'0974287807', N'', N'839738333225', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (59, 81, N'Mai Văn Trường', CAST(N'1982-06-22' AS Date), N'Làm ruộng', N'0387387162', N'', N'246539431810', N'Tống Thị Thảo', CAST(N'1987-09-01' AS Date), N'Làm Ruộng', N'0489680606', N'', N'259198170900', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (60, 82, N'Nguyễn Văn Bình', CAST(N'1973-01-08' AS Date), N'Làm ruộng', N'0846055150', N'', N'681928420066', N'Lê Thị Bến', CAST(N'1974-12-14' AS Date), N'Làm Ruộng', N'0754067397', N'', N'197907280921', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (61, 83, N'Vũ Đình Thoán', CAST(N'1985-12-11' AS Date), N'Làm ruộng', N'0466675943', N'', N'469065898656', N'Trần Thị Huyền', CAST(N'1989-04-23' AS Date), N'Làm Ruộng', N'0547961467', N'', N'109264081716', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (62, 84, N'Nguyễn Văn Sang', CAST(N'1984-10-02' AS Date), N'Làm ruộng', N'0358689296', N'', N'471490132808', N'Tạ Văn Hoa', CAST(N'1974-11-25' AS Date), N'Làm Ruộng', N'0248865926', N'', N'236491286754', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (63, 85, N'Nguyễn Văn Hệ', CAST(N'1986-06-07' AS Date), N'Điện lực', N'0989772039', N'', N'741457420587', N'Đỗ Thị Nhẫn', CAST(N'1979-08-22' AS Date), N'Giáo viên', N'0467142540', N'', N'439666849374', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (64, 86, N'Bùi Ngọc Mạnh', CAST(N'1982-11-29' AS Date), N'Làm ruộng', N'0368905401', N'', N'393585586547', N'Nguyễn Thị Ngát', CAST(N'1980-12-10' AS Date), N'Làm Ruộng', N'0443092727', N'', N'269318628311', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (65, 87, N'Phạm Văn Quỳnh', CAST(N'1983-01-21' AS Date), N'Làm ruộng', N'0833110541', N'', N'669861000776', N'Nguyễn Thị Thanh Dệt', CAST(N'1989-04-22' AS Date), N'Làm Ruộng', N'0141652077', N'', N'981577891111', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (66, 88, N'Lưu Văn Minh', CAST(N'1990-01-30' AS Date), N'Làm ruộng', N'0879429447', N'', N'286805021762', N'Nguyễn Thị Hồng', CAST(N'1973-06-14' AS Date), N'Làm Ruộng', N'0310494291', N'', N'884906303882', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (67, 89, N'Nguyễn Văn Thực', CAST(N'1971-04-18' AS Date), N'Làm ruộng', N'0192419749', N'', N'267412167787', N'Đinh Thị Khen', CAST(N'1987-05-07' AS Date), N'Làm Ruộng', N'0379156857', N'', N'623352462053', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (68, 90, N'Trần Văn Chung', CAST(N'1986-01-02' AS Date), N'Làm ruộng', N'0691357779', N'', N'625023102760', N'Trần Thị Sen', CAST(N'1970-02-09' AS Date), N'Làm Ruộng', N'0641351532', N'', N'756469798088', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (69, 91, N'Nguyễn Văn Thịnh', CAST(N'1981-04-13' AS Date), N'Làm ruộng', N'0899566882', N'', N'172643178701', N'Vũ Thị Nhung', CAST(N'1990-07-12' AS Date), N'Làm Ruộng', N'0868970388', N'', N'230500358343', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (70, 92, N'Lâm Văn Đam', CAST(N'1980-09-14' AS Date), N'Làm ruộng', N'0503619992', N'', N'512174308300', N'Nguyễn Thị Huế', CAST(N'1985-09-22' AS Date), N'Làm Ruộng', N'0687255179', N'', N'332010972499', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (71, 93, N'Đỗ Văn Tiến', CAST(N'1976-01-13' AS Date), N'Làm ruộng', N'0274169403', N'', N'915156847238', N'Nguyễn Thị Lý', CAST(N'1974-04-30' AS Date), N'Làm Ruộng', N'0797726732', N'', N'125422245264', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (72, 94, N'Nguyễn Văn Thuận', CAST(N'1982-10-18' AS Date), N'Làm ruộng', N'0208144950', N'', N'335231447219', N'Bùi Thị Hậu', CAST(N'1987-01-21' AS Date), N'Làm Ruộng', N'0364084291', N'', N'845109081268', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (73, 95, N'Vũ Văn Thuận', CAST(N'1983-10-13' AS Date), N'Công nhân', N'0588096970', N'', N'806690996885', N'Vũ Thị Nụ', CAST(N'1984-11-24' AS Date), N'Công nhân', N'0790444964', N'', N'805036610364', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (74, 96, N'Nguyễn Văn Viện', CAST(N'1986-09-21' AS Date), N'Làm ruộng', N'0646354925', N'', N'441012299060', N'Vũ Thị Trang', CAST(N'1988-12-14' AS Date), N'Làm Ruộng', N'0278812897', N'', N'812039911746', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (75, 97, N'Vũ Văn Duy', CAST(N'1985-03-24' AS Date), N'Làm ruộng', N'0645989018', N'', N'360698539018', N'Lô Thị Điệp', CAST(N'1989-04-12' AS Date), N'Công nhân', N'0580109244', N'', N'399422341585', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (76, 98, N'Vũ Văn Thiệu', CAST(N'1971-03-30' AS Date), N'Làm ruộng', N'0517612862', N'', N'927439379692', N'Trần Thị Tâm', CAST(N'1977-11-18' AS Date), N'Làm Ruộng', N'0170769762', N'', N'505848050117', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (77, 99, N'Đặng Văn Định', CAST(N'1987-03-06' AS Date), N'Làm ruộng', N'0109912961', N'', N'668568176031', N'Trần Thị Ngoan', CAST(N'1981-01-14' AS Date), N'Làm Ruộng', N'0533889526', N'', N'599601930379', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (78, 100, N'Hoàng Văn An', CAST(N'1974-12-08' AS Date), N'Giáo viên', N'0955247771', N'', N'664877831935', N'Phạm Thị Thảo', CAST(N'1989-06-26' AS Date), N'Dược sỹ', N'0182976281', N'', N'822997272014', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (79, 101, N'Đồng Kính Tôn', CAST(N'1975-09-20' AS Date), N'Công nhân', N'0621288019', N'', N'485610729455', N'Mai Thị Hà', CAST(N'1988-07-30' AS Date), N'Công nhân', N'0989127880', N'', N'607590299844', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (80, 102, N'Phạm Văn Thịnh', CAST(N'1986-10-10' AS Date), N'Làm ruộng', N'0810783243', N'', N'188176441192', N'Trần Thị Hằng', CAST(N'1980-10-18' AS Date), N'Làm Ruộng', N'0604187488', N'', N'283224058151', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (81, 103, N'Nguyễn Văn Cương', CAST(N'1987-09-27' AS Date), N'Làm ruộng', N'0598199671', N'', N'604936093091', N'Lâm Thị Thim', CAST(N'1977-05-12' AS Date), N'Làm Ruộng', N'0777215451', N'', N'286834257841', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (82, 104, N'Nguyễn Văn Bình', CAST(N'1974-05-18' AS Date), N'Công nhân', N'0111994087', N'', N'725036776065', N'Trần Thị Mừng', CAST(N'1987-07-04' AS Date), N'Công nhân', N'0447261202', N'', N'428219234943', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (83, 105, N'Nguyễn Văn Hạnh', CAST(N'1977-03-18' AS Date), N'Làm ruộng', N'0611229735', N'', N'922345811128', N'Nguyễn Thị Hạnh', CAST(N'1976-05-31' AS Date), N'Làm Ruộng', N'0138485032', N'', N'616167575120', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (84, 106, N'Hoàng Văn Hòa', CAST(N'1983-02-02' AS Date), N'Làm ruộng', N'0585416102', N'', N'851335453987', N'Vũ Thị Nhường', CAST(N'1983-04-26' AS Date), N'Làm Ruộng', N'0854480290', N'', N'409762740135', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (85, 107, N'Lâm Văn Tứ', CAST(N'1983-11-20' AS Date), N'Làm ruộng', N'0465927070', N'', N'120366281270', N'Mai Thị Tuyết', CAST(N'1977-04-09' AS Date), N'Làm Ruộng', N'0856244271', N'', N'575406688451', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (86, 108, N'Nguyễn Quang Sáng', CAST(N'1978-03-16' AS Date), N'Công nhân', N'0458933961', N'', N'589024531841', N'Phạm Thị Mai', CAST(N'1976-02-28' AS Date), N'Công nhân', N'0645453059', N'', N'482936513423', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (87, 109, N'Bùi Văn Hai', CAST(N'1990-10-12' AS Date), N'Làm ruộng', N'0845043903', N'', N'924547582864', N'Nguyễn Thị Hoa', CAST(N'1984-02-17' AS Date), N'Làm Ruộng', N'0263074499', N'', N'615712028741', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (88, 110, N'Nguyễn Văn Lộc', CAST(N'1980-07-21' AS Date), N'Làm ruộng', N'0785072231', N'', N'487235021591', N'Vũ Thị Tám', CAST(N'1972-02-14' AS Date), N'Làm Ruộng', N'0348216772', N'', N'145040512084', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (89, 111, N'Phạm Văn Tiên', CAST(N'1970-08-28' AS Date), N'Làm ruộng', N'0988662785', N'', N'710152930021', N'Đỗ Thị Ngát', CAST(N'1989-04-04' AS Date), N'Làm Ruộng', N'0297520178', N'', N'592839974164', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (90, 112, N'Nguyễn Văn Minh', CAST(N'1977-02-12' AS Date), N'Làm ruộng', N'0250614511', N'', N'552208530902', N'Nguyễn Thị Thêm', CAST(N'1981-12-19' AS Date), N'Làm Ruộng', N'0679169285', N'', N'285836851596', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (91, 113, N'Nguyễn Văn Tuấn', CAST(N'1986-05-13' AS Date), N'Làm ruộng', N'0315339165', N'', N'562363749742', N'Hoàng Thị Hương', CAST(N'1972-08-16' AS Date), N'Làm Ruộng', N'0814293986', N'', N'966410428285', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (92, 114, N'Vũ Văn Thường', CAST(N'1985-06-22' AS Date), N'Làm ruộng', N'0663175702', N'', N'516682028770', N'Vũ Thị Hương', CAST(N'1987-01-29' AS Date), N'Làm Ruộng', N'0562453818', N'', N'939747071266', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (93, 115, N'Trần Văn Thanh', CAST(N'1980-03-09' AS Date), N'Kinh doanh', N'0412384599', N'', N'418125385046', N'Nguyễn Thị Lan', CAST(N'1980-06-23' AS Date), N'Giáo viên', N'0576122516', N'', N'993315023183', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (94, 116, N'Nguyễn Văn Hạnh', CAST(N'1971-12-09' AS Date), N'Làm ruộng', N'0530352389', N'', N'464350736141', N'Nguyễn Thị Hạnh', CAST(N'1987-03-28' AS Date), N'Làm ruộng', N'0904421842', N'', N'855627524852', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (95, 117, N'Trần Văn Hùng', CAST(N'1982-04-26' AS Date), N'Làm ruộng', N'0448415571', N'', N'342758423089', N'Trần Thị Bình', CAST(N'1983-08-24' AS Date), N'Làm ruộng', N'0780357605', N'', N'468390947580', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (96, 118, N'Nguyễn Văn Tụng', CAST(N'1976-10-06' AS Date), N'Làm ruộng', N'0272076368', N'', N'150034141540', N'Bùi Thị Hoài', CAST(N'1978-03-26' AS Date), N'Làm ruộng', N'0763799095', N'', N'824747896194', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (97, 119, N'Nguyễn Văn Ba', CAST(N'1976-01-16' AS Date), N'Làm ruộng', N'0886292952', N'', N'319460648298', N'Dương Thị Dung', CAST(N'1980-11-29' AS Date), N'Làm ruộng', N'0798478287', N'', N'990485399961', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (98, 120, N'Phạm Văn Tuấn', CAST(N'1979-08-01' AS Date), N'Công nhân', N'0531796276', N'', N'981170141696', N'Nguyễn Thị Khuyên', CAST(N'1986-06-25' AS Date), N'Công nhân', N'0683179724', N'', N'332597839832', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (99, 121, N'Nguyễn Văn Đỉnh', CAST(N'1976-02-10' AS Date), N'Làm ruộng', N'0899577075', N'', N'154824620485', N'Trần Thị Thúy', CAST(N'1972-07-02' AS Date), N'Làm ruộng', N'0331608372', N'', N'257035666704', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (100, 122, N'Vũ Văn Cường', CAST(N'1985-08-30' AS Date), N'Công nhân', N'0477112364', N'', N'451262307167', N'Nguyễn Thị Thơm', CAST(N'1970-07-09' AS Date), N'Công nhân', N'0272473549', N'', N'725146746635', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (101, 123, N'Nguyễn Phúc Hạnh', CAST(N'1987-07-19' AS Date), N'Làm ruộng', N'0320623439', N'', N'428495877981', N'Trần Thị Tươi', CAST(N'1989-01-21' AS Date), N'Làm ruộng', N'0810174649', N'', N'115289825201', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (102, 124, N'Nguyễn Văn Hợp', CAST(N'1973-11-10' AS Date), N'Làm ruộng', N'0505489385', N'', N'540905272960', N'Đỗ Thị Duyên', CAST(N'1980-06-02' AS Date), N'Làm ruộng', N'0759931457', N'', N'405181634426', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (103, 125, N'Nguyễn Văn Giang', CAST(N'1982-08-24' AS Date), N'Làm ruộng', N'0973444038', N'', N'240096765756', N'Vũ Thị Nụ', CAST(N'1990-09-23' AS Date), N'Làm ruộng', N'0890830713', N'', N'723293071985', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (104, 126, N'Nguyễn Mạnh Tân', CAST(N'1985-09-23' AS Date), N'Làm ruộng', N'0495672607', N'', N'527013254165', N'Lương Thị Tâm', CAST(N'1971-06-19' AS Date), N'Làm ruộng', N'0951373910', N'', N'199348163604', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (105, 127, N'Nguyễn Văn Nhã', CAST(N'1975-07-16' AS Date), N'Làm ruộng', N'0669459313', N'', N'331540888547', N'Phạm Thị Bắc', CAST(N'1987-05-16' AS Date), N'Làm ruộng', N'0928771418', N'', N'198764675855', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (106, 128, N'Nguyễn Thanh Quang', CAST(N'1981-04-13' AS Date), N'Công nhân', N'0270431959', N'', N'665876686573', N'Triệu Thị Đào', CAST(N'1972-01-01' AS Date), N'Công nhân', N'0463247597', N'', N'361519777774', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (107, 129, N'Nguyễn Văn Lâm', CAST(N'1977-12-17' AS Date), N'Làm ruộng', N'0369265729', N'', N'587863188982', N'Lương Thị Tuyết', CAST(N'1988-06-29' AS Date), N'Làm ruộng', N'0928562957', N'', N'458990556001', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (108, 130, N'Vũ Văn Hợp', CAST(N'1982-06-01' AS Date), N'Đã mất', N'0161259293', N'', N'240671992301', N'Vương Thị Huệ', CAST(N'1986-03-20' AS Date), N'Công nhân', N'0920135188', N'', N'603119969367', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (109, 131, N'Mai Văn Chinh', CAST(N'1977-02-19' AS Date), N'Làm ruộng', N'0238543981', N'', N'944690650701', N'Nguyễn Thị Hòa', CAST(N'1971-06-14' AS Date), N'Làm ruộng', N'0101613563', N'', N'379856485128', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (110, 132, N'Trần Tuấn', CAST(N'1973-03-12' AS Date), N'Làm ruộng', N'0690643775', N'', N'689049470424', N'Vương Thị Quế', CAST(N'1983-03-29' AS Date), N'Làm ruộng', N'0532343232', N'', N'366022670269', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (111, 133, N'Đỗ Mạnh Tuấn', CAST(N'1972-08-22' AS Date), N'Làm ruộng', N'0151232820', N'', N'555478626489', N'Phạm Thị Hoa', CAST(N'1985-10-25' AS Date), N'Làm ruộng', N'0983409839', N'', N'827146917581', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (112, 134, N'Vũ Văn Chí', CAST(N'1979-06-11' AS Date), N'Làm ruộng', N'0612550401', N'', N'451424312591', N'Bùi Thị Mơ', CAST(N'1972-07-22' AS Date), N'Làm ruộng', N'0194193172', N'', N'378655767440', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (113, 135, N'Trần Văn Hải', CAST(N'1984-03-02' AS Date), N'Làm ruộng', N'0618093508', N'', N'196637791395', N'Nguyễn Thị Sen', CAST(N'1985-09-24' AS Date), N'Làm ruộng', N'0588643711', N'', N'438234442472', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (114, 136, N'Mai Văn Doanh', CAST(N'1988-01-15' AS Date), N'Làm ruộng', N'0876726639', N'', N'805595743656', N'Vũ Thị Tho', CAST(N'1971-04-10' AS Date), N'Làm ruộng', N'0768640482', N'', N'460932743549', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (115, 137, N'Vũ Văn Thanh', CAST(N'1985-10-26' AS Date), N'Làm ruộng', N'0343789893', N'', N'513676720857', N'Vũ Thị Hoan', CAST(N'1984-03-05' AS Date), N'Làm ruộng', N'0807355004', N'', N'857284861803', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (116, 138, N'Vũ Anh Thơi', CAST(N'1974-06-27' AS Date), N'Làm ruộng', N'0707462191', N'', N'778319287300', N'Trần Thị Hoa', CAST(N'1988-07-13' AS Date), N'Làm ruộng', N'0248846936', N'', N'468637442588', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (117, 139, N'Đỗ Văn Đạt', CAST(N'1987-05-15' AS Date), N'Làm ruộng', N'0340609115', N'', N'636485093832', N'Nguyễn Thị Gấm', CAST(N'1973-05-14' AS Date), N'Làm ruộng', N'0270214647', N'', N'752102893590', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (118, 140, N'Trần Văn Tăng', CAST(N'1977-03-26' AS Date), N'Làm ruộng', N'0112426888', N'', N'849457156658', N'Phạm Thị Quyên', CAST(N'1983-11-19' AS Date), N'Làm ruộng', N'0612330996', N'', N'692886960506', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (119, 141, N'Trần Văn Ngọc', CAST(N'1981-07-20' AS Date), N'Làm ruộng', N'0451947933', N'', N'116882520914', N'Trịnh Thị Xuân', CAST(N'1973-08-13' AS Date), N'Làm ruộng', N'0336073499', N'', N'465743499994', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (120, 142, N'Nguyễn Văn Quyết', CAST(N'1971-09-08' AS Date), N'Tư do', N'0862211704', N'', N'289331769943', N'Phạm Thị Thu', CAST(N'1976-04-12' AS Date), N'Công nhân', N'0208321332', N'', N'955297660827', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (121, 143, N'Nguyễn Văn Duy', CAST(N'1971-04-29' AS Date), N'Làm ruộng', N'0463689678', N'', N'166557997465', N'Phạm Thị Bích', CAST(N'1986-10-14' AS Date), N'Làm ruộng', N'0479901808', N'', N'681013840436', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (122, 144, N'Nguyễn Văn Sáng', CAST(N'1976-08-12' AS Date), N'Làm ruộng', N'0906197893', N'', N'194907391071', N'Trần Thị Phấn', CAST(N'1973-06-05' AS Date), N'Làm ruộng', N'0843938457', N'', N'564479315280', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (123, 145, N'Trần Văn Tấn', CAST(N'1977-04-07' AS Date), N'Làm ruộng', N'0206596833', N'', N'398524981737', N'Phạm Thị Giang', CAST(N'1986-10-21' AS Date), N'Làm ruộng', N'0884244281', N'', N'300990849733', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (124, 146, N'Đỗ Văn Chiều', CAST(N'1974-06-19' AS Date), N'Làm ruộng', N'0815379261', N'', N'215424895286', N'Nguyễn Thị Oanh', CAST(N'1982-01-08' AS Date), N'Làm ruộng', N'0459829497', N'', N'624482369422', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (125, 147, N'Nguyễn Văn Ngọ', CAST(N'1975-09-18' AS Date), N'Làm ruộng', N'0477844232', N'', N'100217097997', N'Lê Thị Nhiệm', CAST(N'1988-06-17' AS Date), N'Công nhân', N'0562315791', N'', N'773679441213', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (126, 148, N'Vũ Văn Chiều', CAST(N'1982-01-02' AS Date), N'Làm ruộng', N'0567762553', N'', N'583114659786', N'Hoàng Thị Tươi', CAST(N'1974-08-08' AS Date), N'Công nhân', N'0110881078', N'', N'510823333263', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (127, 149, N'Vũ Đình Luyến', CAST(N'1973-03-13' AS Date), N'Làm ruộng', N'0534817892', N'', N'691349464654', N'Hoàng Thị Lụa', CAST(N'1976-07-01' AS Date), N'Làm ruộng', N'0265862709', N'', N'913936334848', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (128, 150, N'Vũ Văn Tình', CAST(N'1982-04-26' AS Date), N'Làm ruộng', N'0316970968', N'', N'589612579345', N'Nguyễn Thị Thảo', CAST(N'1988-01-30' AS Date), N'Làm ruộng', N'0192635345', N'', N'354713296890', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (129, 151, N'Nguyễn Văn Dương', CAST(N'1988-07-13' AS Date), N'Làm ruộng', N'0739304465', N'', N'844670647382', N'Mai Thị Hằng', CAST(N'1983-01-14' AS Date), N'Làm ruộng', N'0755914849', N'', N'700784510374', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (130, 152, N'Nguyễn Văn Thuấn', CAST(N'1970-04-05' AS Date), N'Làm ruộng', N'0109675800', N'', N'173704206943', N'Nguyễn Thị Lựu', CAST(N'1989-04-22' AS Date), N'Làm ruộng', N'0679034101', N'', N'901114571094', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (131, 153, N'Trần Văn Kết', CAST(N'1983-12-10' AS Date), N'Làm ruộng', N'0255196315', N'', N'272730237245', N'Nguyễn Thị Tốt', CAST(N'1989-04-07' AS Date), N'Làm ruộng', N'0739553052', N'', N'353243285417', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (132, 154, N'Vũ Văn Đồng', CAST(N'1975-11-04' AS Date), N'Làm ruộng', N'0942481207', N'', N'711022019386', N'Đỗ Thị Lụa', CAST(N'1976-03-13' AS Date), N'Làm ruộng', N'0562116074', N'', N'431250452995', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (133, 155, N'Vũ Văn Lương', CAST(N'1981-08-19' AS Date), N'Làm ruộng', N'0627837222', N'', N'508787053823', N'Nguyễn Thị Sen', CAST(N'1985-05-23' AS Date), N'Làm ruộng', N'0800817388', N'', N'112799018621', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (134, 156, N'Bùi Văn Hà', CAST(N'1984-11-03' AS Date), N'Làm ruộng', N'0773887097', N'', N'421320807933', N'Nguyễn Thị Hoa', CAST(N'1974-08-17' AS Date), N'Làm ruộng', N'0909292304', N'', N'365193116664', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (135, 157, N'Nguyễn Văn Đông', CAST(N'1978-10-09' AS Date), N'Làm ruộng', N'0198133713', N'', N'234982973337', N'Nguyễn Thị Na', CAST(N'1980-02-15' AS Date), N'Làm ruộng', N'0978880983', N'', N'985641437768', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (136, 158, N'Vũ Văn Chính', CAST(N'1984-05-26' AS Date), N'Làm ruộng', N'0931955146', N'', N'733956193923', N'Phạm Thị Tím', CAST(N'1984-03-04' AS Date), N'Làm ruộng', N'0878824663', N'', N'785154628753', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (137, 159, N'Trần Việt Thành', CAST(N'1979-11-07' AS Date), N'Làm ruộng', N'0529557007', N'', N'935907381772', N'Trần Thị Lan', CAST(N'1982-03-18' AS Date), N'Làm ruộng', N'0571614474', N'', N'622790592908', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (138, 160, N'Phạm Văn Luyện', CAST(N'1985-11-07' AS Date), N'Công nhân', N'0783302938', N'', N'577191269397', N'Trần Thị Hồng', CAST(N'1982-07-21' AS Date), N'Giáo viên', N'0559132492', N'', N'645106089115', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (139, 161, N'Trần Văn Chinh', CAST(N'1990-12-28' AS Date), N'Làm ruộng', N'0351160603', N'', N'180677253007', N'Vũ Thị Cửu', CAST(N'1986-12-23' AS Date), N'Làm ruộng', N'0867666083', N'', N'341239434480', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (140, 162, N'Nguyễn Văn Hưng', CAST(N'1980-08-15' AS Date), N'Làm ruộng', N'0154051232', N'', N'781956362724', N'Nguyễn Thị Hiền', CAST(N'1989-09-13' AS Date), N'Làm ruộng', N'0422552371', N'', N'557349896430', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (141, 163, N'Phan Văn Thực', CAST(N'1973-08-19' AS Date), N'Làm ruộng', N'0444738477', N'', N'461657851934', N'Nguyễn Thị Xoan', CAST(N'1983-12-12' AS Date), N'Làm ruộng', N'0935182327', N'', N'814237016439', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (142, 164, N'Nguyễn Văn Hoàng', CAST(N'1987-05-02' AS Date), N'Làm ruộng', N'0418349349', N'', N'515686929225', N'Vũ Thị The', CAST(N'1986-12-11' AS Date), N'Làm ruộng', N'0909176003', N'', N'871670138835', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (143, 165, N'Lương Văn Ninh', CAST(N'1980-01-12' AS Date), N'Làm ruộng', N'0829248911', N'', N'542949062585', N'Nguyễn Thị Mây', CAST(N'1987-06-19' AS Date), N'Làm ruộng', N'0220294338', N'', N'183837324380', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (144, 166, N'Nguyễn Văn Hợp', CAST(N'1979-04-30' AS Date), N'Làm ruộng', N'0795103693', N'', N'286864566802', N'Vương Thị Quỳnh', CAST(N'1971-06-19' AS Date), N'Làm ruộng', N'0481391239', N'', N'587686109542', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (145, 167, N'Vũ Đình Thuyên', CAST(N'1970-08-20' AS Date), N'Làm ruộng', N'0620628947', N'', N'396762341260', N'Nguyễn Thị Thế', CAST(N'1972-04-24' AS Date), N'Làm ruộng', N'0333494824', N'', N'473838728666', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (146, 168, N'Phạm Văn Đam', CAST(N'1981-01-23' AS Date), N'Kỹ sư', N'0118534386', N'', N'185886156558', N'Phạm Thị Ánh', CAST(N'1985-02-11' AS Date), N'Công chức xã', N'0614751207', N'', N'166033947467', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (147, 169, N'Lương Văn Hảo', CAST(N'1976-12-10' AS Date), N'Làm ruộng', N'0370124465', N'', N'630813294649', N'Phạm Thị Lan', CAST(N'1985-06-18' AS Date), N'Làm ruộng', N'0427030640', N'', N'506239062547', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (148, 170, N'Nguyễn Văn Hiến', CAST(N'1977-04-11' AS Date), N'Làm ruộng', N'0173185038', N'', N'252886128425', N'Nguyễn Thị Hà', CAST(N'1980-12-17' AS Date), N'Làm ruộng', N'0315796589', N'', N'729001402854', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (149, 171, N'Vũ Văn Đam', CAST(N'1974-08-22' AS Date), N'Làm ruộng', N'0609260886', N'', N'934854882955', N'Nguyễn Thị Cúc', CAST(N'1981-02-27' AS Date), N'Làm ruộng', N'0591435998', N'', N'549331325292', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (150, 172, N'Nguyễn Văn Giang', CAST(N'1975-07-18' AS Date), N'Làm ruộng', N'0609010636', N'', N'188136851787', N'Nguyễn Thị Thu', CAST(N'1971-06-30' AS Date), N'Làm ruộng', N'0782456004', N'', N'466240727901', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (151, 173, N'Trịnh Ngọc Hưởng', CAST(N'1990-12-08' AS Date), N'Công nhân', N'0454579502', N'', N'499476248025', N'Đỗ Thị Hoa', CAST(N'1984-03-14' AS Date), N'Công nhân', N'0924331289', N'', N'950565010309', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (152, 174, N'Vũ Văn Bốn', CAST(N'1987-10-13' AS Date), N'Làm ruộng', N'0877168560', N'', N'345652151107', N'Lương Thị Nhung', CAST(N'1974-11-16' AS Date), N'Làm ruộng', N'0547649526', N'', N'986184692382', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (153, 175, N'Nguyễn Văn Thắng', CAST(N'1983-06-15' AS Date), N'Làm ruộng', N'0350264424', N'', N'885292166471', N'Nguyễn Thị Thắm', CAST(N'1975-08-29' AS Date), N'Làm ruộng', N'0349612540', N'', N'632298058271', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (154, 176, N'Phan Văn Lợi', CAST(N'1972-12-06' AS Date), N'Công nhân', N'0552137720', N'', N'775618946552', N'Trần Thị Thim', CAST(N'1980-12-18' AS Date), N'Làm ruộng', N'0371720325', N'', N'578790724277', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (155, 177, N'Nguyễn Văn Trường', CAST(N'1982-09-04' AS Date), N'Làm ruộng', N'0651785165', N'', N'345648127794', N'Vũ Thị Trang', CAST(N'1970-06-15' AS Date), N'Làm ruộng', N'0390156489', N'', N'653564435243', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (156, 178, N'Nguyễn Đề Thám', CAST(N'1983-09-24' AS Date), N'Làm ruộng', N'0430790829', N'', N'706282019615', N'Nguyễn Thị Hạt', CAST(N'1982-07-07' AS Date), N'Làm ruộng', N'0474319434', N'', N'180238175392', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (157, 179, N'Phạm Văn Huy', CAST(N'1979-01-16' AS Date), N'Làm ruộng', N'0578679841', N'', N'330966037511', N'Đỗ Thị Thu', CAST(N'1981-10-14' AS Date), N'Làm ruộng', N'0422166293', N'', N'265982335805', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (158, 180, N'Vũ Đình Từ', CAST(N'1982-01-13' AS Date), N'Công chức xã', N'0724044787', N'', N'180906903743', N'Nguyễn Thị Ngần', CAST(N'1985-12-30' AS Date), N'Công nhân', N'0821368634', N'', N'355203711986', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (159, 181, N'Vũ Văn Duy', CAST(N'1972-02-10' AS Date), N'Làm ruộng', N'0489604002', N'', N'127855545282', N'Lô Thị Điệp', CAST(N'1971-06-30' AS Date), N'Làm ruộng', N'0864282852', N'', N'116928011178', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (160, 182, N'Vũ Văn Hà', CAST(N'1990-08-09' AS Date), N'Công nhân', N'0981714200', N'', N'390445899963', N'Nông Thị Tuyết', CAST(N'1979-08-30' AS Date), N'Công nhân', N'0107726478', N'', N'237339830398', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (161, 183, N'Phạm Văn Hinh', CAST(N'1978-06-02' AS Date), N'Làm ruộng', N'0757770079', N'', N'811115998029', N'Đoàn Thị Như Quỳnh', CAST(N'1976-01-29' AS Date), N'Làm ruộng', N'0379586762', N'', N'478100222349', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (162, 184, N'Phạm Văn Dũng', CAST(N'1979-06-29' AS Date), N'Làm ruộng', N'0316193020', N'', N'367532217502', N'Vũ Thị Loan', CAST(N'1976-06-25' AS Date), N'Làm ruộng', N'0101182425', N'', N'593581497669', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (163, 185, N'Lâm Văn Thìn', CAST(N'1975-03-16' AS Date), N'Làm ruộng', N'0649902468', N'', N'311754018068', N'Vũ Thị Thắm', CAST(N'1980-12-22' AS Date), N'Làm ruộng', N'0393615895', N'', N'602600318193', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (164, 186, N'Nguyễn Văn Vinh', CAST(N'1977-07-13' AS Date), N'Làm ruộng', N'0315589308', N'', N'532427239418', N'Đỗ Thị Xuân', CAST(N'1990-01-16' AS Date), N'Làm ruộng', N'0638404107', N'', N'802905917167', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (165, 187, N'Vũ Đình Đại', CAST(N'1975-01-20' AS Date), N'Làm ruộng', N'0836833232', N'', N'329141706228', N'Phạm Thị Thúy', CAST(N'1986-02-22' AS Date), N'Làm ruộng', N'0756523603', N'', N'138652938604', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (166, 188, N'Vũ Văn Thùy', CAST(N'1977-07-28' AS Date), N'Làm ruộng', N'0661938130', N'', N'406545913219', N'Vũ Thị Hoa', CAST(N'1975-02-09' AS Date), N'Làm ruộng', N'0488462722', N'', N'465170419216', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (167, 189, N'Trần Văn Tuấn', CAST(N'1978-08-12' AS Date), N'Công nhân', N'0788863426', N'', N'140440469980', N'Phạm Thị Thu', CAST(N'1979-06-22' AS Date), N'Công nhân', N'0302502542', N'', N'153092342615', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (168, 190, N'Nguyễn Văn Điều', CAST(N'1971-02-05' AS Date), N'Làm ruộng', N'0195117568', N'', N'534185266494', N'HoàngThị Nga', CAST(N'1974-05-16' AS Date), N'Làm ruộng', N'0624561548', N'', N'380653476715', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (169, 191, N'Bùi Tiến Phường', CAST(N'1973-10-09' AS Date), N'Công nhân', N'0534015053', N'', N'285415476560', N'Vũ Thị Tầm', CAST(N'1970-05-21' AS Date), N'Công nhân', N'0984599131', N'', N'642739361524', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (170, 192, N'Nguyễn Văn Luyến', CAST(N'1972-01-22' AS Date), N'Làm ruộng', N'0188092863', N'', N'878081047534', N'Hoàng Thị Yến', CAST(N'1975-10-01' AS Date), N'Làm ruộng', N'0369592583', N'', N'565923845767', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (171, 193, N'Vũ Văn Mạnh', CAST(N'1987-07-25' AS Date), N'Làm ruộng', N'0282298642', N'', N'629765731096', N'Bùi Thị Gấm', CAST(N'1989-02-18' AS Date), N'Làm ruộng', N'0988043624', N'', N'636793977022', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (172, 194, N'Bùi Văn Ninh', CAST(N'1990-07-03' AS Date), N'Làm ruộng', N'0524113678', N'', N'779417490959', N'Lê Thị Huyền', CAST(N'1987-02-22' AS Date), N'Làm ruộng', N'0506146311', N'', N'396662831306', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (173, 195, N'Phạm Xuân Lâm', CAST(N'1983-04-09' AS Date), N'Công an', N'0644121450', N'', N'935094779729', N'Cao Thị Huyền', CAST(N'1986-02-14' AS Date), N'Giáo viên', N'0250220817', N'', N'918368524312', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (174, 196, N'Trịnh Văn Ai', CAST(N'1984-05-12' AS Date), N'Làm ruộng', N'0391489493', N'', N'397915208339', N'Bùi Thị Cúc', CAST(N'1984-08-05' AS Date), N'Làm ruộng', N'0666599595', N'', N'793064677715', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (175, 197, N'Nguyễn Văn Kiện', CAST(N'1974-11-05' AS Date), N'Làm ruộng', N'0995961290', N'', N'588647037744', N'Mai Thị Rọi', CAST(N'1990-06-26' AS Date), N'Làm ruộng', N'0640406376', N'', N'618286520242', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (176, 198, N'Trần Văn Lệ', CAST(N'1978-08-18' AS Date), N'Làm ruộng', N'0936568117', N'', N'172622203826', N'Bùi Thị Cam', CAST(N'1990-09-18' AS Date), N'Làm ruộng', N'0143906688', N'', N'488440084457', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (177, 199, N'Nguyễn Văn Hòa', CAST(N'1985-02-10' AS Date), N'Làm ruộng', N'0971430987', N'', N'670934742689', N'Nguyễn Thị Thêu', CAST(N'1980-02-05' AS Date), N'Công nhân', N'0377393788', N'', N'759272116422', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (178, 200, N'Vũ Đình Tân', CAST(N'1970-10-29' AS Date), N'Công nhân', N'0340542328', N'', N'469033014774', N'Tạ Thị Phương', CAST(N'1982-07-28' AS Date), N'Làm ruộng', N'0688718378', N'', N'246647846698', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (179, 201, N'Vũ Văn Quyền', CAST(N'1976-03-30' AS Date), N'Làm ruộng', N'0380858451', N'', N'964380532503', N'Trần Thị Thủy', CAST(N'1983-04-10' AS Date), N'Làm ruộng', N'0488136941', N'', N'153655177354', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (180, 202, N'Phạm Văn Hùng', CAST(N'1973-06-11' AS Date), N'Làm ruộng', N'0810709643', N'', N'203160977363', N'Lò Thị Hồng', CAST(N'1980-04-03' AS Date), N'Làm ruộng', N'0183454251', N'', N'318979620933', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (181, 203, N'Trần Văn Tuấn', CAST(N'1976-12-18' AS Date), N'Làm ruộng', N'0287507385', N'', N'931100648641', N'Vũ Thị Tin', CAST(N'1988-01-02' AS Date), N'Làm ruộng', N'0835533326', N'', N'782716983556', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (182, 204, N'Nguyễn Văn Dực', CAST(N'1981-02-26' AS Date), N'Công nhân', N'0401810204', N'', N'658188617229', N'Trần Thị Hường', CAST(N'1970-11-06' AS Date), N'Làm ruộng', N'0508953082', N'', N'956122815608', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (183, 205, N'Vũ Văn Trường', CAST(N'1970-10-23' AS Date), N'Làm ruộng', N'0993563610', N'', N'412953764200', N'Nguyễn Thị Huệ', CAST(N'1990-08-05' AS Date), N'Làm ruộng', N'0368472868', N'', N'410801559686', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (184, 206, N'Tống Văn Phương', CAST(N'1975-12-15' AS Date), N'Làm ruộng', N'0408067798', N'', N'234321165084', N'Trần Thị Mai', CAST(N'1975-12-07' AS Date), N'Làm ruộng', N'0944326353', N'', N'715826606750', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (185, 207, N'Bùi Văn Trưởng', CAST(N'1985-05-08' AS Date), N'Làm ruộng', N'0114012020', N'', N'531980437040', N'Vũ Thị Hằng', CAST(N'1977-12-09' AS Date), N'Làm ruộng', N'0272277373', N'', N'812317627668', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (186, 208, N'Nguyễn Hữu Luận', CAST(N'1982-07-15' AS Date), N'Giáo viên', N'0791558992', N'', N'297468197345', N'Bùi Thị Nhung', CAST(N'1978-02-14' AS Date), N'Giáo viên', N'0865639889', N'', N'131896078586', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (187, 209, N'Phạm Văn Truyển', CAST(N'1983-10-03' AS Date), N'Công nhân', N'0441529160', N'', N'994358187913', N'Mai Thị Vui', CAST(N'1971-12-30' AS Date), N'Công nhân', N'0822655612', N'', N'814756184816', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (188, 210, N'Vũ Văn Thắng', CAST(N'1973-02-02' AS Date), N'Làm ruộng', N'0437535405', N'', N'217378401756', N'Trần Việt Hà', CAST(N'1989-04-20' AS Date), N'Làm ruộng', N'0634048628', N'', N'535139489173', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (189, 211, N'Nguyễn Văn Hùng', CAST(N'1981-12-08' AS Date), N'Làm ruộng', N'0630516427', N'', N'125997203588', N'Trần Thị Hiền', CAST(N'1989-02-16' AS Date), N'Làm ruộng', N'0971299022', N'', N'285848706960', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (190, 212, N'Trần Văn Tới', CAST(N'1977-11-15' AS Date), N'Làm ruộng', N'0352324092', N'', N'410852468013', N'Nguyễn Thị Tin', CAST(N'1983-03-05' AS Date), N'Làm ruộng', N'0589009511', N'', N'641893661022', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (191, 213, N'Mai Văn Thiều', CAST(N'1974-11-03' AS Date), N'Công nhân', N'0453398901', N'', N'592901664972', N'Trần Thị Hằng', CAST(N'1984-12-05' AS Date), N'Công nhân', N'0916243034', N'', N'917990976572', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (192, 214, N'Nguyễn Văn Hạnh', CAST(N'1972-12-26' AS Date), N'Làm ruộng', N'0944431066', N'', N'930659103393', N'Nguyễn Thị Thu', CAST(N'1987-03-08' AS Date), N'Làm ruộng', N'0987197494', N'', N'950752496719', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (193, 215, N'Phạm Văn Sỹ', CAST(N'1987-12-08' AS Date), N'Làm ruộng', N'0407314795', N'', N'584421700239', N'Nguyễn Thị Tươi', CAST(N'1978-11-04' AS Date), N'Làm ruộng', N'0935119026', N'', N'688057535886', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (194, 216, N'Trần Văn Phúc', CAST(N'1983-08-31' AS Date), N'Làm ruộng', N'0954472935', N'', N'465779173374', N'Tôn Thị Hồng Thắm', CAST(N'1988-07-21' AS Date), N'Làm ruộng', N'0552749693', N'', N'113123619556', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (195, 217, N'Trần Văn Thoại', CAST(N'1978-01-28' AS Date), N'Làm ruộng', N'0874320536', N'', N'862520962953', N'Đồng Thị Thủy', CAST(N'1970-01-13' AS Date), N'Làm ruộng', N'0784421902', N'', N'695731765031', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (196, 218, N'Lê Văn Khánh', CAST(N'1973-03-19' AS Date), N'Làm ruộng', N'0424561667', N'', N'843602967262', N'Tống Thị Hằng', CAST(N'1972-09-14' AS Date), N'Làm ruộng', N'0529462647', N'', N'968238139152', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (197, 219, N'Nguyễn Văn Hải', CAST(N'1977-07-29' AS Date), N'Làm ruộng', N'0863236469', N'', N'705184835195', N'Vũ Thị Ngân', CAST(N'1981-11-30' AS Date), N'Làm ruộng', N'0592918294', N'', N'108476585149', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (198, 220, N'Nguyễn Đình Tài', CAST(N'1981-12-24' AS Date), N'Làm ruộng', N'0422767484', N'', N'749705588817', N'Lâm Thị Hường', CAST(N'1989-12-14' AS Date), N'Làm ruộng', N'0650567710', N'', N'958238542079', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (199, 221, N'Nguyễn Mạnh Linh', CAST(N'1978-10-18' AS Date), N'Làm ruộng', N'0186258178', N'', N'997094255685', N'Vũ Thị Thủy', CAST(N'1982-08-25' AS Date), N'Làm ruộng', N'0802320390', N'', N'166270786523', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (200, 222, N'Bùi Xuân Ánh', CAST(N'1979-01-02' AS Date), N'Làm ruộng', N'0563284873', N'', N'405825471878', N'Vũ Thị Vân', CAST(N'1975-02-03' AS Date), N'Làm ruộng', N'0666709566', N'', N'363969707489', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (201, 223, N'Trần Trung Nghĩa', CAST(N'1977-05-12' AS Date), N'Làm ruộng', N'0148458451', N'', N'545690172910', N'Đinh Thị Huệ', CAST(N'1970-01-16' AS Date), N'Làm ruộng', N'0733350390', N'', N'624235981702', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (202, 224, N'Phạm Văn Quang', CAST(N'1983-03-14' AS Date), N'Làm ruộng', N'0987926733', N'', N'471671020984', N'Trịnh Thị Thủy', CAST(N'1988-06-20' AS Date), N'Làm ruộng', N'0597752869', N'', N'827098047733', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (203, 225, N'Mai Khương Duy', CAST(N'1973-12-09' AS Date), N'Làm ruộng', N'0753304845', N'', N'725753623247', N'Trần Thị Huệ', CAST(N'1979-04-20' AS Date), N'Làm ruộng', N'0980320578', N'', N'136279183626', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (204, 226, N'Phạm Duy Thành', CAST(N'1971-10-23' AS Date), N'Làm ruộng', N'0399571633', N'', N'261180377006', N'Đào Thị Lan', CAST(N'1982-03-18' AS Date), N'Làm ruộng', N'0299042010', N'', N'149183773994', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (205, 227, N'Bùi Văn Giang', CAST(N'1975-10-27' AS Date), N'Làm ruộng', N'0752317899', N'', N'480626434087', N'Mai Thị Lệ', CAST(N'1987-07-30' AS Date), N'Làm ruộng', N'0662354034', N'', N'607876008749', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (206, 228, N'Vũ Tiến Định', CAST(N'1990-04-14' AS Date), N'Làm ruộng', N'0863189208', N'', N'588859307765', N'Trần Thị Thúy', CAST(N'1985-02-14' AS Date), N'Làm ruộng', N'0957739007', N'', N'383331286907', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (207, 229, N'Vũ Văn Thuyên', CAST(N'1989-12-16' AS Date), N'Làm ruộng', N'0791994959', N'', N'833197551965', N'Phạm Thị Ngọc', CAST(N'1986-07-01' AS Date), N'Làm ruộng', N'0584370952', N'', N'471119505167', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (208, 230, N'Nguyễn Văn Thơ', CAST(N'1988-07-14' AS Date), N'Làm ruộng', N'0615068674', N'', N'586822223663', N'Nguyễn Thị Thơm', CAST(N'1979-08-01' AS Date), N'Làm ruộng', N'0559864521', N'', N'559042692184', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (209, 231, N'Đặng Quý Hà', CAST(N'1987-09-27' AS Date), N'Làm ruộng', N'0236124628', N'', N'484779995679', N'Nguyễn Thị Ngọc', CAST(N'1987-06-19' AS Date), N'Làm ruộng', N'0185965603', N'', N'760159605741', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (210, 232, N'Vũ Văn Thường', CAST(N'1973-10-21' AS Date), N'Làm ruộng', N'0445875465', N'', N'558161318302', N'Vũ Thị Hương', CAST(N'1981-01-15' AS Date), N'Làm ruộng', N'0793666994', N'', N'505899441242', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (211, 233, N'Phạm Văn Tính', CAST(N'1989-11-24' AS Date), N'Làm ruộng', N'0966616851', N'', N'305003434419', N'Lê Thị Loãn', CAST(N'1983-02-08' AS Date), N'Làm ruộng', N'0681298905', N'', N'390158206224', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (212, 234, N'Nguyễn Trung Thành', CAST(N'1986-06-23' AS Date), N'Làm ruộng', N'0848161005', N'', N'657268834114', N'Nguyễn Thị Hài', CAST(N'1971-05-28' AS Date), N'Làm ruộng', N'0779944920', N'', N'166697096824', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (213, 235, N'Trần Văn Chung', CAST(N'1981-02-09' AS Date), N'Làm ruộng', N'0190973609', N'', N'540847390890', N'Trần Thị Sen', CAST(N'1985-10-31' AS Date), N'Làm ruộng', N'0918131631', N'', N'467884868383', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (214, 236, N'Trần Văn Bắc', CAST(N'1979-11-26' AS Date), N'Làm ruộng', N'0643950593', N'', N'712737452983', N'Nguyễn Thị Ngát', CAST(N'1986-01-09' AS Date), N'Làm ruộng', N'0944947230', N'', N'615658223628', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (215, 237, N'Nguyễn Văn Hiển', CAST(N'1978-12-16' AS Date), N'Làm ruộng', N'0409525257', N'', N'897940069437', N'Trần Thị Sen', CAST(N'1987-09-30' AS Date), N'Làm ruộng', N'0609123235', N'', N'787078148126', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (216, 238, N'Nguyễn Văn Duy', CAST(N'1990-06-24' AS Date), N'Làm ruộng', N'0833034420', N'', N'433463811874', N'Trần Thị Hiên', CAST(N'1973-10-25' AS Date), N'Làm ruộng', N'0976476812', N'', N'622348403930', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (217, 239, N'Nguyễn Quang Sáng', CAST(N'1976-01-28' AS Date), N'Làm ruộng', N'0120557469', N'', N'372247809171', N'Phạm Thị Mai', CAST(N'1970-10-09' AS Date), N'Làm ruộng', N'0613521307', N'', N'686697548627', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (218, 240, N'Nguyễn Văn Sang', CAST(N'1982-11-25' AS Date), N'Làm ruộng', N'0527586710', N'', N'363580143451', N'Tạ Thị Hoa', CAST(N'1985-05-15' AS Date), N'Làm ruộng', N'0328822147', N'', N'826920378208', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (219, 241, N'Phạm Văn Nam', CAST(N'1986-12-02' AS Date), N'Làm ruộng', N'0166453820', N'', N'235280162096', N'Đỗ Thị Thu', CAST(N'1972-09-02' AS Date), N'Làm ruộng', N'0972366648', N'', N'475191098451', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (220, 242, N'Nguyễn Văn Dũng', CAST(N'1987-03-09' AS Date), N'Làm ruộng', N'0863737988', N'', N'576839041709', N'Nguyễn Thị Hoài', CAST(N'1984-01-15' AS Date), N'Làm ruộng', N'0967142081', N'', N'817311310768', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (221, 243, N'Nguyễn Văn Bảo', CAST(N'1971-11-30' AS Date), N'Làm ruộng', N'0548978990', N'', N'300935596227', N'Vũ Thị Thoa', CAST(N'1981-02-05' AS Date), N'Làm ruộng', N'0325338977', N'', N'748903554677', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (222, 244, N'Phạm Văn Thịnh', CAST(N'1975-07-11' AS Date), N'Làm ruộng', N'0605725467', N'', N'626076352596', N'Trần Thị Hằng', CAST(N'1977-04-27' AS Date), N'Làm ruộng', N'0566928708', N'', N'724018180370', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (223, 245, N'Phạm Văn Quân', CAST(N'1974-03-05' AS Date), N'Làm ruộng', N'0566827589', N'', N'427112823724', N'Nguyễn Thị Hồng', CAST(N'1989-01-29' AS Date), N'Làm ruộng', N'0562368255', N'', N'757750231027', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (224, 246, N'Đồng Văn Suốt', CAST(N'1982-08-27' AS Date), N'Làm ruộng', N'0683246564', N'', N'888377189636', N'Phạm Thị Hà', CAST(N'1981-12-05' AS Date), N'Làm ruộng', N'0609173393', N'', N'273076295852', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (225, 247, N'Nguyễn Văn Bằng', CAST(N'1971-03-20' AS Date), N'Làm ruộng', N'0953563612', N'', N'530212754011', N'Nguyễn Thị Lê', CAST(N'1989-08-23' AS Date), N'Làm ruộng', N'0988136643', N'', N'796281450986', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (226, 248, N'Mai Văn Phòng', CAST(N'1979-04-26' AS Date), N'Làm ruộng', N'0927640545', N'', N'271570074558', N'Phạm Thị Nhạn', CAST(N'1978-11-27' AS Date), N'Làm ruộng', N'0936860907', N'', N'162865936756', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (227, 249, N'Phạm Văn Phiên', CAST(N'1976-12-13' AS Date), N'Làm ruộng', N'0619075518', N'', N'715656071901', N'Nguyễn Thị Chung', CAST(N'1986-10-12' AS Date), N'Làm ruộng', N'0702596074', N'', N'323262625932', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (228, 250, N'Nguyễn Văn Thọ', CAST(N'1986-10-19' AS Date), N'Làm ruộng', N'0397523283', N'', N'772674202919', N'Nguyễn Thị Nhung', CAST(N'1987-10-02' AS Date), N'Làm ruộng', N'0263416695', N'', N'955372118949', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (229, 251, N'Hoàng Văn Chính', CAST(N'1977-08-21' AS Date), N'Làm ruộng', N'0622671931', N'', N'652541440725', N'Vũ Thị Lụa', CAST(N'1982-04-20' AS Date), N'Làm ruộng', N'0225626462', N'', N'837894958257', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (230, 252, N'Phạm Văn Quận', CAST(N'1982-01-27' AS Date), N'Làm ruộng', N'0309500157', N'', N'803924834728', N'Đoàn Thị Hà', CAST(N'1982-12-01' AS Date), N'Làm ruộng', N'0748732268', N'', N'297522485256', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (231, 253, N'Phạm Văn Thịnh', CAST(N'1971-02-09' AS Date), N'Làm ruộng', N'0330942970', N'', N'245569330453', N'Trần Thị Hằng', CAST(N'1981-05-09' AS Date), N'Làm ruộng', N'0818959528', N'', N'265801769495', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (232, 254, N'Vũ Văn Phúc', CAST(N'1979-06-26' AS Date), N'Làm ruộng', N'0714582061', N'', N'827001810073', N'Mai Thị Thủy', CAST(N'1978-11-08' AS Date), N'Làm ruộng', N'0583393716', N'', N'313228321075', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (233, 255, N'Phạm Văn Thơi', CAST(N'1971-11-08' AS Date), N'Làm ruộng', N'0638512200', N'', N'282356470823', N'Vũ Thị Quyên', CAST(N'1981-05-26' AS Date), N'Làm ruộng', N'0183493250', N'', N'932905453443', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (234, 256, N'Vũ Văn Nhật', CAST(N'1971-10-02' AS Date), N'Làm ruộng', N'0285929548', N'', N'961086189746', N'Nguyễn Thị Bình', CAST(N'1976-11-05' AS Date), N'Làm ruộng', N'0146738350', N'', N'331753695011', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (235, 257, N'Vũ Đình Đăng', CAST(N'1972-09-08' AS Date), N'Làm ruộng', N'0554804211', N'', N'159265929460', N'Vũ Thị  Miến', CAST(N'1981-12-24' AS Date), N'Làm ruộng', N'0535121947', N'', N'330320054292', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (236, 258, N'Nguyễn Văn Năm', CAST(N'1987-03-08' AS Date), N'Làm ruộng', N'0708550095', N'', N'155370020866', N'Nguyễn Thị Thu', CAST(N'1977-07-19' AS Date), N'Làm ruộng', N'0729364466', N'', N'743037724494', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (237, 259, N'Nguyễn Văn Thịnh', CAST(N'1980-08-19' AS Date), N'Làm ruộng', N'0684952825', N'', N'813877815008', N'Vũ Thị Nhung', CAST(N'1978-11-01' AS Date), N'Làm ruộng', N'0287206977', N'', N'698384469747', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (238, 260, N'Trần Văn Tứ', CAST(N'1972-12-12' AS Date), N'Làm ruộng', N'0286573493', N'', N'441644227504', N'Nguyễn Thị Hương', CAST(N'1974-08-13' AS Date), N'Làm ruộng', N'0235719239', N'', N'795594966411', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (239, 261, N'Nguyễn Văn Khiêm', CAST(N'1977-01-28' AS Date), N'Làm ruộng', N'0357974916', N'', N'517225605249', N'Nguyễn Thị Thu Hồng', CAST(N'1970-07-17' AS Date), N'Làm ruộng', N'0192813068', N'', N'782961279153', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (240, 262, N'Phạm Văn Thuyên', CAST(N'1980-05-29' AS Date), N'Làm ruộng', N'0308730363', N'', N'207589626312', N'Trần Thị Yến', CAST(N'1971-05-07' AS Date), N'Làm ruộng', N'0407389736', N'', N'377618932723', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (241, 263, N'Trần Văn Ba', CAST(N'1975-07-02' AS Date), N'Làm ruộng', N'0580334872', N'', N'203923100233', N'Nguyễn Thị Lan', CAST(N'1971-03-26' AS Date), N'Làm ruộng', N'0124835270', N'', N'842126196622', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (242, 264, N'Hoàng Văn Hoa', CAST(N'1987-06-17' AS Date), N'Làm ruộng', N'0237658798', N'', N'256396067142', N'Lương Thị Lái', CAST(N'1978-01-22' AS Date), N'Làm ruộng', N'0732722055', N'', N'746913731098', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (243, 265, N'Vũ Văn Dưỡng', CAST(N'1977-02-21' AS Date), N'Làm ruộng', N'0918024665', N'', N'143307000398', N'Phạm Thị Dịu', CAST(N'1989-05-14' AS Date), N'Làm ruộng', N'0697141534', N'', N'906373149156', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (244, 266, N'Vũ Minh Điềm', CAST(N'1974-03-11' AS Date), N'Làm ruộng', N'0313664126', N'', N'743334698677', N'Nguyễn Thị Nhung', CAST(N'1974-10-25' AS Date), N'Làm ruộng', N'0333393597', N'', N'300278830528', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (245, 267, N'Vũ Quý Thạch', CAST(N'1978-09-04' AS Date), N'Làm ruộng', N'0235284560', N'', N'579720109701', N'Phạm Thị Miên', CAST(N'1981-12-29' AS Date), N'Giáo viên', N'0754855912', N'', N'420459979772', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (246, 268, N'Phạm Văn Tuyên', CAST(N'1979-10-18' AS Date), N'Làm ruộng', N'0688556802', N'', N'604908359050', N'Vũ Thị Dung', CAST(N'1984-12-12' AS Date), N'Làm ruộng', N'0543563449', N'', N'597971951961', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (247, 269, N'Phạm Văn Hiến', CAST(N'1986-04-03' AS Date), N'Làm ruộng', N'0643089443', N'', N'295060163736', N'Nguyễn Thị Mỳ', CAST(N'1988-05-15' AS Date), N'Làm ruộng', N'0886907392', N'', N'420019775629', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (248, 270, N'Nguyễn Văn Tuấn', CAST(N'1983-07-22' AS Date), N'Làm ruộng', N'0830193424', N'', N'521205091476', N'Trần Thị Huyền', CAST(N'1980-09-19' AS Date), N'Làm ruộng', N'0222225904', N'', N'283875083923', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (249, 271, N'Nguyễn Văn Tiến', CAST(N'1977-08-05' AS Date), N'Làm ruộng', N'0735525768', N'', N'771719282865', N'Vũ Thị Hiền', CAST(N'1974-04-19' AS Date), N'Làm ruộng', N'0838969343', N'', N'621062821149', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (250, 272, N'Phạm Văn Chung', CAST(N'1989-05-26' AS Date), N'Làm ruộng', N'0463345873', N'', N'777079784870', N'Lo Thị Coi', CAST(N'1975-02-05' AS Date), N'Làm ruộng', N'0364392101', N'', N'666988623142', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (251, 273, N'Vũ Văn Dụ', CAST(N'1973-06-02' AS Date), N'Làm ruộng', N'0417184144', N'', N'459039050340', N'Nguyễn Thị Nhan', CAST(N'1978-01-22' AS Date), N'Làm ruộng', N'0529914599', N'', N'225494176149', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (252, 274, N'Đặng Xuân Hinh', CAST(N'1985-11-09' AS Date), N'Làm ruộng', N'0837523579', N'', N'912788939476', N'Vũ Thị Liễu', CAST(N'1990-06-14' AS Date), N'Làm ruộng', N'0601724791', N'', N'154878640174', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (253, 275, N'Mai Văn Phong', CAST(N'1978-07-01' AS Date), N'Làm ruộng', N'0249692529', N'', N'771406215429', N'Trần Thị Bích', CAST(N'1988-03-16' AS Date), N'Làm ruộng', N'0199911159', N'', N'670771878957', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (254, 276, N'Vũ Đình Huy', CAST(N'1983-12-03' AS Date), N'Làm ruộng', N'0837373912', N'', N'179703557491', N'Nguyễn Thị Lan', CAST(N'1973-04-09' AS Date), N'Làm ruộng', N'0108251225', N'', N'854702270030', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (255, 277, N'Phan Văn Lợi', CAST(N'1986-01-30' AS Date), N'Làm ruộng', N'0970515066', N'', N'671114021539', N'Trần Thị Thim', CAST(N'1978-06-24' AS Date), N'Làm ruộng', N'0843283516', N'', N'676830774545', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (256, 278, N'Phạm Văn Chỉnh', CAST(N'1990-09-21' AS Date), N'Làm ruộng', N'0676285696', N'', N'741725158691', N'Nguyễn Thị Hồng', CAST(N'1973-03-31' AS Date), N'Làm ruộng', N'0351779174', N'', N'482436227798', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (257, 279, N'Trịnh Văn Quân', CAST(N'1973-03-30' AS Date), N'Làm ruộng', N'0962141531', N'', N'114114159345', N'Phạm Thị Thanh', CAST(N'1988-05-15' AS Date), N'Làm ruộng', N'0904264611', N'', N'268396967649', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (258, 280, N'Nguyễn Văn Hải', CAST(N'1984-11-04' AS Date), N'Làm ruộng', N'0938820850', N'', N'488029921054', N'Tạ Thị Mai', CAST(N'1985-12-01' AS Date), N'Làm ruộng', N'0156641066', N'', N'545933449268', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (259, 281, N'Phan Văn Lương', CAST(N'1973-04-01' AS Date), N'Làm ruộng', N'0474792414', N'', N'461784344911', N'Phạm Thị Thu', CAST(N'1989-07-16' AS Date), N'Làm ruộng', N'0738763409', N'', N'975817900896', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (260, 282, N'Vũ Văn Hiền', CAST(N'1976-11-08' AS Date), N'Làm ruộng', N'0262599158', N'', N'347765946388', N'Nguyễn Thị Duyên', CAST(N'1975-03-07' AS Date), N'Làm ruộng', N'0768391251', N'', N'792432856559', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (261, 283, N'Phạm Văn Tuyên', CAST(N'1984-10-03' AS Date), N'Làm ruộng', N'0930764621', N'', N'986836522817', N'Phạm Thế Biên', CAST(N'1975-11-29' AS Date), N'Làm ruộng', N'0470273107', N'', N'442533057928', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (262, 284, N'Lê Văn Tuyên', CAST(N'1987-10-07' AS Date), N'Làm nông', N'0575261151', N'', N'822328650951', N'Nguyễn Thị Luyến', CAST(N'1983-04-06' AS Date), N'Làm nông', N'0136081397', N'', N'536147248744', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (263, 285, N'Trần Văn Tiển', CAST(N'1972-08-03' AS Date), N'Làm ruộng', N'0463542801', N'', N'726490694284', N'Nguyễn Thị Đào', CAST(N'1972-08-19' AS Date), N'Làm ruộng', N'0493044954', N'', N'592310291528', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (264, 286, N'Phan Văn Sự', CAST(N'1983-06-29' AS Date), N'Làm ruộng', N'0227134132', N'', N'475839281082', N'Mai Thị Liên', CAST(N'1976-09-10' AS Date), N'Làm ruộng', N'0852739000', N'', N'706554317474', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (265, 287, N'Nguyễn Hữu Hạnh', CAST(N'1974-04-19' AS Date), N'Làm ruộng', N'0569801300', N'', N'761039370298', N'Vũ Phuơng Hoa', CAST(N'1983-05-12' AS Date), N'Làm ruộng', N'0600652712', N'', N'134372776746', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (266, 288, N'Nguyễn Ngọc Thi', CAST(N'1975-04-03' AS Date), N'Làm ruộng', N'0585226309', N'', N'349451553821', N'Nguyễn Thị Hương', CAST(N'1978-07-05' AS Date), N'Làm ruộng', N'0619674026', N'', N'683015787601', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (267, 289, N'Nguyễn Văn Lộc', CAST(N'1970-12-23' AS Date), N'Làm ruộng', N'0277421742', N'', N'420927649736', N'Vũ Thị Tám', CAST(N'1981-04-29' AS Date), N'Làm ruộng', N'0243072730', N'', N'909541589021', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (268, 290, N'Vũ Văn Đang', CAST(N'1978-06-14' AS Date), N'Làm ruộng', N'0579174065', N'', N'490111422538', N'Phạm Thị Yến', CAST(N'1974-12-24' AS Date), N'Làm ruộng', N'0925238680', N'', N'406349682807', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (269, 291, N'Nguyễn Văn Tiều', CAST(N'1987-05-22' AS Date), N'Làm ruộng', N'0357651227', N'', N'950473922491', N'Trần Thị Vui', CAST(N'1988-09-28' AS Date), N'Làm ruộng', N'0840404647', N'', N'205518907308', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (270, 292, N'Trần Văn Thụ', CAST(N'1977-05-08' AS Date), N'Làm ruộng', N'0714767348', N'', N'209394967555', N'Nguyễn Thị Huờng', CAST(N'1980-12-02' AS Date), N'Làm ruộng', N'0553665292', N'', N'482980716228', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (271, 293, N'Nguyễn Văn Hệ', CAST(N'1974-01-25' AS Date), N'Điện lực', N'0734526163', N'', N'681356197595', N'Đỗ Thị Nhẫn', CAST(N'1979-05-28' AS Date), N'Giáo viên', N'0506357723', N'', N'794436842203', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (272, 294, N'Nguyễn Văn Vinh', CAST(N'1987-04-13' AS Date), N'Làm ruộng', N'0495678186', N'', N'413049411773', N'Nguyễn Thị Xuyến', CAST(N'1978-02-15' AS Date), N'Làm ruộng', N'0164607334', N'', N'422293806076', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (273, 295, N'Vũ Văn Hiểu', CAST(N'1990-09-15' AS Date), N'Làm ruộng', N'0569686716', N'', N'193989056348', N'Vũ Thị Thảo', CAST(N'1974-07-17' AS Date), N'Làm ruộng', N'0559627789', N'', N'470796889066', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (274, 296, N'Vũ Văn Quynh', CAST(N'1987-05-05' AS Date), N'Làm ruộng', N'0419017326', N'', N'266862261295', N'Nguyễn Thị Thảo', CAST(N'1990-02-05' AS Date), N'Làm ruộng', N'0309008562', N'', N'772815716266', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (275, 297, N'Nguyễn Văn Bôn', CAST(N'1989-07-25' AS Date), N'Làm ruộng', N'0825706893', N'', N'169916230440', N'Hoàng Thị Trang', CAST(N'1985-03-25' AS Date), N'Làm ruộng', N'0226289826', N'', N'492925006151', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (276, 298, N'Trần Văn Tiên', CAST(N'1982-05-30' AS Date), N'Làm ruộng', N'0135344004', N'', N'489483571052', N'Nguyễn Thị Hiên', CAST(N'1972-04-19' AS Date), N'Làm ruộng', N'0671925282', N'', N'747849822044', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (277, 299, N'Trần Hiền Lương', CAST(N'1977-04-07' AS Date), N'Làm ruộng', N'0336065238', N'', N'963343805074', N'Vũ Thị Huệ', CAST(N'1985-02-04' AS Date), N'Làm ruộng', N'0261331170', N'', N'556067317724', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (278, 300, NULL, NULL, N'Unknown', NULL, NULL, NULL, N'Đỗ Thị Thanh Thủy', CAST(N'1973-08-18' AS Date), N'Công nhân', N'0457926738', N'', N'256189000606', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (279, 301, N'Nguyễn Văn Chính', CAST(N'1988-08-28' AS Date), N'Làm ruộng', N'0934881168', N'', N'544939047098', N'Nguyễn Thị Nhung', CAST(N'1989-06-01' AS Date), N'Làm ruộng', N'0442450338', N'', N'350251442193', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (280, 302, N'Phạm Quốc Hùng', CAST(N'1985-04-17' AS Date), N'Làm ruộng', N'0877669811', N'', N'975670003890', N'Vũ Thị Dung', CAST(N'1987-10-01' AS Date), N'Làm ruộng', N'0959698629', N'', N'178531646728', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (281, 303, N'Lâm Văn Sỹ', CAST(N'1987-02-23' AS Date), N'Làm ruộng', N'0524541920', N'', N'296019858121', N'Trần Thị Hương', CAST(N'1988-07-01' AS Date), N'Làm ruộng', N'0414246481', N'', N'981038445234', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (282, 304, N'Trần Văn Đại', CAST(N'1979-07-03' AS Date), N'Làm ruộng', N'0401961481', N'', N'441092121601', N'Mai Thị Lụa', CAST(N'1988-11-03' AS Date), N'Làm ruộng', N'0775474750', N'', N'355225813388', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (283, 305, N'Vũ Xuân Trường', CAST(N'1971-11-30' AS Date), N'Làm ruộng', N'0884345132', N'', N'456259852647', N'Trần Thị Bưởi', CAST(N'1972-03-28' AS Date), N'Làm ruộng', N'0985924464', N'', N'656290417909', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (284, 306, N'Lê Thế Hịch', CAST(N'1970-08-30' AS Date), N'Làm ruộng', N'0387017178', N'', N'353353095054', N'Đinh Thị Thủy', CAST(N'1988-04-15' AS Date), N'Làm ruộng', N'0465921759', N'', N'375966477394', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (285, 307, N'Tạ Văn Lực', CAST(N'1972-05-16' AS Date), N'Làm ruộng', N'0548282045', N'', N'553034061193', N'Đồng Thị Thúy', CAST(N'1977-02-22' AS Date), N'Giáo viên', N'0510640567', N'', N'867078679800', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (286, 308, N'Vũ Ngọc Oanh', CAST(N'1988-03-26' AS Date), N'Làm ruộng', N'0400394427', N'', N'224859082698', N'Vũ Kim Phượng', CAST(N'1980-03-14' AS Date), N'Làm ruộng', N'0366102063', N'', N'387070930004', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (287, 309, N'Phạm Văn Quang', CAST(N'1983-03-02' AS Date), N'Làm ruộng', N'0905086332', N'', N'565530258417', N'Trịnh Thị Thủy', CAST(N'1988-02-07' AS Date), N'Làm ruộng', N'0999613815', N'', N'815857607126', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (288, 310, N'Đỗ Văn Ban', CAST(N'1989-06-21' AS Date), N'Làm ruộng', N'0501673460', N'', N'968828010559', N'Trần Thị Phòng', CAST(N'1977-01-04' AS Date), N'Làm ruộng', N'0693139839', N'', N'256957292556', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (289, 311, N'Nguyễn Văn Xướng', CAST(N'1971-01-30' AS Date), N'Làm ruộng', N'0798673552', N'', N'601750916242', N'Hoàng Thị Huyền', CAST(N'1973-04-03' AS Date), N'Làm ruộng', N'0149127930', N'', N'370029085874', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (290, 312, N'Nguyễn Minh Hiển', CAST(N'1974-11-14' AS Date), N'Làm ruộng', N'0791138851', N'', N'918283712863', N'Tạ Thị Loan', CAST(N'1986-04-22' AS Date), N'Làm ruộng', N'0715215170', N'', N'165451157093', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (291, 313, N'Nguyễn Văn Thắng', CAST(N'1973-04-23' AS Date), N'Làm ruộng', N'0132096225', N'', N'691530781984', N'Nguyễn Thị Thắm', CAST(N'1984-08-25' AS Date), N'Làm ruộng', N'0583548909', N'', N'544022589921', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (292, 314, N'Vũ Văn Mạnh', CAST(N'1987-04-11' AS Date), N'Làm ruộng', N'0947914290', N'', N'497003197669', N'Bùi Thị Gấm', CAST(N'1978-07-22' AS Date), N'Làm ruộng', N'0272511315', N'', N'507545351982', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (293, 315, N'Nguyễn Văn Bảy', CAST(N'1988-11-13' AS Date), N'Làm ruộng', N'0803139537', N'', N'723695081472', N'Nguyễn Thị Lan', CAST(N'1972-03-03' AS Date), N'Làm ruộng', N'0342483228', N'', N'638015884160', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (294, 316, N'Trần Hữu Phước', CAST(N'1982-03-30' AS Date), N'Làm ruộng', N'0573175466', N'', N'158042037487', N'Tạ Thị Tuyết', CAST(N'1976-10-30' AS Date), N'Làm ruộng', N'0865740096', N'', N'127237832546', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (295, 317, NULL, NULL, N'Unknown', NULL, NULL, NULL, N'Nguyễn Thị Mai', CAST(N'1970-03-08' AS Date), N'Làm ruộng', N'0865201669', N'', N'386421352624', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (296, 318, N'Trần Văn Xương', CAST(N'1982-05-20' AS Date), N'Làm ruộng', N'0479066085', N'', N'730462884902', N'Trần Thị Cúc', CAST(N'1980-11-19' AS Date), N'Công nhân', N'0152870416', N'', N'222072696685', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (297, 319, N'Vũ Văn Bằng', CAST(N'1985-09-06' AS Date), N'Làm ruộng', N'0307950752', N'', N'547363871335', N'Nguyễn Thị Dung', CAST(N'1974-02-23' AS Date), N'Làm ruộng', N'0550453776', N'', N'394262951612', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (298, 320, N'Trần Văn Sơn', CAST(N'1971-03-07' AS Date), N'Làm ruộng', N'0788942015', N'', N'239892113208', N'Trần Thị Tuyết', CAST(N'1970-12-06' AS Date), N'Làm ruộng', N'0919684898', N'', N'784009325504', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (299, 321, N'Nguyễn Văn Quân', CAST(N'1990-03-21' AS Date), N'Làm ruộng', N'0511782437', N'', N'301363784074', N'Lâm Thị Hoa', CAST(N'1982-09-05' AS Date), N'Làm ruộng', N'0694797927', N'', N'664568787813', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (300, 322, N'Vũ Văn Hiếu', CAST(N'1989-04-08' AS Date), N'Làm ruộng', N'0939568543', N'', N'393529582023', N'Đỗ Thị Thơm', CAST(N'1973-02-25' AS Date), N'Làm ruộng', N'0914789652', N'', N'967244648933', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (301, 323, N'Vũ Văn Hoàng', CAST(N'1986-10-13' AS Date), N'Làm ruộng', N'0586038106', N'', N'806039756536', N'Bùi Thị Lan', CAST(N'1985-08-26' AS Date), N'Làm ruộng', N'0437572044', N'', N'844904321432', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (302, 324, N'Phạm Văn Huy', CAST(N'1973-07-04' AS Date), N'Làm ruộng', N'0600895774', N'', N'145236527919', N'Nguyễn Thị Hạnh', CAST(N'1988-01-16' AS Date), N'Làm ruộng', N'0364702165', N'', N'598613536357', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (303, 325, N'Bùi Văn Tuấn', CAST(N'1975-11-21' AS Date), N'Làm ruộng', N'0262273699', N'', N'566214329004', N'Vũ Thị Lượt', CAST(N'1985-11-17' AS Date), N'Làm ruộng', N'0765629917', N'', N'646171194314', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (304, 326, N'Vũ Văn Nhợi', CAST(N'1979-07-08' AS Date), N'Làm ruộng', N'0754037141', N'', N'781326580047', N'Nguyễn Thị Hương', CAST(N'1971-03-10' AS Date), N'Làm ruộng', N'0309642314', N'', N'271192312240', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (305, 327, N'Nguyễn Văn Nhã', CAST(N'1970-04-29' AS Date), N'Làm ruộng', N'0569805163', N'', N'820602864027', N'Phạm Thị Bắc', CAST(N'1974-07-15' AS Date), N'Công nhân', N'0955968159', N'', N'911796683073', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (306, 328, N'Trần Văn Tới', CAST(N'1971-11-03' AS Date), N'Làm ruộng', N'0793076050', N'', N'392684900760', N'Nguyễn Thị Tin', CAST(N'1973-08-15' AS Date), N'Làm ruộng', N'0225651514', N'', N'386730396747', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (307, 329, N'Nguyễn Văn Hiến', CAST(N'1985-04-28' AS Date), N'Làm ruộng', N'0380288535', N'', N'975456124544', N'Nguyễn Thị Hà', CAST(N'1976-02-09' AS Date), N'Công nhân', N'0759368783', N'', N'640438777208', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (308, 330, N'Nguyễn Đề Thám', CAST(N'1971-08-16' AS Date), N'Làm ruộng', N'0341325640', N'', N'823840451240', N'Nguyễn Thị Hạt', CAST(N'1987-08-09' AS Date), N'Làm ruộng', N'0923664975', N'', N'637535071372', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (309, 331, N'Đặng Hồng Huyễn', CAST(N'1988-06-25' AS Date), N'Làm ruộng', N'0408940643', N'', N'707343477010', N'Phạm Thị Hoa', CAST(N'1987-11-24' AS Date), N'Làm ruộng', N'0253182405', N'', N'790331882238', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (310, 332, N'Đinh Ngọc Cầm', CAST(N'1987-01-19' AS Date), N'Làm ruộng', N'0297666680', N'', N'914616382122', N'Tạ Thị Kiên', CAST(N'1979-11-30' AS Date), N'Làm ruộng', N'0941162097', N'', N'743434369564', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (311, 333, N'Phạm Văn Dương', CAST(N'1980-10-14' AS Date), N'Làm ruộng', N'0525525432', N'', N'888201719522', N'Trần Thị Oanh', CAST(N'1982-09-03' AS Date), N'Làm ruộng', N'0316377073', N'', N'718398147821', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (312, 334, N'Nguyễn Văn Tình', CAST(N'1990-02-19' AS Date), N'Làm ruộng', N'0553588581', N'', N'474983549118', N'Phạm Thị Ngọc', CAST(N'1986-04-11' AS Date), N'Làm ruộng', N'0967019987', N'', N'784443092346', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (313, 335, N'Đồng Kính Tôn', CAST(N'1984-09-15' AS Date), N'Công nhân', N'0753230923', N'', N'210774534940', N'Mai Thị Hà', CAST(N'1971-07-06' AS Date), N'Công nhân', N'0604977720', N'', N'332249420881', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (314, 336, N'Mai Văn Phong', CAST(N'1990-11-06' AS Date), N'Công nhân', N'0146558535', N'', N'958742153644', N'Nguyễn Thị Soi', CAST(N'1979-01-28' AS Date), N'Công nhân', N'0465195095', N'', N'594756948947', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (315, 337, N'Vũ Văn Tháp', CAST(N'1983-04-27' AS Date), N'Làm ruộng', N'0358561784', N'', N'817567569017', N'Trần Thị Khuyên', CAST(N'1980-09-19' AS Date), N'Làm ruộng', N'0231021243', N'', N'558204823732', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (316, 338, N'Trịnh Văn Thành', CAST(N'1985-05-04' AS Date), N'Làm ruộng', N'0440343785', N'', N'726656508445', N'Phạm Thị Hà', CAST(N'1980-03-12' AS Date), N'Làm ruộng', N'0587857985', N'', N'608699822425', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (317, 339, N'Đặng Ngọc Huy', CAST(N'1982-02-19' AS Date), N'Làm ruộng', N'0460372537', N'', N'461444133520', N'Nguyễn Thị Huệ', CAST(N'1979-05-06' AS Date), N'Làm ruộng', N'0273152202', N'', N'353448957204', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (318, 340, N'Trần Văn Doanh', CAST(N'1976-02-19' AS Date), N'Làm ruộng', N'0447912013', N'', N'114667928218', N'Vũ Thị Lan', CAST(N'1983-10-14' AS Date), N'Làm ruộng', N'0693606221', N'', N'474249160289', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (319, 341, N'Vũ Đình Hạnh', CAST(N'1981-07-27' AS Date), N'Công nhân', N'0511166387', N'', N'250986534357', N'Trần Thị Hạnh', CAST(N'1972-11-22' AS Date), N'Công nhân', N'0271984153', N'', N'415455728769', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (320, 342, N'Nguyễn Văn Tạo', CAST(N'1989-03-05' AS Date), N'Làm ruộng', N'0637534856', N'', N'247810745239', N'Nguyễn Thị Nhãn', CAST(N'1984-10-28' AS Date), N'Làm ruộng', N'0311380386', N'', N'974764490127', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (321, 343, N'Trần Văn Tứ', CAST(N'1985-10-10' AS Date), N'Làm ruộng', N'0828784674', N'', N'558748024702', N'Nguyễn Thị Hương', CAST(N'1987-06-09' AS Date), N'Làm ruộng', N'0888351601', N'', N'566802805662', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (322, 344, N'Vũ Đình Thuyên', CAST(N'1987-10-12' AS Date), N'Làm ruộng', N'0287719547', N'', N'465456449985', N'Nguyễn Thị Thế', CAST(N'1976-05-17' AS Date), N'Làm ruộng', N'0615708220', N'', N'624544060230', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (323, 345, N'Phạm Văn Thuân', CAST(N'1982-05-17' AS Date), N'Làm ruộng', N'0181611949', N'', N'195520383119', N'Vũ Thị Hà', CAST(N'1977-06-07' AS Date), N'Làm ruộng', N'0827577573', N'', N'518501693010', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (324, 346, N'Phạm Văn Phiên', CAST(N'1979-08-25' AS Date), N'Làm ruộng', N'0681593680', N'', N'348510956764', N'Nguyễn Thị Thanh', CAST(N'1978-07-14' AS Date), N'Làm ruộng', N'0603901886', N'', N'128834605216', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (325, 347, N'Vũ Ngọc Oanh', CAST(N'1976-04-26' AS Date), N'Làm ruộng', N'0155421680', N'', N'678742116689', N'Vũ Kim Phượng', CAST(N'1981-12-10' AS Date), N'Làm ruộng', N'0657881826', N'', N'439968436956', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (326, 348, N'Lâm Văn Đang', CAST(N'1975-02-05' AS Date), N'Làm ruộng', N'0556368100', N'', N'864189994335', N'Phạm Thị Sen', CAST(N'1971-05-16' AS Date), N'Giáo viên', N'0490833365', N'', N'223919236660', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (327, 349, N'Phạm Văn Đệ', CAST(N'1975-03-17' AS Date), N'Làm ruộng', N'0354987579', N'', N'548172289133', N'Đỗ Thị Diệp', CAST(N'1990-11-20' AS Date), N'Làm ruộng', N'0276054674', N'', N'688759952783', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (328, 350, N'Nguyễn Văn Đam', CAST(N'1990-05-10' AS Date), N'Làm ruộng', N'0148502922', N'', N'268997621536', N'Nguyễn Thị Ngọc', CAST(N'1981-08-17' AS Date), N'Làm ruộng', N'0761912965', N'', N'237908458709', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (329, 351, NULL, NULL, N'Unknown', NULL, NULL, NULL, N'Trần Thị Dung', CAST(N'1974-09-26' AS Date), N'Làm ruộng', N'0398521441', N'', N'278200978040', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (330, 352, NULL, NULL, N'Unknown', NULL, NULL, NULL, N'Trần Thị Duyên', CAST(N'1990-07-28' AS Date), N'Làm ruộng', N'0849895966', N'', N'437859308719', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (331, 353, N'Trịnh Ngọc Bỉnh', CAST(N'1975-09-30' AS Date), N'Làm ruộng', N'0948511296', N'', N'391199332475', N'Lương Thị Hồng Nhãn', CAST(N'1988-07-12' AS Date), N'Làm ruộng', N'0680922538', N'', N'186026650667', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (332, 354, N'Vương Quốc Triều', CAST(N'1977-05-15' AS Date), N'Bộ đội', N'0917858529', N'', N'793749499320', N'Trần Thị Lan', CAST(N'1978-09-24' AS Date), N'Kế toán', N'0726142382', N'', N'653847622871', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (333, 355, N'Nguyễn Văn Kỳ', CAST(N'1985-04-27' AS Date), N'Làm ruộng', N'0385738462', N'', N'296200317144', N'Phạm Thị Nhuần', CAST(N'1979-02-01' AS Date), N'Làm ruộng', N'0794334167', N'', N'732165712118', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (334, 356, N'Bùi Văn Huy', CAST(N'1986-06-04' AS Date), N'Làm ruộng', N'0198083770', N'', N'502293908596', N'Nguyễn Thị Hoa', CAST(N'1987-12-26' AS Date), N'Làm ruộng', N'0821954858', N'', N'495618426799', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (335, 357, N'Nguyễn Văn Đông', CAST(N'1974-03-31' AS Date), N'Làm ruộng', N'0881842523', N'', N'312424999475', N'Nguyễn Thị Len', CAST(N'1982-11-30' AS Date), N'Làm ruộng', N'0211254650', N'', N'478789335489', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (336, 358, N'Nguyễn Văn Duy', CAST(N'1970-04-22' AS Date), N'Làm ruộng', N'0661932229', N'', N'890546131134', N'Phạm Thị Bích', CAST(N'1983-09-27' AS Date), N'Làm ruộng', N'0420619678', N'', N'402439451217', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (337, 359, N'Nguyễn Văn Tuyên', CAST(N'1976-04-05' AS Date), N'Làm ruộng', N'0946315211', N'', N'649389523267', N'Phạm Thị Duyên', CAST(N'1970-05-11' AS Date), N'Làm ruộng', N'0779481381', N'', N'730750578641', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (338, 360, N'Đỗ Văn Khuyến', CAST(N'1976-06-28' AS Date), N'Làm ruộng', N'0262959969', N'', N'436988985538', N'Trần Thị Vui', CAST(N'1986-02-07' AS Date), N'Làm ruộng', N'0285775911', N'', N'291782772541', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (339, 361, N'Nguyễn Thành Chung', CAST(N'1970-10-03' AS Date), N'Làm ruộng', N'0172394591', N'', N'450551682710', N'Phạm Thị Thúy', CAST(N'1987-06-27' AS Date), N'Làm ruộng', N'0187003403', N'', N'139539462327', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (340, 362, N'Trần Văn Trung', CAST(N'1983-09-24' AS Date), N'Làm ruộng', N'0959734034', N'', N'874530339241', N'Đỗ Thị Phấn', CAST(N'1978-07-17' AS Date), N'Làm ruộng', N'0216737675', N'', N'897459578514', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (341, 363, N'Vũ Đình Khoa', CAST(N'1978-10-06' AS Date), N'Làm ruộng', N'0369535773', N'', N'627344125509', N'Vũ Thị Huệ', CAST(N'1975-10-04' AS Date), N'Làm ruộng', N'0646034616', N'', N'138878673315', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (342, 364, N'Trịnh Thanh Lãm', CAST(N'1989-04-03' AS Date), N'Làm ruộng', N'0368420350', N'', N'938403499126', N'Nguyễn Thị Quế', CAST(N'1989-11-20' AS Date), N'Làm ruộng', N'0446394526', N'', N'512833058834', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (343, 365, N'Đặng Văn Đoàn', CAST(N'1976-08-05' AS Date), N'Làm ruộng', N'0455647236', N'', N'565473181009', N'Đỗ Thị Nga', CAST(N'1984-01-07' AS Date), N'Làm ruộng', N'0649312597', N'', N'765084892511', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (344, 366, N'Nguyễn Văn Tuyền', CAST(N'1988-02-13' AS Date), N'Làm ruộng', N'0336074733', N'', N'397270727157', N'Hoàng Thị Hằng', CAST(N'1989-07-29' AS Date), N'Làm ruộng', N'0722314977', N'', N'779734420776', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (345, 367, N'Nguyễn văn Mạnh', CAST(N'1989-06-30' AS Date), N'Làm ruộng', N'0887217777', N'', N'881608313322', N'Nguyễn Thị Thêu', CAST(N'1984-07-26' AS Date), N'Làm ruộng', N'0876788061', N'', N'840320748090', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (346, 368, N'Trần Văn Kiên', CAST(N'1979-10-23' AS Date), N'Làm ruộng', N'0400262033', N'', N'333154881000', N'Vũ Thị Vóc', CAST(N'1986-03-05' AS Date), N'Làm ruộng', N'0136678135', N'', N'288707029819', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (347, 369, N'Nguyễn Văn Tài', CAST(N'1974-02-19' AS Date), N'Làm ruộng', N'0330459100', N'', N'333587199449', N'Trần Thị Hiền', CAST(N'1986-09-25' AS Date), N'Làm ruộng', N'0655829936', N'', N'521862393617', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (348, 370, N'Trần Thanh Tùng', CAST(N'1990-10-18' AS Date), N'Giáo viên', N'0125628399', N'', N'260824179649', N'Phạm Thị Hải', CAST(N'1988-01-01' AS Date), N'Giáo viên', N'0995658469', N'', N'881203675270', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (349, 371, N'Nguyễn Văn Trung', CAST(N'1978-04-23' AS Date), N'Làm ruộng', N'0801589006', N'', N'412261432409', N'Phạm Thị Vân', CAST(N'1984-08-29' AS Date), N'Làm ruộng', N'0303071063', N'', N'845507711172', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (350, 372, N'Nguyễn Trung Thuyên', CAST(N'1974-09-19' AS Date), N'Làm ruộng', N'0333051669', N'', N'102255094051', N'Nguyễn Thị Mơ', CAST(N'1988-03-12' AS Date), N'Làm ruộng', N'0443888700', N'', N'469361960887', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (351, 373, N'Phan Văn Thực', CAST(N'1973-12-26' AS Date), N'Làm ruộng', N'0829380232', N'', N'768107849359', N'Nguyễn Thị Xoan', CAST(N'1986-07-23' AS Date), N'Làm ruộng', N'0601019531', N'', N'516250139474', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (352, 374, N'Nguyễn Trọng Đức', CAST(N'1983-01-15' AS Date), N'Làm ruộng', N'0661994886', N'', N'856798362731', N'Phạm Thị Hảo', CAST(N'1984-08-20' AS Date), N'Làm ruộng', N'0584625816', N'', N'413982820510', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (353, 375, N'Nguyễn Văn Thạnh', CAST(N'1972-08-25' AS Date), N'Làm ruộng', N'0518099898', N'', N'350730484724', N'Phạm Thị Duyên', CAST(N'1987-12-24' AS Date), N'Làm ruộng', N'0787560623', N'', N'324343127012', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (354, 376, N'Nguyễn Văn Trường', CAST(N'1979-01-17' AS Date), N'Làm ruộng', N'0131687939', N'', N'732673132419', N'Phạm Thị Thu', CAST(N'1974-08-03' AS Date), N'Làm ruộng', N'0561245214', N'', N'826337158679', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (355, 377, N'Nguyễn Văn Dũng', CAST(N'1982-11-10' AS Date), N'Làm ruộng', N'0313964587', N'', N'464378148317', N'Vũ Thị Len', CAST(N'1970-04-23' AS Date), N'Làm ruộng', N'0261474400', N'', N'589880210161', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (356, 378, N'Vũ Anh Thơi', CAST(N'1975-08-12' AS Date), N'Làm ruộng', N'0391762328', N'', N'830414223670', N'Trần Thị Hoa', CAST(N'1977-12-06' AS Date), N'Làm ruộng', N'0890687966', N'', N'584425616264', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (357, 379, N'Nguyễn Văn Du', CAST(N'1981-08-21' AS Date), N'Làm ruộng', N'0703236049', N'', N'767602628469', N'Nguyễn Thị Phúc', CAST(N'1972-09-25' AS Date), N'Làm ruộng', N'0432093900', N'', N'664015716314', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (358, 380, N'Tạ Minh Thục', CAST(N'1973-09-29' AS Date), N'Làm ruộng', N'0777964055', N'', N'493897521495', N'Trần Thị Hiên', CAST(N'1973-03-27' AS Date), N'Làm ruộng', N'0164486205', N'', N'179316461086', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (359, 381, N'Nguyễn Văn Tiên', CAST(N'1986-12-05' AS Date), N'Làm ruộng', N'0395082527', N'', N'170182520151', N'Nguyễn Thị Cẩm', CAST(N'1983-08-11' AS Date), N'Làm ruộng', N'0666228967', N'', N'739951092004', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (360, 382, N'Vũ Đình Cẩm', CAST(N'1976-09-08' AS Date), N'Làm ruộng', N'0613569641', N'', N'469568490982', N'Trần Thị Lựu', CAST(N'1973-02-12' AS Date), N'Làm ruộng', N'0457991647', N'', N'332186603546', NULL, NULL, N'Unknown', NULL, NULL, NULL)
GO
INSERT [dbo].[Parents] ([ParentID], [UserID], [FullNameFather], [YearOfBirthFather], [OccupationFather], [PhoneNumberFather], [EmailFather], [IdcardNumberFather], [FullNameMother], [YearOfBirthMother], [OccupationMother], [PhoneNumberMother], [EmailMother], [IdcardNumberMother], [FullNameGuardian], [YearOfBirthGuardian], [OccupationGuardian], [PhoneNumberGuardian], [EmailGuardian], [IdcardNumberGuardian]) VALUES (361, 383, N'Vũ Văn Duy', CAST(N'1977-09-10' AS Date), N'Làm ruộng', N'0463733025', N'', N'734992760415', N'Bùi Thị Hà', CAST(N'1978-10-06' AS Date), N'Làm ruộng', N'0667330715', NULL, N'405437678095', N'', NULL, N'', N'', N'', NULL)
GO
SET IDENTITY_INSERT [dbo].[Parents] OFF
GO
SET IDENTITY_INSERT [dbo].[Roles] ON 
GO
INSERT [dbo].[Roles] ([RoleID], [RoleName]) VALUES (1, N'Cán bộ văn thư')
GO
INSERT [dbo].[Roles] ([RoleID], [RoleName]) VALUES (5, N'Giáo viên')
GO
INSERT [dbo].[Roles] ([RoleID], [RoleName]) VALUES (6, N'Hiệu phó')
GO
INSERT [dbo].[Roles] ([RoleID], [RoleName]) VALUES (4, N'Hiệu trưởng')
GO
INSERT [dbo].[Roles] ([RoleID], [RoleName]) VALUES (3, N'Phụ huynh')
GO
INSERT [dbo].[Roles] ([RoleID], [RoleName]) VALUES (2, N'Trưởng bộ môn')
GO
SET IDENTITY_INSERT [dbo].[Roles] OFF
GO
SET IDENTITY_INSERT [dbo].[Semesters] ON 
GO
INSERT [dbo].[Semesters] ([SemesterID], [AcademicYearID], [SemesterName], [StartDate], [EndDate]) VALUES (1, 1, N'Học kỳ 1', CAST(N'2026-09-05' AS Date), CAST(N'2026-12-26' AS Date))
GO
INSERT [dbo].[Semesters] ([SemesterID], [AcademicYearID], [SemesterName], [StartDate], [EndDate]) VALUES (2, 1, N'Học kỳ 2', CAST(N'2027-04-01' AS Date), CAST(N'2027-05-03' AS Date))
GO
INSERT [dbo].[Semesters] ([SemesterID], [AcademicYearID], [SemesterName], [StartDate], [EndDate]) VALUES (3, 2, N'Học kỳ 1', CAST(N'2023-08-01' AS Date), CAST(N'2023-12-31' AS Date))
GO
INSERT [dbo].[Semesters] ([SemesterID], [AcademicYearID], [SemesterName], [StartDate], [EndDate]) VALUES (4, 2, N'Học kỳ 2', CAST(N'2024-01-01' AS Date), CAST(N'2024-05-31' AS Date))
GO
INSERT [dbo].[Semesters] ([SemesterID], [AcademicYearID], [SemesterName], [StartDate], [EndDate]) VALUES (5, 3, N'Học kỳ 1', CAST(N'2025-08-01' AS Date), CAST(N'2026-01-11' AS Date))
GO
INSERT [dbo].[Semesters] ([SemesterID], [AcademicYearID], [SemesterName], [StartDate], [EndDate]) VALUES (6, 3, N'Học kỳ 2', CAST(N'2026-01-12' AS Date), CAST(N'2026-05-25' AS Date))
GO
INSERT [dbo].[Semesters] ([SemesterID], [AcademicYearID], [SemesterName], [StartDate], [EndDate]) VALUES (7, 7, N'Học kỳ 1', CAST(N'2024-09-05' AS Date), CAST(N'2025-01-17' AS Date))
GO
INSERT [dbo].[Semesters] ([SemesterID], [AcademicYearID], [SemesterName], [StartDate], [EndDate]) VALUES (8, 7, N'Học kỳ 2', CAST(N'2025-01-20' AS Date), CAST(N'2025-05-30' AS Date))
GO
SET IDENTITY_INSERT [dbo].[Semesters] OFF
GO
SET IDENTITY_INSERT [dbo].[StudentClasses] ON 
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (1, 1, 13, 7)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (2, 2, 13, 7)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (3, 3, 13, 7)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (4, 4, 13, 7)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (5, 5, 13, 7)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (6, 6, 13, 7)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (7, 7, 13, 7)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (8, 8, 13, 7)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (9, 9, 13, 7)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (10, 10, 13, 7)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (11, 11, 13, 7)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (12, 12, 13, 7)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (13, 13, 13, 7)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (14, 14, 13, 7)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (15, 15, 13, 7)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (16, 16, 13, 7)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (17, 17, 13, 7)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (18, 18, 13, 7)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (19, 19, 13, 7)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (20, 20, 13, 7)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (21, 21, 13, 7)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (22, 22, 13, 7)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (23, 23, 13, 7)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (24, 24, 13, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (25, 25, 13, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (26, 26, 13, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (27, 27, 13, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (28, 28, 13, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (29, 29, 13, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (30, 30, 13, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (31, 31, 13, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (32, 32, 13, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (33, 33, 13, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (34, 34, 13, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (35, 35, 13, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (36, 36, 13, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (37, 37, 13, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (38, 38, 13, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (39, 39, 13, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (40, 40, 13, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (41, 41, 13, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (42, 42, 13, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (43, 43, 13, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (44, 44, 13, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (45, 45, 13, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (46, 46, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (47, 47, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (48, 48, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (49, 49, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (50, 50, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (51, 51, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (52, 52, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (53, 53, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (54, 54, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (55, 55, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (56, 56, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (57, 57, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (58, 58, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (59, 59, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (60, 60, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (61, 61, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (62, 62, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (63, 63, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (64, 64, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (65, 65, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (66, 66, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (67, 67, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (68, 68, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (69, 69, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (70, 70, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (71, 71, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (72, 72, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (73, 73, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (74, 74, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (75, 75, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (76, 76, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (77, 77, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (78, 78, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (79, 79, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (80, 80, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (81, 81, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (82, 82, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (83, 83, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (84, 84, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (85, 85, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (86, 86, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (87, 87, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (88, 88, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (89, 89, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (90, 90, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (91, 91, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (92, 92, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (93, 93, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (94, 94, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (95, 95, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (96, 96, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (97, 97, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (98, 98, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (99, 99, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (100, 100, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (101, 101, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (102, 102, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (103, 103, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (104, 104, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (105, 105, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (106, 106, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (107, 107, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (108, 108, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (109, 109, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (110, 110, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (111, 111, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (112, 112, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (113, 113, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (114, 114, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (115, 115, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (116, 116, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (117, 117, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (118, 118, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (119, 119, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (120, 120, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (121, 121, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (122, 122, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (123, 123, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (124, 124, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (125, 125, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (126, 126, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (127, 127, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (128, 128, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (129, 129, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (130, 130, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (131, 131, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (132, 132, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (133, 133, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (134, 134, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (135, 135, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (136, 136, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (137, 137, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (138, 138, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (139, 139, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (140, 140, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (141, 141, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (142, 142, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (143, 143, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (144, 144, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (145, 145, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (146, 146, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (147, 147, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (148, 148, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (149, 149, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (150, 150, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (151, 151, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (152, 152, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (153, 153, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (154, 154, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (155, 155, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (156, 156, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (157, 157, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (158, 158, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (159, 159, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (160, 160, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (161, 161, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (162, 162, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (163, 163, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (164, 164, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (165, 165, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (166, 166, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (167, 167, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (168, 168, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (169, 169, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (170, 170, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (171, 171, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (172, 172, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (173, 173, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (174, 174, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (175, 175, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (176, 176, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (177, 177, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (178, 178, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (179, 179, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (180, 180, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (181, 181, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (182, 182, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (183, 183, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (184, 184, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (185, 185, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (186, 186, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (187, 187, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (188, 188, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (189, 189, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (190, 190, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (191, 191, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (192, 192, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (193, 193, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (194, 194, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (195, 195, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (196, 196, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (197, 197, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (198, 198, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (199, 199, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (200, 200, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (201, 201, 17, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (202, 202, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (203, 203, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (204, 204, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (205, 205, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (206, 206, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (207, 207, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (208, 208, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (209, 209, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (210, 210, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (211, 211, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (212, 212, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (213, 213, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (214, 214, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (215, 215, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (216, 216, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (217, 217, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (218, 218, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (219, 219, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (220, 220, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (221, 221, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (222, 222, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (223, 223, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (224, 224, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (225, 225, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (226, 226, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (227, 227, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (228, 228, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (229, 229, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (230, 230, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (231, 231, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (232, 232, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (233, 233, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (234, 234, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (235, 235, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (236, 236, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (237, 237, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (238, 238, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (239, 239, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (240, 240, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (241, 241, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (242, 242, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (243, 243, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (244, 244, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (245, 245, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (246, 246, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (247, 247, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (248, 248, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (249, 249, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (250, 250, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (251, 251, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (252, 252, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (253, 253, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (254, 254, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (255, 255, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (256, 256, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (257, 257, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (258, 258, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (259, 259, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (260, 260, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (261, 261, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (262, 262, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (263, 263, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (264, 264, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (265, 265, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (266, 266, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (267, 267, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (268, 268, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (269, 269, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (270, 270, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (271, 271, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (272, 272, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (273, 273, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (274, 274, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (275, 275, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (276, 276, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (277, 277, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (278, 278, 19, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (279, 279, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (280, 280, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (281, 281, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (282, 282, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (283, 283, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (284, 284, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (285, 285, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (286, 286, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (287, 287, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (288, 288, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (289, 289, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (290, 290, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (291, 291, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (292, 292, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (293, 293, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (294, 294, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (295, 295, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (296, 296, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (297, 297, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (298, 298, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (299, 299, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (300, 300, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (301, 301, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (302, 302, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (303, 303, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (304, 304, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (305, 305, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (306, 306, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (307, 307, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (308, 308, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (309, 309, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (310, 310, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (311, 311, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (312, 312, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (313, 313, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (314, 314, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (315, 315, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (316, 316, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (317, 317, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (318, 318, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (319, 319, 20, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (320, 320, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (321, 321, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (322, 322, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (323, 323, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (324, 324, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (325, 325, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (326, 326, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (327, 327, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (328, 328, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (329, 329, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (330, 330, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (331, 331, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (332, 332, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (333, 333, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (334, 334, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (335, 335, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (336, 336, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (337, 337, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (338, 338, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (339, 339, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (340, 340, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (341, 341, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (342, 342, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (343, 343, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (344, 344, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (345, 345, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (346, 346, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (347, 347, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (348, 348, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (349, 349, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (350, 350, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (351, 351, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (352, 352, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (353, 353, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (354, 354, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (355, 355, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (356, 356, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (357, 357, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (358, 358, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (359, 359, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (360, 360, 21, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (361, 361, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (362, 362, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (363, 363, 18, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (364, 364, 14, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (365, 365, 16, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (366, 366, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (367, 367, 15, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (368, 368, 13, 1)
GO
INSERT [dbo].[StudentClasses] ([ID], [StudentID], [ClassID], [AcademicYearID]) VALUES (369, 369, 16, 1)
GO
SET IDENTITY_INSERT [dbo].[StudentClasses] OFF
GO
SET IDENTITY_INSERT [dbo].[Students] ON 
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (1, N'Vũ Minh An', CAST(N'2013-10-28' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Tuy Đức, tỉnh Đắk Nông', N'Không', 0, N'036213020694', N'Đang học', 1)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (2, N'Nguyễn Việt Anh', CAST(N'2013-03-10' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036213010642', N'Đang học', 2)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (3, N'Trần Thị Ngọc Anh', CAST(N'2013-09-08' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, xã Hải Giang, huyện Hải Hâu, tỉnh Nam Định', N'Trạm y tế xã Nghĩa Sơn, huyện Nghĩa Hưng, tỉnh Nam Định', N'Không', 0, N'036313016139', N'Đang học', 3)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (4, N'Phạm Thị Vân Anh', CAST(N'2013-06-05' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036313006570', N'Đang học', 4)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (5, N'Nguyễn Hồng Ánh', CAST(N'2013-11-09' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thuận, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036313002112', N'Đang học', 5)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (6, N'Nguyễn Gia Bảo', CAST(N'2013-01-23' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036213017353', N'Đang học', 6)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (7, N'Nguyễn Quốc Bảo', CAST(N'2013-11-15' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 6, xã Hải Ninh, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Ninh, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036213019040', N'Đang học', 7)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (8, N'Nguyễn Thanh Bình', CAST(N'2013-03-18' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, xã Hải Giang, huyện Hải Hâu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036213017392', N'Đang học', 8)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (9, N'Vũ Thị Bảo Châm', CAST(N'2013-01-02' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036313001328', N'Đang học', 9)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (10, N'Lê Nguyễn Quỳnh Chi', CAST(N'2013-03-25' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tinh Nam Định', N'Bệnh viện đa khoa Đồng Nai', N'Không', 0, N'075313000814', N'Đang học', 10)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (11, N'Phạm Ngọc Diệp', CAST(N'2013-01-09' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Khoa phụ sản Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036313013495', N'Đang học', 11)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (12, N'Nguyễn Tuấn Dũng', CAST(N'2013-01-09' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tinh Nam Định', N'Bệnh viện đa khoa huyện Chơn Thành tỉnh Bình Phước', N'Không', 0, N'036213008299', N'Đang học', 12)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (13, N'Nguyễn Tiến Đạt', CAST(N'2013-01-17' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thuận, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036213010645', N'Đang học', 13)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (14, N'Nguyễn Minh Đạt', CAST(N'2013-08-28' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Hà, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036213008613', N'Đang học', 14)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (15, N'Vương Trần Gia Hân', CAST(N'2013-06-04' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036313019348', N'Đang học', 15)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (16, N'Nguyễn Thị Kim Huệ', CAST(N'2013-07-29' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Đông, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Phú, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036313005402', N'Đang học', 16)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (17, N'Nguyễn Huy Hùng', CAST(N'2013-01-06' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036213003920', N'Đang học', 17)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (18, N'Trần Quốc Hưng', CAST(N'2013-08-23' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036213002849', N'Đang học', 18)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (19, N'Nguyễn Đăng Khoa', CAST(N'2013-02-07' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, xã Hải Giang, huyện Hải Hâu, tỉnh Nam Định', N'Bệnh viện huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036213018393', N'Đang học', 19)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (20, N'Lê Tuấn Kiệt', CAST(N'2013-01-25' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036213016180', N'Đang học', 20)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (21, N'Vũ Hà Linh', CAST(N'2013-02-02' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thuận, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Phú, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036313016681', N'Đang học', 21)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (22, N'Trần Thị Mai Lụa', CAST(N'2013-07-07' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036313013792', N'Đang học', 22)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (23, N'Nguyễn Thảo Ly', CAST(N'2013-12-12' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036313015563', N'Đang học', 23)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (24, N'Bùi Ngọc Mai', CAST(N'2013-02-08' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036313004242', N'Đang học', 24)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (25, N'Nguyễn Minh Minh', CAST(N'2013-02-01' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Hà, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Ninh, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036313013016', N'Đang học', 25)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (26, N'Nguyễn Bùi Linh Ngân', CAST(N'2013-08-17' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Đông, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036313019280', N'Đang học', 26)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (27, N'Nguyễn Thị Bảo Ngọc', CAST(N'2013-10-20' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036313017266', N'Đang học', 27)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (28, N'Nguyễn Khánh Ngọc', CAST(N'2013-09-07' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Đông, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Hùng Vương - Thành phố Hồ Chí Minh', N'Công giáo', 0, N'036313005665', N'Đang học', 28)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (29, N'Trần Thảo Nguyên', CAST(N'2013-09-07' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Phú, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036313010032', N'Đang học', 29)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (30, N'Phạm Hồng Sơn', CAST(N'2013-11-17' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Ninh, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036213020034', N'Đang học', 30)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (31, N'Đới Minh Tân', CAST(N'2013-04-24' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036213016797', N'Đang học', 31)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (32, N'Bùi Trọng Tấn', CAST(N'2013-10-12' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036213007741', N'Đang học', 32)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (33, N'Nguyễn Phương Thảo', CAST(N'2013-10-26' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036313012550', N'Đang học', 33)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (34, N'Phạm Hoài Thương', CAST(N'2013-09-04' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tinh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036313007693', N'Đang học', 34)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (35, N'Trần Ngọc Thưởng', CAST(N'2013-07-31' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa tỉnh Bình Dương', N'Công giáo', 0, N'036213001801', N'Đang học', 35)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (36, N'Vũ Minh Tới', CAST(N'2013-11-24' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036213007725', N'Đang học', 36)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (37, N'Phạm Thị Ngọc Trâm', CAST(N'2013-01-01' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Hà, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Thạch Thành, tỉnh Thanh Hóa', N'Công giáo', 0, N'036313011554', N'Đang học', 37)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (38, N'Bùi Phú Trọng', CAST(N'2013-05-06' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Ninh, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036213013477', N'Đang học', 38)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (39, N'Phạm Tuấn Tú', CAST(N'2013-08-10' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 9, xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Trực Hùng, huyện Trực Ninh, tỉnh Nam Định', N'Công giáo', 0, N'036213009527', N'Đang học', 39)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (40, N'Nguyễn Tuấn Tú', CAST(N'2012-08-04' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036212009836', N'Đang học', 40)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (41, N'Vũ Văn Tuệ', CAST(N'2013-09-13' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Đông, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036213010328', N'Đang học', 41)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (42, N'Trần Nguyễn Phương Uyên', CAST(N'2013-11-09' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036313004197', N'Đang học', 42)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (43, N'Phan Thị Phương Uyên', CAST(N'2013-10-27' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Đông, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036313007790', N'Đang học', 43)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (44, N'Vũ Thị Ngọc Vy', CAST(N'2013-01-09' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036313013430', N'Đang học', 44)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (45, N'Vũ Thị Hải Yến', CAST(N'2013-03-26' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036313013213', N'Đang học', 45)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (46, N'Nguyễn Trần Xuân An', CAST(N'2013-06-21' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 9, xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036213005695', N'Đang học', 46)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (47, N'Vũ Minh Anh', CAST(N'2013-06-26' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036313020475', N'Đang học', 47)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (48, N'Lưu Tuấn Anh', CAST(N'2013-08-24' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Hà, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036213003347', N'Đang học', 48)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (49, N'Trịnh Thị Ngọc Ánh', CAST(N'2013-05-01' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 9, xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036313003693', N'Đang học', 49)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (50, N'Đào Hoàng Bách', CAST(N'2013-09-08' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 9, xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036213016642', N'Đang học', 50)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (51, N'Nguyễn Hoàng Bảo', CAST(N'2013-06-08' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Đông, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036213013711', N'Đang học', 51)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (52, N'Nguyễn Thái Bảo', CAST(N'2013-10-11' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 9, xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036213020338', N'Đang học', 52)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (53, N'Phạm Trần Quỳnh Chi', CAST(N'2013-07-07' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thuận, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036313012177', N'Đang học', 53)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (54, N'Trần Thùy Chi', CAST(N'2013-11-29' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 9, xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa Hồng Đức', N'Không', 0, N'031313011062', N'Đang học', 54)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (55, N'Nguyễn Tiến Dũng', CAST(N'2013-06-09' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036213010042', N'Đang học', 55)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (56, N'Phan Thái Thùy Linh Đan', CAST(N'2013-12-04' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036313017192', N'Đang học', 56)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (57, N'Nguyễn Thị Linh Đan', CAST(N'2013-11-16' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036313005032', N'Đang học', 57)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (58, N'Nguyễn Ngọc Linh Đan', CAST(N'2013-07-30' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036313014395', N'Đang học', 58)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (59, N'Mai Hắc Đế', CAST(N'2013-02-20' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036213015116', N'Đang học', 59)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (60, N'Nguyễn Trường Giang', CAST(N'2013-10-21' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Nhà hộ sinh Thiện Phước - Quận Tân Phú -TP Hồ Chí Minh', N'Không', 0, N'036213016116', N'Đang học', 60)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (61, N'Vũ Đình Hanh', CAST(N'2013-06-09' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036213005002', N'Đang học', 61)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (62, N'Nguyễn Ngọc Gia Hân', CAST(N'2013-02-01' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036313011850', N'Đang học', 62)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (63, N'Nguyễn Ngọc Hân', CAST(N'2013-06-08' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 9, xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036313012861', N'Đang học', 63)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (64, N'Bùi Thu Hiền', CAST(N'2013-04-04' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Đông, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Từ Dũ, thành phố Hồ Chí Minh', N'Không', 0, N'038313013545', N'Đang học', 64)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (65, N'Phạm Trung Hiếu', CAST(N'2013-03-04' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa Khoa Hải Hậu', N'Không', 0, N'036213006986', N'Đang học', 65)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (66, N'Lưu Thị Kim Huệ', CAST(N'2013-10-19' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036313020190', N'Đang học', 66)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (67, N'Nguyễn Thị Huệ', CAST(N'2013-01-07' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036313003871', N'Đang học', 67)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (68, N'Trần Quốc Huy', CAST(N'2013-12-17' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036213016621', N'Đang học', 68)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (69, N'Nguyễn Quốc Huy', CAST(N'2013-07-14' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036213009200', N'Đang học', 69)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (70, N'Lâm Tuấn Huy', CAST(N'2013-09-10' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036213011435', N'Đang học', 70)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (71, N'Đỗ Duy Hưng', CAST(N'2013-09-23' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036213012428', N'Đang học', 71)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (72, N'Nguyễn Nguyên Khang', CAST(N'2013-06-11' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Hà, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036213016427', N'Đang học', 72)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (73, N'Vũ Nguyên Khang', CAST(N'2013-01-01' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036213013534', N'Đang học', 73)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (74, N'Nguyễn Trung Kiên', CAST(N'2013-02-20' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036213008575', N'Đang học', 74)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (75, N'Vũ Thị Loan', CAST(N'2013-05-05' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036313017686', N'Đang học', 75)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (76, N'Vũ Trần Quang Minh', CAST(N'2013-08-04' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Hà, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Phú, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036213012535', N'Đang học', 76)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (77, N'Đặng Bảo Ngân', CAST(N'2013-08-17' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thuận, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036313013029', N'Đang học', 77)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (78, N'Hoàng Khánh Nhi', CAST(N'2013-11-28' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 7, xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036313018393', N'Đang học', 78)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (79, N'Đồng An Nhiên', CAST(N'2013-08-14' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Hà, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036313020202', N'Đang học', 79)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (80, N'Phạm Đại Phát', CAST(N'2013-07-06' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036213010574', N'Đang học', 80)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (81, N'Nguyễn Minh Phúc', CAST(N'2013-08-17' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Phú, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036213009234', N'Đang học', 81)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (82, N'Nguyễn Thị Khánh Quỳnh', CAST(N'2013-02-04' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Hà, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036313001499', N'Đang học', 82)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (83, N'Nguyễn Như Quỳnh', CAST(N'2013-11-07' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036313020224', N'Đang học', 83)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (84, N'Hoàng Minh Sang', CAST(N'2013-02-28' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 9. xã Hải Sơn, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036213003267', N'Đang học', 84)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (85, N'Lâm Hoài Thu', CAST(N'2013-10-06' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036313010577', N'Đang học', 85)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (86, N'Nguyễn Thị Quỳnh Thư', CAST(N'2013-06-03' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036313019595', N'Đang học', 86)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (87, N'Bùi Nguyễn Anh Thư', CAST(N'2013-07-07' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thuận, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036313001491', N'Đang học', 87)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (88, N'Nguyễn Mạnh Tới', CAST(N'2013-06-01' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036213016418', N'Đang học', 88)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (89, N'Phạm Đỗ Hiền Trang', CAST(N'2013-11-02' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036313018577', N'Đang học', 89)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (90, N'Nguyễn Thị Phương Trinh', CAST(N'2013-02-09' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Cư Jút tỉnh Đắk Nông', N'Công giáo', 0, N'036313011468', N'Đang học', 90)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (91, N'Nguyễn Văn Tú', CAST(N'2013-09-28' AS Date), N'Nam', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036213009581', N'Đang học', 91)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (92, N'Vũ Tường Vy', CAST(N'2013-06-02' AS Date), N'Nữ', CAST(N'2024-08-28' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 7 xã Hải Ninh, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Ninh, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036313002911', N'Đang học', 92)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (93, N'Trần Hoàng Phương Anh', CAST(N'2012-04-18' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036312007777', N'Đang học', 93)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (94, N'Nguyễn Duy Ánh', CAST(N'2012-06-06' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036212005735', N'Đang học', 94)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (95, N'Trần Ngọc Diệp', CAST(N'2012-01-01' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036312014561', N'Đang học', 95)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (96, N'Nguyễn Thùy Dung', CAST(N'2012-05-22' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036312018036', N'Đang học', 96)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (97, N'Nguyễn Mỹ Duyên', CAST(N'2012-01-07' AS Date), N'Nữ', CAST(N'2024-08-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036312011664', N'Đang học', 97)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (98, N'Phạm Thành Đạt', CAST(N'2012-01-01' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036212022582', N'Đang học', 98)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (99, N'Nguyễn Thanh Hải', CAST(N'2012-02-10' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thuận, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Lý, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036212016852', N'Đang học', 99)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (100, N'Vũ Mạnh Hải', CAST(N'2012-01-01' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Đông, Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036212004146', N'Đang học', 100)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (101, N'Nguyễn Gia Hưng', CAST(N'2012-02-02' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036212015646', N'Đang học', 101)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (102, N'Nguyễn Mạnh Long', CAST(N'2012-10-24' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Hà, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036212013621', N'Đang học', 102)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (103, N'Nguyễn Khánh Ly', CAST(N'2012-01-10' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thuận, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036312011009', N'Đang học', 103)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (104, N'Nguyễn Tiến Mạnh', CAST(N'2012-11-04' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Hà, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Ninh, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036212015666', N'Đang học', 104)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (105, N'Nguyễn Phạm Trà My', CAST(N'2012-08-27' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Hà, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036312012513', N'Đang học', 105)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (106, N'Nguyễn Hoàng Nam', CAST(N'2011-06-14' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện khu vực Ayun Pa tỉnh Gia Lai', N'Không', 0, N'036211020047', N'Đang học', 106)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (107, N'Nguyễn Thị Thu Ngân', CAST(N'2012-06-23' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036312010109', N'Đang học', 107)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (108, N'Vũ Thị Kim Ngân', CAST(N'2012-01-02' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036312022682', N'Đang học', 108)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (109, N'Mai Thị Ánh Ngọc', CAST(N'2012-11-04' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036312013293', N'Đang học', 109)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (110, N'Trần Đình Quốc Nhật', CAST(N'2012-02-06' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'046212007949', N'Đang học', 110)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (111, N'Đỗ Thị Uyển Nhi', CAST(N'2012-12-18' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Phú, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036312016462', N'Đang học', 111)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (112, N'Vũ Thị Quỳnh Như', CAST(N'2012-10-19' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036312005007', N'Đang học', 112)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (113, N'Trần Nhật Phúc', CAST(N'2012-03-16' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 6C, xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036212014594', N'Đang học', 113)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (114, N'Mai Hà Phương', CAST(N'2012-11-10' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Khoa phụ sản Bệnh viện đa khoa Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036312002223', N'Đang học', 114)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (115, N'Vũ Thị Bích Phượng', CAST(N'2012-05-13' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036312013464', N'Đang học', 115)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (116, N'Vũ Trường Sơn', CAST(N'2012-09-14' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thuận, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Phú, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036212011173', N'Đang học', 116)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (117, N'Đỗ Tiến Tài', CAST(N'2012-01-13' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036212012224', N'Đang học', 117)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (118, N'Trần Văn Tân', CAST(N'2012-04-21' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 9B, xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036212010398', N'Đang học', 118)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (119, N'Trần Đức Thịnh', CAST(N'2012-12-03' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 1,xã Hải Ninh, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Ninh, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036212022879', N'Đang học', 119)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (120, N'Nguyễn Thị Thanh Thúy', CAST(N'2012-09-06' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036312015848', N'Đang học', 120)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (121, N'Nguyễn Anh Thư', CAST(N'2012-07-18' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 6, xã Hải Ninh, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Ninh, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036312011708', N'Đang học', 121)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (122, N'Nguyễn Thị Hoài Thương', CAST(N'2012-10-08' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Hà, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Từ Dũ Thành phố Hồ Chí Minh', N'Không', 0, N'036312020317', N'Đang học', 122)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (123, N'Trần Văn Tốn', CAST(N'2012-09-19' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036212017402', N'Đang học', 123)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (124, N'Đỗ Thị Thanh Trà', CAST(N'2012-09-29' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Hà, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036312003759', N'Đang học', 124)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (125, N'Nguyễn Văn Trọng', CAST(N'2010-05-26' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036210015766', N'Đang học', 125)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (126, N'Vũ Hoàng Thanh Trúc', CAST(N'2012-11-01' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036312004310', N'Đang học', 126)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (127, N'Vũ Minh Tuấn', CAST(N'2012-07-11' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thuận, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036212006111', N'Đang học', 127)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (128, N'Vũ Tuấn Tú', CAST(N'2012-06-03' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036212017801', N'Đang học', 128)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (129, N'Nguyễn Quốc An', CAST(N'2012-01-15' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036212019660', N'Đang học', 129)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (130, N'Nguyễn Thị Ngọc Anh', CAST(N'2012-01-02' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036312022753', N'Đang học', 130)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (131, N'Trần Thị Kim Anh', CAST(N'2012-10-19' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thuận, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036312021251', N'Đang học', 131)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (132, N'Vũ Ngọc Diệu Anh', CAST(N'2012-07-23' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện tỉnh Đắk Nông', N'Công giáo', 0, N'036312002441', N'Đang học', 132)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (133, N'Vũ Thị Lan Anh', CAST(N'2012-11-24' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036312010754', N'Đang học', 133)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (134, N'Bùi Ngọc Ánh', CAST(N'2012-06-09' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Thành phố Mát- xcơ- va. Liên Bang Nga', N'Công giáo', 0, N'036312018039', N'Đang học', 134)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (135, N'Nguyễn Thị Ngọc Ánh', CAST(N'2012-09-03' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Phú, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036312005907', N'Đang học', 135)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (136, N'Vũ Hoàng Bách', CAST(N'2012-11-04' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Nghĩa Lạc, huyện Nghĩa Hưng, tỉnh Nam Định', N'Công giáo', 0, N'036212000218', N'Đang học', 136)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (137, N'Trần Bảo Châu', CAST(N'2012-12-13' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu ,tỉnh Nam Định', N'Công giáo', 0, N'095312009997', N'Đang học', 137)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (138, N'Phạm Trần Chí Công', CAST(N'2012-01-24' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036212004227', N'Đang học', 138)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (139, N'Trần Ngọc Diệp', CAST(N'2012-12-09' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036312009622', N'Đang học', 139)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (140, N'Nguyễn Mạnh Dũng', CAST(N'2012-11-21' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 6C, xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036212017946', N'Đang học', 140)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (141, N'Phan Thành Đạt', CAST(N'2012-02-07' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036212020351', N'Đang học', 141)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (142, N'Nguyễn Thị Hà', CAST(N'2012-08-24' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Đông, Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036312007114', N'Đang học', 142)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (143, N'Lương Hòa Hiệp', CAST(N'2012-10-07' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036212002198', N'Đang học', 143)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (144, N'Nguyễn Quang Huy', CAST(N'2012-09-15' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 9, xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036212000331', N'Đang học', 144)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (145, N'Vũ Quốc Hưng', CAST(N'2012-01-07' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036212013986', N'Đang học', 145)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (146, N'Phạm Gia Khánh', CAST(N'2012-01-09' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036212007884', N'Đang học', 146)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (147, N'Lương Thị Phương Linh', CAST(N'2012-01-16' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036312009193', N'Đang học', 147)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (148, N'Nguyễn Hải Long', CAST(N'2012-09-20' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Hà, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036212002390', N'Đang học', 148)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (149, N'Vũ Tiến Long', CAST(N'2012-01-12' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036212005035', N'Đang học', 149)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (150, N'Nguyễn Nhật Minh', CAST(N'2012-11-22' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 6C, xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện phụ sản Nam Định', N'Công giáo', 0, N'036212009939', N'Đang học', 150)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (151, N'Trịnh Thị My', CAST(N'2012-07-17' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'xóm 9B ,xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa tỉnh Bình Dương', N'Không', 0, N'036312023566', N'Đang học', 151)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (152, N'Vũ Thị Trà My', CAST(N'2012-04-30' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036312009968', N'Đang học', 152)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (153, N'Nguyễn Văn Phong', CAST(N'2012-07-25' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036212018812', N'Đang học', 153)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (154, N'Phan Trọng Phúc', CAST(N'2012-12-14' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036212018679', N'Đang học', 154)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (155, N'Nguyễn Minh Quân', CAST(N'2012-09-13' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036212006237', N'Đang học', 155)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (156, N'Nguyễn Thị Diễm Quỳnh', CAST(N'2012-09-02' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Phú, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036312016492', N'Đang học', 156)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (157, N'Phạm Diễm Quỳnh', CAST(N'2012-07-22' AS Date), N'Nữ', CAST(N'2024-03-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036312010777', N'Đang học', 157)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (158, N'Vũ Quang Thiện', CAST(N'2012-06-28' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036212006025', N'Đang học', 158)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (159, N'Vũ Thị Thanh Thủy', CAST(N'2012-01-06' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Tam Thái, huyện Tương Dương, tỉnh Nghệ An', N'Công giáo', 0, N'036312003140', N'Đang học', 159)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (160, N'Vũ Anh Thư', CAST(N'2012-10-23' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036312006895', N'Đang học', 160)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (161, N'Phạm Thị Quỳnh Trang', CAST(N'2012-10-11' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'xóm 6C ,xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036312007677', N'Đang học', 161)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (162, N'Phạm Thùy Trang', CAST(N'2012-07-15' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Ninh, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036312011873', N'Đang học', 162)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (163, N'Lâm Vũ Hà Vy', CAST(N'2012-10-05' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036312003235', N'Đang học', 163)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (164, N'Nguyễn Ngọc Thảo Vy', CAST(N'2012-01-01' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036312008756', N'Đang học', 164)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (165, N'Vũ Mai Tuyết Anh', CAST(N'2012-05-13' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm  Ninh Đông, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Ninh, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036312013482', N'Đang học', 165)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (166, N'Vũ Hải Chính', CAST(N'2012-01-30' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Chính, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036212008313', N'Đang học', 166)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (167, N'Trần Thị Phương Dung', CAST(N'2012-05-12' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036312004422', N'Đang học', 167)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (168, N'Nguyễn Hoàng Duy', CAST(N'2012-03-15' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036212021512', N'Đang học', 168)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (169, N'Bùi Tiến Đại Dương', CAST(N'2012-08-02' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Phú, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'030212014322', N'Đang học', 169)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (170, N'Nguyễn Hoàng Hải', CAST(N'2012-03-07' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Đông, Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036212007819', N'Đang học', 170)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (171, N'Vũ Xuân Hải', CAST(N'2012-10-04' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Ninh, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036212017937', N'Đang học', 171)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (172, N'Bùi Quang Huy', CAST(N'2012-03-30' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036212019737', N'Đang học', 172)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (173, N'Phạm Quang Huy', CAST(N'2012-12-14' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 1, xã Hải Ninh, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Phụ sản tỉnh Nam Định', N'Không', 0, N'036212024515', N'Đang học', 173)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (174, N'Trịnh Gia Huy', CAST(N'2012-06-12' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036212016278', N'Đang học', 174)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (175, N'Nguyễn Thị Thoại Khanh', CAST(N'2012-08-07' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Khoa sản phụ bệnh viện 4 Quân đoàn 4', N'Công giáo', 0, N'036312011527', N'Đang học', 175)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (176, N'Trần Xuân Kiên', CAST(N'2012-11-05' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036212017253', N'Đang học', 176)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (177, N'Nguyễn Hoàng Long', CAST(N'2012-10-14' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Đông, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Thống Nhất, tỉnh Đồng Nai', N'Công giáo', 0, N'036212009580', N'Đang học', 177)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (178, N'Vũ Gia Lộc', CAST(N'2012-05-13' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải  Giang, huyện  Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036212008684', N'Đang học', 178)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (179, N'Vũ Thị Trà My', CAST(N'2012-05-30' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Đông, Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Phú, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036312003526', N'Đang học', 179)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (180, N'Phạm Hoài Nam', CAST(N'2012-10-23' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 9B, xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036212019028', N'Đang học', 180)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (181, N'Trần Thị Huyền Nga', CAST(N'2012-11-13' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện  huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036312002707', N'Đang học', 181)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (182, N'Nguyễn Khánh Ngọc', CAST(N'2012-10-13' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036212009444', N'Đang học', 182)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (183, N'Vũ Duy Phúc', CAST(N'2011-11-18' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thuận, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trung tâm Y tế huyện Mang Yang, tỉnh Gia Lai', N'Công giáo', 0, N'064211007477', N'Đang học', 183)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (184, N'Tống Thị Bích Phượng', CAST(N'2012-09-18' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Phú, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036312015078', N'Đang học', 184)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (185, N'Bùi Minh Quang', CAST(N'2012-02-25' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036212023168', N'Đang học', 185)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (186, N'Nguyễn Hoàng Quân', CAST(N'2012-04-29' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Đông, Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Phụ sản tỉnh Nam Định', N'Không', 0, N'036212015567', N'Đang học', 186)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (187, N'Phạm Hoàng Quân', CAST(N'2012-09-24' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'xóm 9A ,xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036212021209', N'Đang học', 187)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (188, N'Vũ Đức Minh Quân', CAST(N'2012-05-06' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện sản nhi tỉnh Lào Cai', N'Công giáo', 0, N'010212000460', N'Đang học', 188)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (189, N'Nguyễn Phương Thanh', CAST(N'2012-11-12' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036312011520', N'Đang học', 189)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (190, N'Trần Phương Thảo', CAST(N'2012-04-17' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải  Giang, huyện  Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036312002553', N'Đang học', 190)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (191, N'Mai Đức Thiện', CAST(N'2011-01-07' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036211017826', N'Đang học', 191)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (192, N'Nguyễn Thị Minh Thư', CAST(N'2012-08-19' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Hùng Vương, Thành phố Hồ Chí Minh', N'Công giáo', 0, N'036312015743', N'Đang học', 192)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (193, N'Phạm Nguyễn Minh Thư', CAST(N'2012-06-16' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa tỉnh Đắk Nông', N'Công giáo', 0, N'067312008074', N'Đang học', 193)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (194, N'Trần Thủy Tiên', CAST(N'2012-05-22' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa Kiên Lương, tỉnh Kiên Giang', N'Công giáo', 0, N'036312007389', N'Đang học', 194)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (195, N'Trần Duy Tiến', CAST(N'2011-04-15' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Hà, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036211020824', N'Đang học', 195)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (196, N'Lê Quang Trưởng', CAST(N'2012-10-25' AS Date), N'Nam', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa Thống Nhất tỉnh Đồng Nai', N'Không', 0, N'036212023180', N'Đang học', 196)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (197, N'Nguyễn Thị Tươi', CAST(N'2012-11-20' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036312005367', N'Đang học', 197)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (198, N'Nguyễn Thị Kiều Vy', CAST(N'2012-11-27' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Long Sơn, thành phố Vũng Tàu, tỉnh Bà Rịa - Vũng Tàu', N'Công giáo', 0, N'036312018025', N'Đang học', 198)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (199, N'Nguyễn Tường Vy', CAST(N'2012-04-16' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thuận, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036312010801', N'Đang học', 199)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (200, N'Bùi Hải Yến', CAST(N'2012-08-26' AS Date), N'Nữ', CAST(N'2023-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Đông, Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036312009310', N'Đang học', 200)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (201, N'Trần Đình Khánh Chi', CAST(N'2012-05-03' AS Date), N'Nữ', CAST(N'2025-02-03' AS Date), N'Chuyển đến từ trường khác', N'Kinh', N'Ấp Cây Cầy - Xã Phú Lý - Vĩnh Cửu - Đồng Nai', N'Bệnh viện Đa khoa Đồng Nai', N'', 0, N'036311004387', N'Đang học', 201)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (202, N'Phạm Thị Phương Anh', CAST(N'2011-09-29' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 9b Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036211010232', N'Đang học', 202)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (203, N'Mai Trung Bảo', CAST(N'2011-08-05' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036311009025', N'Đang học', 203)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (204, N'Phạm Thị Ngọc Bích', CAST(N'2011-02-15' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 9b Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036311018046', N'Đang học', 204)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (205, N'Bùi Thị Bảo Châm', CAST(N'2011-05-13' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tinh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'022211008424', N'Đang học', 205)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (206, N'Vũ Minh Châu', CAST(N'2011-05-05' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa Tỉnh Quảng Ninh', N'Không', 0, N'036311009028', N'Đang học', 206)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (207, N'Vũ Quỳnh Chi', CAST(N'2011-10-10' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu,tỉnh Nam Định', N'Công giáo', 0, N'036210004580', N'Đang học', 207)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (208, N'Nguyễn Tiến Cường', CAST(N'2010-11-20' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'075211016770', N'Đang học', 208)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (209, N'Đặng Nguyễn Đức Duy', CAST(N'2011-04-25' AS Date), N'Nam', CAST(N'2024-01-15' AS Date), N'Trúng tuyển', N'Kinh', N'xóm Ninh Thành, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa Thống Nhất Đồng Nai', N'Không', 0, N'036211007504', N'Đang học', 209)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (210, N'Vũ Anh Dũng', CAST(N'2011-11-26' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036211003428', N'Đang học', 210)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (211, N'Phạm Văn Đại', CAST(N'2011-11-21' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Phụ sản Nam Định tỉnh Nam Định', N'Không', 0, N'036211006612', N'Đang học', 211)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (212, N'Nguyễn Công Hiếu', CAST(N'2011-06-09' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036211010752', N'Đang học', 212)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (213, N'Trần Công Hiếu', CAST(N'2011-05-21' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm  Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036211017307', N'Đang học', 213)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (214, N'Trần Trung Hiếu', CAST(N'2011-05-08' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036211016154', N'Đang học', 214)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (215, N'Nguyễn Huy Hiệu', CAST(N'2011-12-04' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Hà, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang,huyện Hải Hậu,tỉnh Nam Định', N'Công giáo', 0, N'036211009444', N'Đang học', 215)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (216, N'Nguyễn Thành Hội', CAST(N'2011-06-13' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036211020280', N'Đang học', 216)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (217, N'Nguyễn Gia Huy', CAST(N'2011-02-11' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036211011464', N'Đang học', 217)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (218, N'Nguyễn Gia Huy', CAST(N'2011-01-02' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tinh Nam Định', N'Trạm y tế xã Hải Ninh, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036211012114', N'Đang học', 218)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (219, N'Phạm Minh Khánh', CAST(N'2011-03-30' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'049311012868', N'Đang học', 219)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (220, N'Nguyễn Thị Khánh Ly', CAST(N'2011-07-15' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Hà, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036311011571', N'Đang học', 220)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (221, N'Nguyễn Quang Minh', CAST(N'2011-02-01' AS Date), N'Nam', CAST(N'2024-09-16' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa số I, phường Kim Tân, thành phố Lào Cai, tỉnh Lào Cai', N'Không', 0, N'010211008521', N'Đang học', 221)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (222, N'Phạm Trần Trà My', CAST(N'2011-12-10' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tinh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036311006240', N'Đang học', 222)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (223, N'Phạm Thị Kim Ngân', CAST(N'2011-06-23' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tinh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036311010977', N'Đang học', 223)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (224, N'Đồng Thị Hồng Ngọc', CAST(N'2011-04-06' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Hà, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036311016201', N'Đang học', 224)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (225, N'Nguyễn Đức Phát', CAST(N'2011-01-01' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036211002921', N'Đang học', 225)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (226, N'Mai Hồng Phúc', CAST(N'2011-09-14' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tinh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036211014545', N'Đang học', 226)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (227, N'Phạm Hồng Phúc', CAST(N'2011-03-11' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Đông, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036211012706', N'Đang học', 227)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (228, N'Nguyễn Tiến Thành', CAST(N'2011-11-01' AS Date), N'Nam', CAST(N'2024-08-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 2, xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định.', N'Xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định.', N'Không', 0, N'036211000236', N'Đang học', 228)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (229, N'Hoàng Hương Thảo', CAST(N'2011-10-26' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tinh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036311013310', N'Đang học', 229)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (230, N'Phạm Đoàn Bảo Thi', CAST(N'2011-10-29' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 9a ,xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Trực Hùng- Trực Ninh- Nam Định', N'Công giáo', 0, N'036311013113', N'Đang học', 230)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (231, N'Phạm Thị Hoàng Thúy', CAST(N'2011-12-10' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, huyện Hải Hậu, tinh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036311003107', N'Đang học', 231)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (232, N'Vũ Thành Tiến', CAST(N'2011-04-09' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Khoa phụ sản bệnh viện huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036211020145', N'Đang học', 232)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (233, N'Phạm Thị Thùy Trang', CAST(N'2011-11-11' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa Thống Nhất, tỉnh Đồng Nai', N'Công giáo', 0, N'036311003089', N'Đang học', 233)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (234, N'Vũ Thị Huyền Trang', CAST(N'2011-10-05' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036311008452', N'Đang học', 234)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (235, N'Vũ Đức Trọng', CAST(N'2011-03-12' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036211016529', N'Đang học', 235)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (236, N'Nguyễn Anh Tuấn', CAST(N'2011-06-16' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Hà, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036211007195', N'Đang học', 236)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (237, N'Nguyễn Anh Tuấn', CAST(N'2010-03-02' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036210018689', N'Đang học', 237)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (238, N'Trần Tuấn Tú', CAST(N'2011-10-29' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036211006030', N'Đang học', 238)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (239, N'Nguyễn Ngọc Tường Vy', CAST(N'2011-01-24' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036311009428', N'Đang học', 239)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (240, N'Phạm Trần Kỳ Anh', CAST(N'2011-08-23' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Ninh, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036211019005', N'Đang học', 240)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (241, N'Trần Trung Bảo', CAST(N'2010-08-25' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036210017153', N'Đang học', 241)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (242, N'Hoàng Thị Linh Chi', CAST(N'2011-05-24' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036311009532', N'Đang học', 242)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (243, N'Vũ Đức Diện', CAST(N'2011-05-15' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải  Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang,huyện Hải Hậu,tỉnh Nam Định', N'Công giáo', 0, N'036211017211', N'Đang học', 243)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (244, N'Vũ Minh Dũng', CAST(N'2011-01-13' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu,tỉnh Nam Định', N'Công giáo', 0, N'036211012928', N'Đang học', 244)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (245, N'Vũ Hải Đăng', CAST(N'2011-05-09' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu,tỉnh Nam Định', N'Công giáo', 0, N'036211013816', N'Đang học', 245)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (246, N'Phạm Minh Đức', CAST(N'2011-01-02' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang,huyện Hải Hậu,tỉnh Nam Định', N'Công giáo', 0, N'036211012526', N'Đang học', 246)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (247, N'Phạm Thế Hải', CAST(N'2011-02-22' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thuận, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036211005393', N'Đang học', 247)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (248, N'Nguyễn Thị Bích Hạnh', CAST(N'2011-02-24' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa TP Biên Hòa,tỉnh Đồng Nai', N'Không', 0, N'036311019225', N'Đang học', 248)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (249, N'Nguyễn Trung Hiếu', CAST(N'2010-11-23' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Phong,huyện Hải Hậu,tỉnh Nam Định', N'Công giáo', 0, N'036210019464', N'Đang học', 249)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (250, N'Phạm Minh Hiếu', CAST(N'2011-07-11' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Đông, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Phong,huyện Hải Hậu,tỉnh Nam Định', N'Công giáo', 0, N'036211003122', N'Đang học', 250)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (251, N'Vũ Gia Huy', CAST(N'2011-07-29' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thuận, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Khoa phụ sản bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036211020883', N'Đang học', 251)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (252, N'Đặng Việt Hưng', CAST(N'2011-12-05' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Nga Yên,huyện Nga Sơn,tỉnh Thanh Hóa', N'Không', 0, N'038211013101', N'Đang học', 252)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (253, N'Mai Tiến Hưng', CAST(N'2011-09-10' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải  Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu,tỉnh Nam Định', N'Công giáo', 0, N'036211018061', N'Đang học', 253)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (254, N'Vũ Duy Khánh', CAST(N'2011-03-16' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang,huyện Hải Hậu,tỉnh Nam Định', N'Công giáo', 0, N'036211019045', N'Đang học', 254)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (255, N'Phan Thị Hương Lan', CAST(N'2011-03-22' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Khoa phụ sản bệnh viện đa khoa huyện Hải Hậu,tỉnh Nam Định', N'Không', 0, N'036311008965', N'Đang học', 255)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (256, N'Phạm Kiều Linh', CAST(N'2011-12-13' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 4 Hải An, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu,tỉnh Nam Định', N'Công giáo', 0, N'036311015544', N'Đang học', 256)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (257, N'Trịnh Công Minh', CAST(N'2011-10-31' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 9a, xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Phong,huyện Hải Hậu,tỉnh Nam Định', N'Không', 0, N'036211016871', N'Đang học', 257)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (258, N'Nguyễn Thị Trà My', CAST(N'2011-10-29' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải  Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang,huyện Hải Hậu,tỉnh Nam Định', N'Công giáo', 0, N'036311010542', N'Đang học', 258)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (259, N'Phan Trà My', CAST(N'2011-09-15' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036311017183', N'Đang học', 259)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (260, N'Vũ Ánh Ngọc', CAST(N'2011-12-06' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu,tỉnh Nam Định', N'Công giáo', 0, N'036311018597', N'Đang học', 260)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (261, N'Phạm Nguyễn Tuyết Nhung', CAST(N'2011-07-03' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 8b Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Phú,huyện Hải Hậu,tỉnh Nam Định', N'Không', 0, N'036311011618', N'Đang học', 261)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (262, N'Lê Quỳnh Như', CAST(N'2011-01-31' AS Date), N'Nữ', CAST(N'2023-09-12' AS Date), N'Chuyển đến từ trường khác', N'Kinh', N'Ea Sinh 2, Xã Cư Ni, Huyện Ea Kar, Tỉnh Đắk Lắk', N'Ea Kar, Đắk Lắk', N'Không', 0, N'036311014470', N'Đang học', 262)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (263, N'Trần Gia Phát', CAST(N'2011-04-24' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang,huyện Hải Hậu,tỉnh Nam Định', N'Công giáo', 0, N'036211001987', N'Đang học', 263)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (264, N'Phan Minh Quân', CAST(N'2011-01-03' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Đông, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang,huyện Hải Hậu,tỉnh Nam Định', N'Công giáo', 0, N'036211023474', N'Đang học', 264)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (265, N'Nguyễn Hữu Tất Thành', CAST(N'2011-05-19' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Thạch Hà,tỉnh Hà Tĩnh', N'Không', 0, N'042211007351', N'Đang học', 265)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (266, N'Nguyễn Thanh Thư', CAST(N'2011-09-08' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Hà, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu,tỉnh Nam Định', N'Công giáo', 0, N'036311016930', N'Đang học', 266)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (267, N'Nguyễn Mạnh Tiến', CAST(N'2011-02-03' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Tân,huyện Hải Hậu,tỉnh Nam Định', N'Công giáo', 0, N'036211008679', N'Đang học', 267)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (268, N'Vũ Minh Tiến', CAST(N'2011-01-24' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu,tỉnh Nam Định', N'Công giáo', 0, N'036211009568', N'Đang học', 268)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (269, N'Nguyễn Minh Tiệp', CAST(N'2011-09-09' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu,tỉnh Nam Định', N'Công giáo', 0, N'036211004705', N'Đang học', 269)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (270, N'Trần Thị Mai Trang', CAST(N'2011-11-07' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang,huyện Hải Hậu,tỉnh Nam Định', N'Công giáo', 0, N'036311009712', N'Đang học', 270)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (271, N'Nguyễn Thị Thùy Trâm', CAST(N'2011-09-12' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 9A xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu,tỉnh Nam Định', N'Không', 0, N'036311009736', N'Đang học', 271)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (272, N'Nguyễn Thị Ánh Tuyết', CAST(N'2011-10-25' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang,huyện Hải Hậu,tỉnh Nam Định', N'Công giáo', 0, N'036311019048', N'Đang học', 272)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (273, N'Vũ Thị Ngọc Viên', CAST(N'2011-08-08' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 11, xã Hải An, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải An, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036311011823', N'Đang học', 273)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (274, N'Vũ Đức Vinh', CAST(N'2011-10-11' AS Date), N'Nam', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu,tỉnh Nam Định', N'Công giáo', 0, N'036311018659', N'Đang học', 274)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (275, N'Nguyễn Thị Tường Vy', CAST(N'2011-08-05' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Hà, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Khoa phụ sản bệnh viện đa khoa huyện Hải Hậu,tỉnh Nam Định', N'Không', 0, N'036211008792', N'Đang học', 275)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (276, N'Trần Phương Vy', CAST(N'2011-04-28' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu,tỉnh Nam Định', N'Công giáo', 0, N'036311019987', N'Đang học', 276)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (277, N'Trần Thảo Vy', CAST(N'2011-01-02' AS Date), N'Nữ', CAST(N'2022-09-05' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu,tỉnh Nam Định', N'Công giáo', 0, N'036311007954', N'Đang học', 277)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (278, N'Đỗ Tuấn Phát', CAST(N'2010-09-21' AS Date), N'Nam', CAST(N'2025-01-17' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Từ Dũ, Thành phố Hồ Chí Minh.', N'', 0, N'036311006600', N'Đang học', 278)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (279, N'Nguyễn Ngọc Anh', CAST(N'2010-11-15' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thuận, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036310004225', N'Đang học', 279)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (280, N'Phạm Minh Anh', CAST(N'2010-09-19' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Xuân Ngọc, huyện Xuân Trường, tỉnh Nam Định', N'Công giáo', 0, N'036310012909', N'Đang học', 280)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (281, N'Lâm Nhật Ánh', CAST(N'2010-04-09' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036310006277', N'Đang học', 281)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (282, N'Trần Gia Bảo', CAST(N'2010-02-07' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036210004868', N'Đang học', 282)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (283, N'Vũ Ngọc Diệp', CAST(N'2010-09-02' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện  Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036310017401', N'Đang học', 283)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (284, N'Lê Ngọc Diệu', CAST(N'2010-11-22' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện  Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036310007212', N'Đang học', 284)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (285, N'Tạ Tiến Dinh', CAST(N'2010-10-04' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036210018547', N'Đang học', 285)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (286, N'Vũ Phương Dung', CAST(N'2010-04-08' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036310006766', N'Đang học', 286)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (287, N'Phạm Hoàng Trí Dũng', CAST(N'2010-01-02' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 9B Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Ninh, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036210007926', N'Đang học', 287)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (288, N'Đỗ Hoàng Hải', CAST(N'2010-05-25' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036210014879', N'Đang học', 288)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (289, N'Nguyễn Đức Hoàng', CAST(N'2010-03-08' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036210018928', N'Đang học', 289)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (290, N'Nguyễn Văn Hoàng', CAST(N'2010-01-01' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Minh, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036210010480', N'Đang học', 290)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (291, N'Nguyễn Thị Thu Hồng', CAST(N'2010-10-07' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036310014289', N'Đang học', 291)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (292, N'Vũ Xuân Hồng', CAST(N'2010-05-07' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Ninh, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036210007672', N'Đang học', 292)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (293, N'Nguyễn Thị Thu Hương', CAST(N'2010-08-03' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm  Mỹ Thuận, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'', N'Không', 0, N'036310011702', N'Đang học', 293)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (294, N'Trần Thị Bảo Liên', CAST(N'2010-01-28' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036310001523', N'Đang học', 294)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (295, N'Nguyễn Kiều Linh', CAST(N'2010-02-01' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Đông,  xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Phụ sản tỉnh Thái Bình', N'Công giáo', 0, N'036310002058', N'Đang học', 295)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (296, N'Trần Khánh Linh', CAST(N'2010-07-10' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thuận, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa Khu vực Hóc Môn, thành phố Hồ Chí Minh', N'Không', 0, N'036310000364', N'Đang học', 296)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (297, N'Vũ Phương Linh', CAST(N'2010-11-14' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036310014941', N'Đang học', 297)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (298, N'Trần Thị Thảo Ly', CAST(N'2010-08-09' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036310007941', N'Đang học', 298)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (299, N'Nguyễn Hoàng Minh', CAST(N'2010-07-29' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Đông, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Trực Hùng
, huyện Trực Ninh, tỉnh Nam Định', N'Công giáo', 0, N'036210012649', N'Đang học', 299)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (300, N'Vũ Hoàng Minh', CAST(N'2010-11-05' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036210014411', N'Đang học', 300)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (301, N'Vũ Tuyết Minh', CAST(N'2010-01-22' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036310001607', N'Đang học', 301)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (302, N'Phạm Thị Trà My', CAST(N'2010-11-04' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 14, xã Hải Phú, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Phú, huyện  Hải Hậu, tinh Nam Định', N'Công giáo', 0, N'036310011556', N'Đang học', 302)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (303, N'Bùi Vũ Ánh Nguyệt', CAST(N'2010-08-16' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036310005202', N'Đang học', 303)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (304, N'Vũ Nguyễn Hoàng Phi', CAST(N'2010-01-20' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa,  xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036210002220', N'Đang học', 304)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (305, N'Nguyễn Phạm Uy Phong', CAST(N'2010-10-03' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Hà, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036210014515', N'Đang học', 305)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (306, N'Trần Văn Phố', CAST(N'2010-07-14' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036210001430', N'Đang học', 306)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (307, N'Nguyễn Bình Phước', CAST(N'2010-08-30' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Hà, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa Khoa
 tỉnh Bình Phước', N'Công giáo', 0, N'036210014517', N'Đang học', 307)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (308, N'Nguyễn Minh Quân', CAST(N'2009-11-15' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa,  xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036209017302', N'Đang học', 308)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (309, N'Đặng Thanh Tâm', CAST(N'2010-03-25' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thuận, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036210008394', N'Đang học', 309)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (310, N'Đinh Mẫn Thi', CAST(N'2010-03-08' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Hà, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Từ Dũ Quận 1-
 Thành phố Hồ Chí Minh', N'Không', 0, N'079310023664', N'Đang học', 310)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (311, N'Phạm Thị Anh Thư', CAST(N'2010-05-19' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036310004112', N'Đang học', 311)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (312, N'Nguyễn Minh Tranh', CAST(N'2010-11-15' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Ninh, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036210007518', N'Đang học', 312)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (313, N'Đồng Mai Thùy Trâm', CAST(N'2010-10-14' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Hà, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036310008350', N'Đang học', 313)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (314, N'Mai Thị Bảo Trâm', CAST(N'2010-09-13' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trung Tâm y tế huyện Long Điền, tỉnh Bà Rịa Vũng Tàu', N'Công giáo', 0, N'036310013348', N'Đang học', 314)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (315, N'Vũ Quốc Triệu', CAST(N'2010-10-31' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036210006122', N'Đang học', 315)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (316, N'Trịnh Anh Tuấn', CAST(N'2010-05-31' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 9B Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa tỉnh Bình Dương', N'Không', 0, N'036210004009', N'Đang học', 316)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (317, N'Đặng Thị Tươi', CAST(N'2010-03-17' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036310003201', N'Đang học', 317)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (318, N'Trần Thị Hải Yến', CAST(N'2010-11-07' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Ninh, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036310006326', N'Đang học', 318)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (319, N'Vũ Hải Yến', CAST(N'2010-02-02' AS Date), N'Nữ', CAST(N'2022-02-08' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036310006936', N'Đang học', 319)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (320, N'Nguyễn Hoàng Anh', CAST(N'2010-01-31' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036210003360', N'Đang học', 320)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (321, N'Trần Tuấn Anh', CAST(N'2010-04-22' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036210018583', N'Đang học', 321)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (322, N'Vũ Thị Vân Anh', CAST(N'2010-03-29' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036310005163', N'Đang học', 322)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (323, N'Phạm Thị Hồng Ánh', CAST(N'2010-05-26' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036310015926', N'Đang học', 323)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (324, N'Phạm Quỳnh Chi', CAST(N'2010-05-31' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036210009698', N'Đang học', 324)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (325, N'Vũ Phương Duyên', CAST(N'2010-04-08' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036310016875', N'Đang học', 325)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (326, N'Lâm Tiến Đạt', CAST(N'2010-03-12' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036310016348', N'Đang học', 326)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (327, N'Phạm Tiến Đạt', CAST(N'2010-10-20' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Đông, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện  Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036210007906', N'Đang học', 327)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (328, N'Nguyễn Hải Đăng', CAST(N'2010-04-21' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện  Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036210013966', N'Đang học', 328)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (329, N'Trần Thị Giang', CAST(N'2010-12-12' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thọ, Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036210017715', N'Đang học', 329)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (330, N'Trần Thị Ninh Giang', CAST(N'2010-09-12' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Đại Bản, huyện An Dương, thành phố Hải Phòng', N'Không', 0, N'036310012510', N'Đang học', 330)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (331, N'Trịnh Tuấn Giang', CAST(N'2010-02-13' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 9B, xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Khoa phụ sản bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036310012948', N'Đang học', 331)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (332, N'Vương Phi Hải', CAST(N'2010-11-05' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Phụ sản tỉnh Nam Định', N'Không', 0, N'036210008282', N'Đang học', 332)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (333, N'Nguyễn Thị Thu Hiền', CAST(N'2010-01-14' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036210016612', N'Đang học', 333)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (334, N'Bùi Thu Hoài', CAST(N'2010-01-18' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036310012695', N'Đang học', 334)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (335, N'Nguyễn Ngọc Huyền', CAST(N'2010-12-07' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036310004187', N'Đang học', 335)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (336, N'Nguyễn Quốc Khánh', CAST(N'2010-06-17' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Phú Quý, xã Hải Ninh, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Ninh
, huyện Hải Hậu, tỉnh  Nam Định', N'Không', 0, N'036310016894', N'Đang học', 336)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (337, N'Nguyễn Tuấn Kiệt', CAST(N'2010-06-15' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 9B, xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang , huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036210006550', N'Đang học', 337)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (338, N'Đỗ Thị Ngọc Linh', CAST(N'2010-12-03' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036210004318', N'Đang học', 338)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (339, N'Nguyễn Khánh Linh', CAST(N'2010-11-13' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Đông, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036310014836', N'Đang học', 339)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (340, N'Trần Thị Bảo Linh', CAST(N'2010-07-31' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang,  xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế Nam Dong- Cư Jút- Đắk Nông', N'Không', 0, N'036310014857', N'Đang học', 340)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (341, N'Vũ Hiền Lương', CAST(N'2010-02-19' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Đông, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Nghĩa Lạc, huyện Nghĩa Hưng, tỉnh Nam Định', N'Không', 0, N'036310016222', N'Đang học', 341)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (342, N'Trịnh Thị Trà My', CAST(N'2010-11-05' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 9B, xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Trạm Y tế xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036310001945', N'Đang học', 342)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (343, N'Đặng Bảo Ngọc', CAST(N'2010-10-10' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thuận, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036310001167', N'Đang học', 343)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (344, N'Nguyễn Ánh Ngọc', CAST(N'2010-05-31' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036310016526', N'Đang học', 344)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (345, N'Nguyễn Kiều Oanh', CAST(N'2010-07-20' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Công giáo', 0, N'036310012508', N'Đang học', 345)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (346, N'Trần Minh Phúc', CAST(N'2010-05-05' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Giang,  xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036310003442', N'Đang học', 346)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (347, N'Nguyễn Đức Quang', CAST(N'2010-04-12' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036210003783', N'Đang học', 347)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (348, N'Trần Duy Quang', CAST(N'2010-03-15' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036210003516', N'Đang học', 348)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (349, N'Nguyễn Minh Tâm', CAST(N'2010-07-24' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Hòa, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036210014197', N'Đang học', 349)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (350, N'Nguyễn Tiến Thành', CAST(N'2010-01-15' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thuận, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036210007049', N'Đang học', 350)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (351, N'Phan Trung Thành', CAST(N'2010-10-06' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036210009339', N'Đang học', 351)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (352, N'Nguyễn Văn Thái', CAST(N'2010-09-08' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Tày', N'Xóm Ninh Giang,  xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036210014684', N'Đang học', 352)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (353, N'Nguyễn Minh Thuần', CAST(N'2010-02-08' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036210003589', N'Đang học', 353)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (354, N'Nguyễn Thị Huyền Trang', CAST(N'2010-11-18' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036210011186', N'Đang học', 354)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (355, N'Nguyễn Thu Trang', CAST(N'2010-11-13' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Trung, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036310016544', N'Đang học', 355)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (356, N'Vũ Minh Tuấn', CAST(N'2010-12-15' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thuận, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Trạm y tế xã Hải Giang, huyện Hải Hậu, tinh Nam Định', N'Không', 0, N'036310013999', N'Đang học', 356)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (357, N'Nguyễn Thị Cẩm Tú', CAST(N'2010-07-24' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036210015981', N'Đang học', 357)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (358, N'Tạ Thanh Tứ', CAST(N'2010-01-06' AS Date), N'Nam', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Ninh Thành, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036310003185', N'Đang học', 358)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (359, N'Nguyễn Thị Thảo Vân', CAST(N'2010-06-16' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Thuận, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036210011339', N'Đang học', 359)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (360, N'Vũ Trần Hạ Vy', CAST(N'2010-08-08' AS Date), N'Nữ', CAST(N'2021-09-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm 9A, xã Hải Phong, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện Đa khoa huyện Hải Hậu, tỉnh Nam Định', N'Không', 0, N'036310013145', N'Đang học', 360)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (361, N'Ngô Hoàng Kim', CAST(N'2025-04-03' AS Date), N'Nam', CAST(N'2025-04-09' AS Date), N'Trúng tuyển', N'Ha Noi', N'123 Đường Láng, Hà Nội', N'Ha Noi', N'Không', 0, N'123456789111', N'Tốt nghiệp', 1)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (362, N'Ngô Hoàng Kim2', CAST(N'2025-04-05' AS Date), N'Nữ', CAST(N'2025-04-05' AS Date), N'Trúng tuyển', N'Kinh', N'123 Đường Láng, Hà Nội', N'Ha Noi', N'Không', 0, N'123456789121', N'Bảo lưu', 1)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (363, N'Ngô Hoàng Kim3', CAST(N'2025-04-06' AS Date), N'Khác', CAST(N'2025-04-12' AS Date), N'Trúng tuyển', N'Kinh', N'123 Đường Láng, Hà Nội', N'Ha Noi', N'Không', 0, N'123456789511', N'Bảo lưu', 1)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (364, N'Ngô Hoàng asd3333', CAST(N'2025-04-06' AS Date), N'Nữ', CAST(N'2025-04-12' AS Date), N'Trúng tuyển', N'Kinh', N'123 Đường Láng, Hà Nội', N'Ha Noi', N'Không', 0, N'123456789171', N'Nghỉ học', 361)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (365, N'Ngô Hoàng asd6', CAST(N'2025-04-04' AS Date), N'Nam', CAST(N'2025-04-11' AS Date), N'Trúng tuyển', N'Kinh', N'123 Đường Láng, Hà Nội', N'Ha Noi', N'Không', 0, N'123456789123', N'Tốt nghiệp', 1)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (366, N'Ngô Hoàng asdbbbb', CAST(N'2025-04-06' AS Date), N'Nữ', CAST(N'2025-04-11' AS Date), N'Trúng tuyển', N'Kinh', N'123 Đường Láng, Hà Nội', N'Ha Noi', N'Không', 0, N'123456789153', N'Bảo lưu', 1)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (367, N'Ngô Hoàng abc3333', CAST(N'2025-04-06' AS Date), N'Khác', CAST(N'2025-04-04' AS Date), N'Trúng tuyển', N'Kinh', N'123 Đường Láng, Hà Nội', N'Ha Noi', N'Không', 0, N'123456789561', N'Tốt nghiệp', 1)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (368, N'Ngô Hoàng Kimabcd', CAST(N'2025-04-06' AS Date), N'Nữ', CAST(N'2025-04-04' AS Date), N'Trúng tuyển', N'Kinh', N'123 Đường Láng, Hà Nội', N'Ha Noi', N'Không', 0, N'020202004114', N'Đang học', 1)
GO
INSERT [dbo].[Students] ([StudentID], [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status], [ParentID]) VALUES (369, N'Vương Thị Ngọc Anhnaaaaa', CAST(N'2025-04-05' AS Date), N'Nữ', CAST(N'2025-04-01' AS Date), N'Trúng tuyển', N'Kinh', N'Xóm Mỹ Đức, xã Hải Giang, huyện Hải Hậu, tỉnh Nam Định', N'Bệnh viện đa khoa huyện Tuy Đức, tỉnh Đắk Nông', N'Không', 0, N'020201207114', N'Nghỉ học', 1)
GO
SET IDENTITY_INSERT [dbo].[Students] OFF
GO
SET IDENTITY_INSERT [dbo].[Subjects] ON 
GO
INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [SubjectCategory], [TypeOfGrade]) VALUES (1, N'string', N'string', N'Tính điểm')
GO
INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [SubjectCategory], [TypeOfGrade]) VALUES (78, N'Âm nhạc', N'Khoa học xã hội', N'Nhận xét')
GO
INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [SubjectCategory], [TypeOfGrade]) VALUES (79, N'Mỹ thuật', N'Khoa học xã hội', N'Nhận xét')
GO
INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [SubjectCategory], [TypeOfGrade]) VALUES (80, N'Hoạt động trải nghiệm', N'Khoa học xã hội', N'Nhận xét')
GO
INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [SubjectCategory], [TypeOfGrade]) VALUES (81, N'Toán', N'Khoa học tự nhiên', N'Tính điểm')
GO
INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [SubjectCategory], [TypeOfGrade]) VALUES (82, N'Ngữ văn', N'Khoa học xã hội', N'Tính điểm')
GO
INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [SubjectCategory], [TypeOfGrade]) VALUES (84, N'Vật lý', N'Khoa học tự nhiên', N'Tính điểm')
GO
INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [SubjectCategory], [TypeOfGrade]) VALUES (85, N'Hóa học', N'Khoa học tự nhiên', N'Tính điểm')
GO
INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [SubjectCategory], [TypeOfGrade]) VALUES (86, N'Sinh học', N'Khoa học tự nhiên', N'Tính điểm')
GO
INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [SubjectCategory], [TypeOfGrade]) VALUES (87, N'Lịch sử', N'Khoa học xã hội', N'Tính điểm')
GO
INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [SubjectCategory], [TypeOfGrade]) VALUES (88, N'Địa lý', N'Khoa học xã hội', N'Tính điểm')
GO
INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [SubjectCategory], [TypeOfGrade]) VALUES (89, N'Giáo dục công dân', N'Khoa học xã hội', N'Tính điểm')
GO
INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [SubjectCategory], [TypeOfGrade]) VALUES (90, N'Thể dục', N'Khoa học tự nhiên', N'Tính điểm')
GO
INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [SubjectCategory], [TypeOfGrade]) VALUES (91, N'Công nghệ', N'Khoa học tự nhiên', N'Tính điểm')
GO
INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [SubjectCategory], [TypeOfGrade]) VALUES (92, N'Tin học', N'Khoa học tự nhiên', N'Tính điểm')
GO
INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [SubjectCategory], [TypeOfGrade]) VALUES (93, N'Tiếng Hàn', N'Khoa học xã hội', N'Tính điểm')
GO
INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [SubjectCategory], [TypeOfGrade]) VALUES (95, N'Tiếng Trung', N'Khoa học xã hội', N'Tính điểm')
GO
INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [SubjectCategory], [TypeOfGrade]) VALUES (96, N'Test', N'Khoa học xã hội', N'Nhận xét')
GO
INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [SubjectCategory], [TypeOfGrade]) VALUES (97, N'Tiếng Đức', N'Khoa học xã hội', N'Tính điểm')
GO
INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [SubjectCategory], [TypeOfGrade]) VALUES (98, N'test123', N'Khoa học xã hội', N'Nhận xét')
GO
INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [SubjectCategory], [TypeOfGrade]) VALUES (99, N'test1234', N'Khoa học xã hội', N'Nhận xét')
GO
INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [SubjectCategory], [TypeOfGrade]) VALUES (100, N'test123456', N'Khoa học xã hội', N'Tính điểm')
GO
INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [SubjectCategory], [TypeOfGrade]) VALUES (101, N'test123aaaaaaa', N'Khoa học xã hội', N'Nhận xét')
GO
INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [SubjectCategory], [TypeOfGrade]) VALUES (102, N'co 4', N'Khoa học xã hội', N'Nhận xét')
GO
INSERT [dbo].[Subjects] ([SubjectID], [SubjectName], [SubjectCategory], [TypeOfGrade]) VALUES (103, N'SEP490', N'Khoa học xã hội', N'Nhận xét')
GO
SET IDENTITY_INSERT [dbo].[Subjects] OFF
GO
SET IDENTITY_INSERT [dbo].[Teachers] ON 
GO
INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FullName], [DOB], [Gender], [Ethnicity], [Religion], [MaritalStatus], [IDCardNumber], [InsuranceNumber], [EmploymentType], [Position], [Department], [IsHeadOfDepartment], [EmploymentStatus], [RecruitmentAgency], [HiringDate], [PermanentEmploymentDate], [SchoolJoinDate], [PermanentAddress], [Hometown]) VALUES (1, 2, N'Vương Thị Ngọc Anh', CAST(N'2000-06-24' AS Date), N'Nữ', N'Kinh', N'Công giáo', N'Đã kết hôn', N'123456789', N'8901054383', N'Hợp đồng lao động dưới 1 năm', N'Giáo viên', N'Khoa học xã hội', 0, N'Đang làm việc', N'Ban tổ chức chính quyền tỉnh Nam Định', CAST(N'1999-09-01' AS Date), CAST(N'1999-09-01' AS Date), CAST(N'2024-09-01' AS Date), N'Xóm Mỹ Hòa xã Hải Giang huyện Hải Hậu tỉnh Nam Định', N'Tỉnh Nam Định')
GO
INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FullName], [DOB], [Gender], [Ethnicity], [Religion], [MaritalStatus], [IDCardNumber], [InsuranceNumber], [EmploymentType], [Position], [Department], [IsHeadOfDepartment], [EmploymentStatus], [RecruitmentAgency], [HiringDate], [PermanentEmploymentDate], [SchoolJoinDate], [PermanentAddress], [Hometown]) VALUES (2, 3, N'Tạ Tuấn Anh', CAST(N'1977-07-20' AS Date), N'Nam', N'Kinh', N'Không', N'Đã kết hôn', N'123456790', N'8901004383', N'Viên chức HĐLV không xác định thời hạn', N'Cán bộ quản lý', N'Khoa học xã hội', 0, N'Đang làm việc', N'Ban Tổ chức CQ tỉnh Nam Định', CAST(N'1999-09-01' AS Date), CAST(N'1999-09-01' AS Date), CAST(N'2013-02-15' AS Date), N'Xóm Mỹ Hòa xã Hải Giang huyện Hải Hậu tỉnh Nam Định', N'Tỉnh Nam Định')
GO
INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FullName], [DOB], [Gender], [Ethnicity], [Religion], [MaritalStatus], [IDCardNumber], [InsuranceNumber], [EmploymentType], [Position], [Department], [IsHeadOfDepartment], [EmploymentStatus], [RecruitmentAgency], [HiringDate], [PermanentEmploymentDate], [SchoolJoinDate], [PermanentAddress], [Hometown]) VALUES (3, 4, N'Phạm Thị Duyên', CAST(N'1972-06-15' AS Date), N'Nữ', N'Kinh', N'Công giáo', N'Độc thân', N'123456792', N'2596026623', N'Viên chức HĐLV không xác định thời hạn', N'Cán bộ quản lý', N'Khoa học tự nhiên', 0, N'Đang làm việc', N'UBND huyện Hải Hậu', CAST(N'1993-09-01' AS Date), CAST(N'1993-09-01' AS Date), CAST(N'2021-08-01' AS Date), N'Xóm 12 xã Hải Ninh huyện Hải Hậu tỉnh Nam Định', N'Tỉnh Nam Định')
GO
INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FullName], [DOB], [Gender], [Ethnicity], [Religion], [MaritalStatus], [IDCardNumber], [InsuranceNumber], [EmploymentType], [Position], [Department], [IsHeadOfDepartment], [EmploymentStatus], [RecruitmentAgency], [HiringDate], [PermanentEmploymentDate], [SchoolJoinDate], [PermanentAddress], [Hometown]) VALUES (4, 5, N'Phạm Công Đoàn', CAST(N'1967-07-18' AS Date), N'Nam', N'Kinh', N'Không', N'Đã kết hôn', N'123456793', N'2596026697', N'Viên chức HĐLV không xác định thời hạn', N'Giáo viên', N'Khoa học xã hội', 0, N'Đang làm việc', N'Ban tổ chức chính quyền tỉnh Nam Định', CAST(N'1989-09-01' AS Date), CAST(N'1989-09-10' AS Date), CAST(N'1989-09-10' AS Date), N'xóm Mỹ Hòa - Hải Giang - Hải Hậu - Nam Định', N'Tỉnh Nam Định')
GO
INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FullName], [DOB], [Gender], [Ethnicity], [Religion], [MaritalStatus], [IDCardNumber], [InsuranceNumber], [EmploymentType], [Position], [Department], [IsHeadOfDepartment], [EmploymentStatus], [RecruitmentAgency], [HiringDate], [PermanentEmploymentDate], [SchoolJoinDate], [PermanentAddress], [Hometown]) VALUES (5, 6, N'Nguyễn Thị Đượm', CAST(N'1984-12-27' AS Date), N'Nữ', N'Kinh', N'Không', N'Đã kết hôn', N'123456794213', N'8906605409', N'Viên chức HĐLV không xác định thời hạn', N'Nhân viên', N'Toàn trường', 0, N'Đang làm việc', N'UBND huyện Hải Hậu', CAST(N'2008-09-01' AS Date), CAST(N'2008-09-01' AS Date), CAST(N'2009-09-01' AS Date), N'Xóm Mỹ Thuận - Hải Giang - Hải Hậu - Nam Định', N'Tỉnh Nam Định')
GO
INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FullName], [DOB], [Gender], [Ethnicity], [Religion], [MaritalStatus], [IDCardNumber], [InsuranceNumber], [EmploymentType], [Position], [Department], [IsHeadOfDepartment], [EmploymentStatus], [RecruitmentAgency], [HiringDate], [PermanentEmploymentDate], [SchoolJoinDate], [PermanentAddress], [Hometown]) VALUES (6, 7, N'Phạm Thị Hải', CAST(N'1982-06-12' AS Date), N'Nữ', N'Kinh', N'Không', N'Đã kết hôn', N'123456795', N'8906005409', N'Viên chức HĐLV không xác định thời hạn', N'Giáo viên', N'Khoa học xã hội', 1, N'Đang làm việc', N'Ban tổ chức chính quyền tỉnh Nam Định', CAST(N'2004-10-01' AS Date), CAST(N'2004-10-01' AS Date), CAST(N'2004-10-01' AS Date), N'Xóm Mỹ Hòa - Hải Giang - Hải Hậu - Nam Định', N'Tỉnh Nam Định')
GO
INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FullName], [DOB], [Gender], [Ethnicity], [Religion], [MaritalStatus], [IDCardNumber], [InsuranceNumber], [EmploymentType], [Position], [Department], [IsHeadOfDepartment], [EmploymentStatus], [RecruitmentAgency], [HiringDate], [PermanentEmploymentDate], [SchoolJoinDate], [PermanentAddress], [Hometown]) VALUES (7, 8, N'Lê Thị Hằng', CAST(N'1993-08-18' AS Date), N'Nữ', N'Kinh', N'Không', N'Độc thân', N'123456796', N'8906005411', N'Hợp đồng lao động dưới 1 năm', N'Nhân viên', N'Toàn trường', 0, N'Đang làm việc', N'', CAST(N'2024-09-01' AS Date), CAST(N'2024-09-01' AS Date), CAST(N'2024-09-01' AS Date), N'Xóm 15, xã Hải Anh, huyện Hải Hậu, tỉnh Nam Định', N'Tỉnh Nam Định')
GO
INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FullName], [DOB], [Gender], [Ethnicity], [Religion], [MaritalStatus], [IDCardNumber], [InsuranceNumber], [EmploymentType], [Position], [Department], [IsHeadOfDepartment], [EmploymentStatus], [RecruitmentAgency], [HiringDate], [PermanentEmploymentDate], [SchoolJoinDate], [PermanentAddress], [Hometown]) VALUES (8, 9, N'Nguyễn Thị Hiền', CAST(N'1982-02-24' AS Date), N'Nữ', N'Kinh', N'Không', N'Đã kết hôn', N'123456797', N'8906005410', N'Viên chức HĐLV không xác định thời hạn', N'Giáo viên', N'Khoa học tự nhiên', 0, N'Đang làm việc', N'Sở nội vụ tỉnh Nam Định', CAST(N'2004-10-01' AS Date), CAST(N'2004-10-01' AS Date), CAST(N'2004-10-01' AS Date), N'Xóm 4 xã Hải Toàn huyện Hải Hậu tỉnh Nam Định', N'Tỉnh Nam Định')
GO
INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FullName], [DOB], [Gender], [Ethnicity], [Religion], [MaritalStatus], [IDCardNumber], [InsuranceNumber], [EmploymentType], [Position], [Department], [IsHeadOfDepartment], [EmploymentStatus], [RecruitmentAgency], [HiringDate], [PermanentEmploymentDate], [SchoolJoinDate], [PermanentAddress], [Hometown]) VALUES (9, 10, N'Vũ Thị Thu Hoài', CAST(N'1979-09-25' AS Date), N'Nữ', N'Kinh', N'Không', N'Đã kết hôn', N'123456798', N'8904002560', N'Viên chức HĐLV không xác định thời hạn', N'Giáo viên', N'Khoa học tự nhiên', 0, N'Đang làm việc', N'UBND huyện Hải Hậu', CAST(N'2002-10-01' AS Date), CAST(N'2002-10-01' AS Date), CAST(N'2002-10-01' AS Date), N'Xóm Mỹ Thuận xã Hải Giang huyện Hải Hậu tỉnh Nam Định', N'Tỉnh Nam Định')
GO
INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FullName], [DOB], [Gender], [Ethnicity], [Religion], [MaritalStatus], [IDCardNumber], [InsuranceNumber], [EmploymentType], [Position], [Department], [IsHeadOfDepartment], [EmploymentStatus], [RecruitmentAgency], [HiringDate], [PermanentEmploymentDate], [SchoolJoinDate], [PermanentAddress], [Hometown]) VALUES (10, 11, N'Vũ Thị Huệ', CAST(N'1974-12-16' AS Date), N'Nữ', N'Kinh', N'Không', N'Đã kết hôn', N'123456799', N'2596026700', N'Viên chức HĐLV không xác định thời hạn', N'Giáo viên', N'Khoa học xã hội', 0, N'Đang làm việc', N'Ban Tổ chức CQ tỉnh Nam Định', CAST(N'1996-09-01' AS Date), CAST(N'1996-09-01' AS Date), CAST(N'1996-09-01' AS Date), N'Xóm 8B xã Hải Phong huyện Hải Hậu tỉnh Nam Định', N'Tỉnh Nam Định')
GO
INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FullName], [DOB], [Gender], [Ethnicity], [Religion], [MaritalStatus], [IDCardNumber], [InsuranceNumber], [EmploymentType], [Position], [Department], [IsHeadOfDepartment], [EmploymentStatus], [RecruitmentAgency], [HiringDate], [PermanentEmploymentDate], [SchoolJoinDate], [PermanentAddress], [Hometown]) VALUES (11, 12, N'Nguyễn Thị Huyền', CAST(N'1981-11-05' AS Date), N'Nữ', N'Kinh', N'Công giáo', N'Độc thân', N'123456800', N'8905002197', N'Viên chức HĐLV không xác định thời hạn', N'Giáo viên', N'Khoa học tự nhiên', 0, N'Đang làm việc', N'Ban tổ chức chính quyền tỉnh Nam Định', CAST(N'2002-10-01' AS Date), CAST(N'2002-10-01' AS Date), CAST(N'2002-10-01' AS Date), N'Xóm 9 - Hải An - Hải Hậu -Nam Định', N'Tỉnh Nam Định')
GO
INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FullName], [DOB], [Gender], [Ethnicity], [Religion], [MaritalStatus], [IDCardNumber], [InsuranceNumber], [EmploymentType], [Position], [Department], [IsHeadOfDepartment], [EmploymentStatus], [RecruitmentAgency], [HiringDate], [PermanentEmploymentDate], [SchoolJoinDate], [PermanentAddress], [Hometown]) VALUES (12, 13, N'Trần Thị Lan', CAST(N'1988-01-05' AS Date), N'Nữ', N'Kinh', N'Công giáo', N'Đã kết hôn', N'123456801', N'3612011256', N'Viên chức HĐLV không xác định thời hạn', N'Nhân viên', N'Toàn trường', 0, N'Đang làm việc', N'UBND huyện Hải Hậu', CAST(N'2012-05-01' AS Date), CAST(N'2012-05-01' AS Date), CAST(N'2012-05-01' AS Date), N'Xóm  ninh đông - Hải Giang - Hải Hậu - Nam Định', N'Tỉnh Nam Định')
GO
INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FullName], [DOB], [Gender], [Ethnicity], [Religion], [MaritalStatus], [IDCardNumber], [InsuranceNumber], [EmploymentType], [Position], [Department], [IsHeadOfDepartment], [EmploymentStatus], [RecruitmentAgency], [HiringDate], [PermanentEmploymentDate], [SchoolJoinDate], [PermanentAddress], [Hometown]) VALUES (13, 14, N'Nguyễn Hữu Luận', CAST(N'1980-08-25' AS Date), N'Nam', N'Kinh', N'Không', N'Đã kết hôn', N'123456802', N'9405001011', N'Viên chức HĐLV không xác định thời hạn', N'Giáo viên', N'Khoa học tự nhiên', 0, N'Đang làm việc', N'Sở GD/ĐT Cà Mau', CAST(N'2004-10-01' AS Date), CAST(N'2004-10-01' AS Date), CAST(N'2004-10-01' AS Date), N'Xóm  Ninh Đông - Hải Giang - Hải Hậu - Nam Định', N'Tỉnh Nam Định')
GO
INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FullName], [DOB], [Gender], [Ethnicity], [Religion], [MaritalStatus], [IDCardNumber], [InsuranceNumber], [EmploymentType], [Position], [Department], [IsHeadOfDepartment], [EmploymentStatus], [RecruitmentAgency], [HiringDate], [PermanentEmploymentDate], [SchoolJoinDate], [PermanentAddress], [Hometown]) VALUES (14, 15, N'Vũ Viết Lượng', CAST(N'1971-03-05' AS Date), N'Nam', N'Kinh', N'Không', N'Đã kết hôn', N'123456803', N'8900000259', N'Hợp đồng lao động dưới 1 năm', N'Giáo viên', N'Khoa học xã hội', 0, N'Đang làm việc', N'Sở GD/ĐT Cà Mau', CAST(N'2004-10-01' AS Date), CAST(N'2004-10-01' AS Date), CAST(N'2021-09-01' AS Date), N'Xóm 6 - Phú Văn Nam - Hải Châu - Hải Hậu - Nam Định', N'Tỉnh Nam Định')
GO
INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FullName], [DOB], [Gender], [Ethnicity], [Religion], [MaritalStatus], [IDCardNumber], [InsuranceNumber], [EmploymentType], [Position], [Department], [IsHeadOfDepartment], [EmploymentStatus], [RecruitmentAgency], [HiringDate], [PermanentEmploymentDate], [SchoolJoinDate], [PermanentAddress], [Hometown]) VALUES (15, 16, N'Trần Thị Minh Nguyệt', CAST(N'1983-01-25' AS Date), N'Nữ', N'Kinh', N'Công giáo', N'Độc thân', N'123456804', N'8906005408', N'Viên chức HĐLV không xác định thời hạn', N'Giáo viên', N'Khoa học xã hội', 0, N'Đang làm việc', N'Sở nội vụ tỉnh Nam Định', CAST(N'2004-10-01' AS Date), CAST(N'2004-10-01' AS Date), CAST(N'2004-10-01' AS Date), N'Xóm Mỹ Hòa - Hải Giang - Hải Hậu - Nam Định', N'Tỉnh Nam Định')
GO
INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FullName], [DOB], [Gender], [Ethnicity], [Religion], [MaritalStatus], [IDCardNumber], [InsuranceNumber], [EmploymentType], [Position], [Department], [IsHeadOfDepartment], [EmploymentStatus], [RecruitmentAgency], [HiringDate], [PermanentEmploymentDate], [SchoolJoinDate], [PermanentAddress], [Hometown]) VALUES (16, 17, N'Lê Hồng Nhung', CAST(N'1991-09-02' AS Date), N'Nữ', N'Kinh', N'Không', N'Đã kết hôn', N'123456805', N'3614011806', N'Viên chức HĐLV không xác định thời hạn', N'Giáo viên', N'Khoa học xã hội', 0, N'Đang làm việc', N'UBND huyện Hải Hậu', CAST(N'2014-01-10' AS Date), CAST(N'2014-01-10' AS Date), CAST(N'2014-01-10' AS Date), N'Xóm Mỹ Tiến xã Hải Giang huyện Hải Hậu tỉnh Nam Định', N'Tỉnh Nam Định')
GO
INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FullName], [DOB], [Gender], [Ethnicity], [Religion], [MaritalStatus], [IDCardNumber], [InsuranceNumber], [EmploymentType], [Position], [Department], [IsHeadOfDepartment], [EmploymentStatus], [RecruitmentAgency], [HiringDate], [PermanentEmploymentDate], [SchoolJoinDate], [PermanentAddress], [Hometown]) VALUES (17, 18, N'Trần Thị Tuyết Nhung', CAST(N'1979-10-01' AS Date), N'Nữ', N'Kinh', N'Không', N'Đã kết hôn', N'123456806', N'3614012806', N'Viên chức HĐLV không xác định thời hạn', N'Giáo viên', N'Khoa học tự nhiên', 0, N'Đang làm việc', N'UBND Tỉnh Nam Định', CAST(N'2000-11-01' AS Date), CAST(N'2000-11-01' AS Date), CAST(N'2000-11-01' AS Date), N'Xóm 6 - Hải Toàn - Hải An - Hải Hậu - Nam Định', N'Tỉnh Nam Định')
GO
INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FullName], [DOB], [Gender], [Ethnicity], [Religion], [MaritalStatus], [IDCardNumber], [InsuranceNumber], [EmploymentType], [Position], [Department], [IsHeadOfDepartment], [EmploymentStatus], [RecruitmentAgency], [HiringDate], [PermanentEmploymentDate], [SchoolJoinDate], [PermanentAddress], [Hometown]) VALUES (18, 19, N'Đỗ Thị Thu', CAST(N'1989-07-01' AS Date), N'Nữ', N'Kinh', N'Công giáo', N'Đã kết hôn', N'123456807', N'3612011313', N'Viên chức HĐLV không xác định thời hạn', N'Giáo viên', N'Khoa học xã hội', 0, N'Đang làm việc', N'UBND huyện Hải Hậu', CAST(N'2012-09-17' AS Date), CAST(N'2012-09-17' AS Date), CAST(N'2015-08-25' AS Date), N'Xóm Mỹ Hòa xã Hải Giang huyện Hải Hậu tỉnh Nam Định', N'Tỉnh Nam Định')
GO
INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FullName], [DOB], [Gender], [Ethnicity], [Religion], [MaritalStatus], [IDCardNumber], [InsuranceNumber], [EmploymentType], [Position], [Department], [IsHeadOfDepartment], [EmploymentStatus], [RecruitmentAgency], [HiringDate], [PermanentEmploymentDate], [SchoolJoinDate], [PermanentAddress], [Hometown]) VALUES (19, 20, N'Phạm Thị Thuỷ', CAST(N'1980-05-16' AS Date), N'Nữ', N'Kinh', N'Không', N'Độc thân', N'123456808', N'8902006011', N'Viên chức HĐLV không xác định thời hạn', N'Giáo viên', N'Khoa học tự nhiên', 0, N'Đang làm việc', N'Ban tổ chức chính quyền tỉnh Nam Định', CAST(N'2001-09-15' AS Date), CAST(N'2001-09-15' AS Date), CAST(N'2001-09-15' AS Date), N'Xóm Mỹ Hòa - Hải Giang - Hải Hậu - Nam Định', N'Tỉnh Nam Định')
GO
INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FullName], [DOB], [Gender], [Ethnicity], [Religion], [MaritalStatus], [IDCardNumber], [InsuranceNumber], [EmploymentType], [Position], [Department], [IsHeadOfDepartment], [EmploymentStatus], [RecruitmentAgency], [HiringDate], [PermanentEmploymentDate], [SchoolJoinDate], [PermanentAddress], [Hometown]) VALUES (20, 21, N'Nguyễn Ngọc Trang', CAST(N'1979-10-08' AS Date), N'Nam', N'Kinh', N'Không', N'Đã kết hôn', N'123456809', N'8902036011', N'Viên chức HĐLV không xác định thời hạn', N'Giáo viên', N'Khoa học tự nhiên', 1, N'Đang làm việc', N'Ban Tổ chức chính quyền tỉnh Nam Đinh', CAST(N'2000-11-10' AS Date), CAST(N'2000-11-10' AS Date), CAST(N'2000-11-10' AS Date), N'xóm 4 Hải Toàn- xã Hải An- Hải Hậu', N'Tỉnh Nam Định')
GO
INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FullName], [DOB], [Gender], [Ethnicity], [Religion], [MaritalStatus], [IDCardNumber], [InsuranceNumber], [EmploymentType], [Position], [Department], [IsHeadOfDepartment], [EmploymentStatus], [RecruitmentAgency], [HiringDate], [PermanentEmploymentDate], [SchoolJoinDate], [PermanentAddress], [Hometown]) VALUES (21, 22, N'Bùi Văn Trang', CAST(N'1979-04-15' AS Date), N'Nam', N'Kinh', N'Công giáo', N'Đã kết hôn', N'123456810', N'8901004380', N'Viên chức HĐLV không xác định thời hạn', N'Giáo viên', N'Khoa học xã hội', 0, N'Đang làm việc', N'Ban Tổ chức CQ tỉnh Nam Định', CAST(N'1999-09-01' AS Date), CAST(N'1999-09-01' AS Date), CAST(N'1999-09-01' AS Date), N'Xóm Mỹ Hòa xã Hải Giang huyện Hải Hậu tỉnh Nam Định', N'Tỉnh Nam Định')
GO
INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FullName], [DOB], [Gender], [Ethnicity], [Religion], [MaritalStatus], [IDCardNumber], [InsuranceNumber], [EmploymentType], [Position], [Department], [IsHeadOfDepartment], [EmploymentStatus], [RecruitmentAgency], [HiringDate], [PermanentEmploymentDate], [SchoolJoinDate], [PermanentAddress], [Hometown]) VALUES (22, 384, N'Ngô Hoàng Kim', CAST(N'2025-04-03' AS Date), N'Nam', N'Ha Noi', N'Không', N'Độc thân', N'123456789111', N'BH161202', N'Hợp đồng lao động dưới 1 năm', N'Giáo viên', N'Khoa học xã hội', 1, N'Đã nghỉ việc', N'Trường THCS HGS', CAST(N'2025-04-03' AS Date), NULL, CAST(N'2025-04-03' AS Date), N'123 Đường Láng, Hà Nội', N'Ha Noi')
GO
INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FullName], [DOB], [Gender], [Ethnicity], [Religion], [MaritalStatus], [IDCardNumber], [InsuranceNumber], [EmploymentType], [Position], [Department], [IsHeadOfDepartment], [EmploymentStatus], [RecruitmentAgency], [HiringDate], [PermanentEmploymentDate], [SchoolJoinDate], [PermanentAddress], [Hometown]) VALUES (23, 385, N'Ngô Hoàng Kim', CAST(N'2025-04-05' AS Date), N'Nam', N'Ha Noi', N'Công giáo', N'Độc thân', N'010203040102', N'BH161202', N'Hợp đồng lao động dưới 1 năm', N'Giáo viên', N'Khoa học tự nhiên', 0, N'Đã nghỉ việc', N'', CAST(N'2025-04-02' AS Date), NULL, CAST(N'2025-04-05' AS Date), N'123 Đường Láng, Hà Nội', N'Ha Noi')
GO
INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FullName], [DOB], [Gender], [Ethnicity], [Religion], [MaritalStatus], [IDCardNumber], [InsuranceNumber], [EmploymentType], [Position], [Department], [IsHeadOfDepartment], [EmploymentStatus], [RecruitmentAgency], [HiringDate], [PermanentEmploymentDate], [SchoolJoinDate], [PermanentAddress], [Hometown]) VALUES (24, 386, N'Ngô Hoàng Kimfsadasaaaaa', CAST(N'2025-04-05' AS Date), N'Nam', N'Ha Noi', N'Không', N'Độc thân', N'123456789321', N'13', N'Hợp đồng lao động dưới 1 năm', N'Giáo viên', N'Khoa học xã hội', 0, N'Đang làm việc', N'', CAST(N'2025-04-11' AS Date), NULL, CAST(N'2025-04-10' AS Date), N'123 Đường Láng, Hà Nội', N'Ha Noi')
GO
INSERT [dbo].[Teachers] ([TeacherID], [UserID], [FullName], [DOB], [Gender], [Ethnicity], [Religion], [MaritalStatus], [IDCardNumber], [InsuranceNumber], [EmploymentType], [Position], [Department], [IsHeadOfDepartment], [EmploymentStatus], [RecruitmentAgency], [HiringDate], [PermanentEmploymentDate], [SchoolJoinDate], [PermanentAddress], [Hometown]) VALUES (26, 1, N'admin', CAST(N'2000-01-01' AS Date), N'Nam', N'xxx', N'không', N'Độc thân', N'123312888999', NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, CAST(N'2000-01-01' AS Date), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Teachers] OFF
GO
SET IDENTITY_INSERT [dbo].[TeacherSubjects] ON 
GO
INSERT [dbo].[TeacherSubjects] ([ID], [TeacherID], [SubjectID], [IsMainSubject]) VALUES (1, 1, 82, 1)
GO
INSERT [dbo].[TeacherSubjects] ([ID], [TeacherID], [SubjectID], [IsMainSubject]) VALUES (2, 1, 89, 0)
GO
INSERT [dbo].[TeacherSubjects] ([ID], [TeacherID], [SubjectID], [IsMainSubject]) VALUES (3, 2, 82, 1)
GO
INSERT [dbo].[TeacherSubjects] ([ID], [TeacherID], [SubjectID], [IsMainSubject]) VALUES (4, 2, 93, 0)
GO
INSERT [dbo].[TeacherSubjects] ([ID], [TeacherID], [SubjectID], [IsMainSubject]) VALUES (5, 2, 95, 0)
GO
INSERT [dbo].[TeacherSubjects] ([ID], [TeacherID], [SubjectID], [IsMainSubject]) VALUES (6, 3, 81, 1)
GO
INSERT [dbo].[TeacherSubjects] ([ID], [TeacherID], [SubjectID], [IsMainSubject]) VALUES (7, 4, 78, 1)
GO
INSERT [dbo].[TeacherSubjects] ([ID], [TeacherID], [SubjectID], [IsMainSubject]) VALUES (8, 5, 91, 1)
GO
INSERT [dbo].[TeacherSubjects] ([ID], [TeacherID], [SubjectID], [IsMainSubject]) VALUES (9, 6, 82, 0)
GO
INSERT [dbo].[TeacherSubjects] ([ID], [TeacherID], [SubjectID], [IsMainSubject]) VALUES (10, 7, 89, 0)
GO
INSERT [dbo].[TeacherSubjects] ([ID], [TeacherID], [SubjectID], [IsMainSubject]) VALUES (11, 8, 84, 0)
GO
INSERT [dbo].[TeacherSubjects] ([ID], [TeacherID], [SubjectID], [IsMainSubject]) VALUES (12, 9, 81, 0)
GO
INSERT [dbo].[TeacherSubjects] ([ID], [TeacherID], [SubjectID], [IsMainSubject]) VALUES (13, 10, 82, 0)
GO
INSERT [dbo].[TeacherSubjects] ([ID], [TeacherID], [SubjectID], [IsMainSubject]) VALUES (14, 11, 81, 0)
GO
INSERT [dbo].[TeacherSubjects] ([ID], [TeacherID], [SubjectID], [IsMainSubject]) VALUES (15, 12, 80, 1)
GO
INSERT [dbo].[TeacherSubjects] ([ID], [TeacherID], [SubjectID], [IsMainSubject]) VALUES (16, 12, 79, 0)
GO
INSERT [dbo].[TeacherSubjects] ([ID], [TeacherID], [SubjectID], [IsMainSubject]) VALUES (17, 13, 81, 0)
GO
INSERT [dbo].[TeacherSubjects] ([ID], [TeacherID], [SubjectID], [IsMainSubject]) VALUES (18, 14, 79, 0)
GO
INSERT [dbo].[TeacherSubjects] ([ID], [TeacherID], [SubjectID], [IsMainSubject]) VALUES (19, 15, 82, 0)
GO
INSERT [dbo].[TeacherSubjects] ([ID], [TeacherID], [SubjectID], [IsMainSubject]) VALUES (20, 16, 88, 0)
GO
INSERT [dbo].[TeacherSubjects] ([ID], [TeacherID], [SubjectID], [IsMainSubject]) VALUES (21, 17, 81, 0)
GO
INSERT [dbo].[TeacherSubjects] ([ID], [TeacherID], [SubjectID], [IsMainSubject]) VALUES (22, 18, 82, 0)
GO
INSERT [dbo].[TeacherSubjects] ([ID], [TeacherID], [SubjectID], [IsMainSubject]) VALUES (23, 19, 85, 0)
GO
INSERT [dbo].[TeacherSubjects] ([ID], [TeacherID], [SubjectID], [IsMainSubject]) VALUES (24, 20, 81, 0)
GO
INSERT [dbo].[TeacherSubjects] ([ID], [TeacherID], [SubjectID], [IsMainSubject]) VALUES (25, 21, 82, 0)
GO
INSERT [dbo].[TeacherSubjects] ([ID], [TeacherID], [SubjectID], [IsMainSubject]) VALUES (26, 5, 88, 0)
GO
SET IDENTITY_INSERT [dbo].[TeacherSubjects] OFF
GO
SET IDENTITY_INSERT [dbo].[TeachingAssignments] ON 
GO
INSERT [dbo].[TeachingAssignments] ([AssignmentID], [TeacherID], [SubjectID], [ClassID], [SemesterID]) VALUES (84, 1, 82, 13, 1)
GO
INSERT [dbo].[TeachingAssignments] ([AssignmentID], [TeacherID], [SubjectID], [ClassID], [SemesterID]) VALUES (85, 1, 82, 15, 1)
GO
INSERT [dbo].[TeachingAssignments] ([AssignmentID], [TeacherID], [SubjectID], [ClassID], [SemesterID]) VALUES (86, 1, 82, 14, 1)
GO
INSERT [dbo].[TeachingAssignments] ([AssignmentID], [TeacherID], [SubjectID], [ClassID], [SemesterID]) VALUES (87, 1, 89, 18, 1)
GO
INSERT [dbo].[TeachingAssignments] ([AssignmentID], [TeacherID], [SubjectID], [ClassID], [SemesterID]) VALUES (88, 2, 95, 20, 1)
GO
INSERT [dbo].[TeachingAssignments] ([AssignmentID], [TeacherID], [SubjectID], [ClassID], [SemesterID]) VALUES (89, 2, 95, 21, 1)
GO
SET IDENTITY_INSERT [dbo].[TeachingAssignments] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (1, N'admin', N'$2a$11$FaIzK4B0orQaEBC4ri8LzuYrCGwQfYbq2Vp.7pb96nCRiiaxmD1VK', N'admin@hgs.edu.vn', N'0901234567', 4, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (2, N'anhvth1', N'$2a$11$FaIzK4B0orQaEBC4ri8LzuYrCGwQfYbq2Vp.7pb96nCRiiaxmD1VK', N'anhngoc24062000@gmail.com', N'0373405863', 5, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (3, N'anhttu1', N'$2a$11$FaIzK4B0orQaEBC4ri8LzuYrCGwQfYbq2Vp.7pb96nCRiiaxmD1VK', N'tuananhgv77@gmail.com', N'0985836971', 5, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (4, N'duyenpth1', N'$2a$11$W959.SpxpGcg.6vn2vuhteuW1hjaU//PndSEyBvc6gBa55axGPqFK', N'36366512022@gmail.com', N'0914877368', 5, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (5, N'doanpco1', N'$2a$11$wkg1SfmJb5rXpNcVhBagbuojsTBSfVz6Db.Tb/3HDc46qZwPwN5Qy', N'phamcongdoan5@gmail.com', N'0834622538', 5, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (6, N'duomnth1', N'$2a$11$BsEqdioVhqPVfggPo1O7Kuwujs/xj0ATWhoDOrfZqz3EmnDyLQTbW', N'nguyenthiduom1984@gmail.com', N'0977470375', 5, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (7, N'haipth1', N'$2a$11$3Ja8De2KwyuJLdUtw3P4Y.ZFDeTo5IIWxskhp2uuTohyouq/3PAfW', N'phamthihai1982hg@gmail.com', N'0857124936', 5, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (8, N'hanglth1', N'$2a$11$9K9OHgVkYXmzpm.OxHq0CexnfXScKBhD.VduuolImZCwzaJ3CGpuC', N'Hangphuongbch@gmail.com', N'0917824977', 5, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (9, N'hiennth1', N'$2a$11$Wp2pHprPC9afns3ymugDKOfEW1pLNe89QiZhda02Qz7sTFPfFKO8K', N'nguyenhienthcshaigiang@gmail.com', N'0967523327', 5, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (10, N'hoaivth1', N'$2a$11$xzR0txYBSWjgEtZexPiaEOCtDJ7B./9btobP0STyvw.WL18O6teHO', N'vuhoaihaigiang@gmail.com', N'0947945228', 5, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (11, N'huevth1', N'$2a$11$2E0S/Hgyh/97vHNUoXdcEuShTm8gthR5r/65UtToP6Jc0zFlgmZR6', N'vuhuehaigiang@gmail.com', N'0906657796', 5, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (12, N'huyennth1', N'$2a$11$Bd7aCzV7AzDJWVKfpOzp5uaHnYwX2fFf/1E/u5bmm83qQkmbutA76', N'nguyenthihuyenhaian@gmail.com', N'0988886828', 5, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (13, N'lantth1', N'$2a$11$elarj.zQTFWjTTwjzbfWf.iQjkNyH49u4ILA1Yjb0fhCSyKrG0gQ.', N'lamtranngocanh2017@gmail.com', N'0974559178', 5, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (14, N'luannhu1', N'$2a$11$Q9E6DIdjxRFowYVeADdhNORWsx9vs4sYY2HPb/DP0/mzLlVaIud4K', N'chanbetgame7773@gmail.com', N'0972461837', 5, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (15, N'luongvvi1', N'$2a$11$YU3py528jMOvm/d46.uTX.d0K9C1nkml5xY48dQEprR64jAxMwe/y', N'vietluong.71@gmail.com', N'972461838', 5, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (16, N'nguyettth1', N'$2a$11$m4SGsvHv/1hjR8/.KOKi6OOliwjOCzR1hMGlIz5FdRdfsg61yN6Ca', N'nguyetthcshaigiang@gmail.com', N'0357539288', 5, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (17, N'nhunglho1', N'$2a$11$oNhKpnLrYXPAlf8zytyRe.0dSthm2Bf.NzLtG6CMJzPqry26t4yju', N'hongnhungsptphcm@gmail.com', N'0356187521', 5, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (18, N'nhungtth1', N'$2a$11$/firEv31WwvsjwXUx/ngqezKLgU.phBW1Tc0Y/doMVJ8cMDs2wJe2', N'nhunghaitoan@gmail.com', N'0987678162', 5, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (19, N'thudth1', N'$2a$11$xWmIHNrdKEOZQweP53SaRuE5zvns17g4F6IFItgzYKV6KTJkqmvuO', N'dothu8983@gmail.com', N'0989762305', 5, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (20, N'thuypth1', N'$2a$11$I0RJhjw5i8yvFu966ZnLrOrs2pq6rEydBwkOfMZsOKwLzTdZUaIwO', N'thuy1980pham@gmail.com', N'0964670817', 5, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (21, N'trangnng1', N'$2a$11$TcGX4sJO0pqaUjYKB3/PP.CESG/eRdc2tsjgJrltUnThg4tNZnmK6', N'ntrang79@gmail.com', N'0977442719', 5, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (22, N'trangbva1', N'$2a$11$hHjMqgf/zyCIFXYp.MZeoOysmvsu9Gs0Jl0hMzAEqLte.ycOYUMba', N'buivantrang1541979@gmail.com', N'0973126848', 5, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (23, N'duyvva23', N'$2a$11$uzcKWK4eFPtJsgnztjU4KOouK9iP8F0pOJSCfuXJX42dRn7Pw.hlC', N'', N'0463733021', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (24, N'thaonva24', N'$2a$11$TvaJvQLqI/83t3TscM3hmO7NDGZD4EOcOjhGS9XsPY0uUs8o0FRlq', N'', N'0338733875', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (25, N'tuyentva25', N'$2a$11$Um3ycXnbABtfDAiA6d2cHOdQxP6/mhJEx6PQGfabmZve66/mHPH42', N'', N'0460990840', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (26, N'duongpva26', N'$2a$11$smWeHdVaJFbalgiPrz/hv.Ce3NnIJfe0o59KMt37srmVRSLsXZrHO', N'', N'0346191382', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (27, N'vietnva27', N'$2a$11$OCIzA2oN6lwz.Rr.8lnmlOBNKfPDdy/pwRifHvwbLHq8i9LG29ely', N'', N'0553425449', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (28, N'bacnva28', N'$2a$11$G8g0faS/yoYq1RwBas4svudlo/VTOvnvAmsqWdlpDxm3G1YAvdpE.', N'', N'0286966168', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (29, N'duynva29', N'$2a$11$7tBDSUBneFQSDchIQ2EG2.7VNR3f9GKwPtRYTOuYZ9Wv2Yllvgi8i', N'', N'0189365249', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (30, N'hoangnva30', N'$2a$11$9MlT7tOju8C1TlIEUnXvPuU9UeNdCuBaQDlMd270rtpIG9JyCzsyW', N'', N'0866854286', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (31, N'hanhvva31', N'$2a$11$qGDscgTvepgk3reENLcdy..q6NsDrXwOEDDTuo6Puw7LVV6mHFLee', N'', N'0999078339', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (32, N'donglva32', N'$2a$11$8CAe461ezId55COPnstY2e9AiHNlgMsgZDX343JfhJ.n3bHAVEwxW', N'', N'0352082693', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (33, N'thuyenpva33', N'$2a$11$SRxoDnQJBdyCV/jVrXJF5.O8nl/eQ0DN7GV3dUl0rNbjROeTo8cFG', N'', N'0512790089', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (34, N'hungnva34', N'$2a$11$Kh7O2Mek37NorbMwspoQhuIcK.KCBU9CkTAvs6XxrUEpS7R6HICBi', N'', N'0365256524', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (35, N'giangnva35', N'$2a$11$0vKR/fg1BfqR7zQmyyYLiOz/GAaIy4egPfwzTJ2M2Mfnuny0pFudK', N'', N'0422841352', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (36, N'diepnva36', N'$2a$11$2h.lbEu6Lvv.F1piYMCIIOmnIRxXJOTlfV0yaPOEUaSFGuhnNdlYO', N'', N'0448884475', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (37, N'trieuvqu37', N'$2a$11$98aaAq9rW9d4zERfMN5scuiD1eANZ18vGBXKdXhLE13CxWjMwiIfS', N'', N'0843112283', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (38, N'luyennva38', N'$2a$11$hELKS0aSAiENEnFvqqWeHOPt.FSeAVsj31tkOe5nVVFYe4MSUaIhy', N'', N'0437107324', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (39, N'thanhnva39', N'$2a$11$t/ull6FG13ZUJQJz.Uq05u9ikBqh8VWvZDbd9Hlq5ur0sZiXWkagy', N'', N'0843836802', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (40, N'thanhnva40', N'$2a$11$1fyl5KvRFniN4Whpqwrx8OY1Qnh6ALzy4MPLhDmWJWWgHKk7lS/L.', N'', N'0201254570', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (41, N'dungnva41', N'$2a$11$3fOFt8YL4rGCsxVp0GPqzuFnOcB9INcLuR8IWVoHcloVaVXe9d6L.', N'', N'0564932662', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (42, N'duylxu42', N'$2a$11$sJIjVV3skTG.ojxgepgxY.b.aD8ZRUTqEP5bQjlRXDwMSwMaz4V0y', N'', N'0959854197', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (43, N'quangvva43', N'$2a$11$b7rqz8nYp1ZYnbrUoysk5eHbMqdrc0/ryC.FhXbGRELfZgMkRdSvy', N'', N'0435972160', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (44, N'tatmi44', N'$2a$11$9OeWFGSF666TM4T0bA20JuxrAHzoYmBUMkYfNYaSe89oOFaehwts6', N'', N'0868720567', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (45, N'hiennva45', N'$2a$11$DulXZ6MlInw.QEk.nKgxOew/PsXk1mjmcmxTR2enA7YO/DNeStJ/e', N'', N'0455918461', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (46, N'toibva46', N'$2a$11$LjaPywLw2CcrKM51Yx1EuOpkU.rAMsNMLymbiKDYzc4F08GpTIjNu', N'', N'0331745433', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (47, N'gioinva47', N'$2a$11$CBKEWuFbYc.CyFglNamD.e/ZCR3PL15bs4VY.bM/AiRhh/.TJ9ISi', N'', N'0910752552', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (48, N'luannhu48', N'$2a$11$RDztHv0xLffLhV9K/tVrROnt0N6PJrBBYWaHj9AtZTEThHXxYuzWy', N'', N'0298017084', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (49, N'thuynva49', N'$2a$11$7aWR/ksvBAHVzxbPbV0QKuZBYCamFEgq6cbls/1zquJdFXbt/ky0e', N'', N'0732115823', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (50, N'chungnth50', N'$2a$11$wm1z7PkJz2g0uZ3JpEQF8uHU8VlOlCnpMJ0yYTuHlrkns7W7Bo.Im', N'', N'0435892605', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (51, N'phuocthu51', N'$2a$11$pXDcEH2/.St5xet654rxYuO1eAd1eNkAWXiGK0sWNzgDqhk8t4/ne', N'', N'0725093263', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (52, N'trieupva52', N'$2a$11$J0.SlPO.CH7IMSMdc5Ff5eR9mAf6H7k5AiozxcA9ke2UtptyO9uTy', N'', N'0699648272', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (53, N'thanhdva53', N'$2a$11$5HLO.kwN8Hdi7yzZUw16qOTzoQJ1/E/TjA2wChv2Efg9kaLEX0ccy', N'', N'0218262296', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (54, N'giangbva54', N'$2a$11$ycErmDwXwC8s6B8bR8L3f.Dz9xsku54LGzBjTk1Zu4stFcHMCv4Ka', N'', N'0129333066', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (55, N'annth55', N'$2a$11$dYQpz..CFEklBlFjcu7nLuqfonk6HwYoO.h3xM/6P5bY6Ha7mLWcq', N'', N'0214132231', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (56, N'vienpva56', N'$2a$11$8QaWH9ikhSLJHaE6cichYuBw1Y5Wx8Fbzy5F1cC.ozUItGxUlCpIm', N'', N'0449450314', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (57, N'thaitva57', N'$2a$11$aAFBUovVVmlyfq638.xT8uYT7uA1XpIaLRid82Pa9T1iu04O0dQ5u', N'', N'0243099337', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (58, N'phucvva58', N'$2a$11$If8Hmr36EA7jHItCffjlH.g.xrYOs/6i/el1rgGfCAJfd7Hv2KWSy', N'', N'0657092452', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (59, N'toipva59', N'$2a$11$aKndvzjds5xwn7ukJN2l1OpabaoTagG8WNDMcJI9r8/uV3k/zSfDC', N'', N'0649042552', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (60, N'thienbva60', N'$2a$11$rRdcN0Bc6tGRUOkolW6hXuO.StmdDpbHgcwf2T/3yO03g/eBYu.b2', N'', N'0515153920', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (61, N'quanpva61', N'$2a$11$vEgewVv2/j.YYQLen83BB.EFc850MKD1dbL7EX7jDmfRKtuZoRXAi', N'', N'0659684807', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (62, N'thanhnva62', N'$2a$11$ami8YLUEm5MeYLnF4QKmQ.gwNgd0dT4c9IT7Jmufp7KUdkySoj...', N'', N'0491247177', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (63, N'truongvva63', N'$2a$11$gJWEmUCcSJ/PASIoXn7RKew5w1a/TXlX.n66DpHq8vZJ/ednYTkIK', N'', N'0819844979', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (64, N'dieptva64', N'$2a$11$xIbndrkUzGTKfPS0TcZzceRDu2uff/p1JNAuYI55ji4jwXBD7pCZ6', N'', N'0307946836', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (65, N'sinhpva65', N'$2a$11$aPZ2ewIOMduInSXj1yW1huu9GfFLEzJi.fKMEA2G4I5RvXK8gET/K', N'', N'0590705472', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (66, N'vietvdi66', N'$2a$11$kNvWBaFGxkVOK3sQ3Ubwnu29A.SO5vugMok32Y2zvkdOpe6SZ4Dem', N'', N'0294986939', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (67, N'sinhvkh67', N'$2a$11$G32j5YpTK/RU6FuOgbc8..79Kvm.l0yFg9wFXSPSzW8ssTwYRlg12', N'', N'0143220418', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (68, N'tungnva68', N'$2a$11$FaWIF9v2KVaNxa.4X5N0j.si5iblqGzIZBKdw6bDt8lVbdBvW4TRq', N'', N'0959036338', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (69, N'hanhvdi69', N'$2a$11$tXS0bG4OZQ60zE2F25GfnuEW2SF5O.UklItXSsgjOcA7Bpn1ANK9u', N'', N'0298789507', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (70, N'tuelva70', N'$2a$11$HHklqmTIJFCKbZG95pUDye5zrdZqpTVtMUM89XQp5eD5aouq.KTo6', N'', N'0911677217', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (71, N'chinhtva71', N'$2a$11$5oxtIydIS8s/0V4vc0lNBOxLydKwq9mijSDX5LOgwJM/9Jivms9/C', N'', N'0195322436', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (72, N'thaidva72', N'$2a$11$SpKSVKQtRW7ZWyvkT.DL.OA7IEq0gfHRCIyAoeNP9FrCL67xZLABO', N'', N'0721211731', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (73, N'dannho73', N'$2a$11$7vIdAorUBYU3BqSgaFTn.eodWWG0L6qvTGyO5C.T5wjoiC8B5O/qm', N'', N'0581818991', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (74, N'trinhnqu74', N'$2a$11$pH5WM5ewQTIdm4kuw1v.i.NrBfz1a6/E43Qc.ZKsmy7sP.oxRyFKq', N'', N'0678921771', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (75, N'hoppva75', N'$2a$11$vJftGFPgAhLqjYXKbaKideSkj1nnIiYSa1hYteAZYzqPk3LW3f/fy', N'', N'0169589751', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (76, N'bactva76', N'$2a$11$gPvkMShpG3/cjW3KCMtcau.nKDctNLPiWehSZbMFmRfJIXysIiB8S', N'', N'0495406854', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (77, N'dunva77', N'$2a$11$gJRjOlU9FiKGPecYWkbEm.pONw17ELM0waEHMeHyCEPZMRIIQsxxu', N'', N'0593242412', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (78, N'luyenpva78', N'$2a$11$0E9Ee1cB9Vtj2N3kLkuPSOWIObKOAkHCKLMMp5eUU26ICaBH1QRyW', N'', N'0167625141', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (79, N'dongnva79', N'$2a$11$nZqiUsH.ViHOpwi03mfzuOUU4RgRi9aj3qU30jp0D141HxYBCLwgW', N'', N'0659558743', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (80, N'tiennva80', N'$2a$11$GMlu.KyhkMvoqkpSKJnpXu6CnePztDVKjNhARY3WrDS3NQYa6gN9m', N'', N'0582262575', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (81, N'truongmva81', N'$2a$11$LFn.S5CN.DvobxnV73ypt.SG2A0gTAPWxFNb6OovPmkBxvEiqgmba', N'', N'0387387162', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (82, N'binhnva82', N'$2a$11$TT98v/aN2PpzptmFtFyv0OrJkp3KeADFY2nU0sDBYmxRGlzNds4Ba', N'', N'0846055150', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (83, N'thoanvdi83', N'$2a$11$1NoTt6x4rtrJ8R2G4rCHKuLFmWujym9OGHZTJfK0ISyaY4.e/QD8u', N'', N'0466675943', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (84, N'sangnva84', N'$2a$11$2FadReF8ug28sUiF2BPKauzrVIvKxdyU.KlLKKEsP6MwUuGpx.GMy', N'', N'0358689296', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (85, N'henva85', N'$2a$11$81x0PjOI5yEBGdmzqtD6HOEYsWFezMY7.V6//UfiZkx5u0vDNs8tS', N'', N'0989772039', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (86, N'manhbng86', N'$2a$11$Q1OeID616ocS4fOJFwO2UuhXudoUaiyrukSRKAi8zZlfOBQHJVvHW', N'', N'0368905401', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (87, N'quynhpva87', N'$2a$11$yl9/UwnhHms9HMxUzTQ7h.Pcp04dh4PPjNsQuMQjR5EqtL8Fb9RA6', N'', N'0833110541', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (88, N'minhlva88', N'$2a$11$wa9HXybvVa9mm0mB0KLlB.jquIXdFMbWLl5ZXoE9QWonDib98W1AO', N'', N'0879429447', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (89, N'thucnva89', N'$2a$11$c/2Hd0SP8SCJmgWwiOConup0XBlr6X4jHnuSBvJNUkVzju7VRz/by', N'', N'0192419749', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (90, N'chungtva90', N'$2a$11$0WPWxczP3VrGIpez.13jmeBnsdu.deiBrol3r8epuW0GifZg59Vi6', N'', N'0691357779', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (91, N'thinhnva91', N'$2a$11$sa3jMrVTxNw0gNetkInG..WG2AfHFsWXRtM2kjnBfIwv6bCPnbF3G', N'', N'0899566882', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (92, N'damlva92', N'$2a$11$iuK3uCcRVsYw6vdITbKjuOOSghRJbLkENggwu.dOwTmzlTOFFHgQW', N'', N'0503619992', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (93, N'tiendva93', N'$2a$11$lY7RXkK7JPa2psBGd89loeJWoFEbsRaF7Oc0RY3/EebjnRcDUtdMK', N'', N'0274169403', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (94, N'thuannva94', N'$2a$11$wpl8twgg0CvKGvaUciaN5OQi87eDpT6OTLt1PiyV1.IyhO82o21Je', N'', N'0208144950', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (95, N'thuanvva95', N'$2a$11$utOIuIRCHqTq8B06FngEVeJLMjcrviFUZ1b0WQrCDBvF9fWJlgBEO', N'', N'0588096970', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (96, N'viennva96', N'$2a$11$xztvyws/iUf.I/En5OpHu.Ms8TqDudU0.3RA8rwBBcqeGObFJu2Li', N'', N'0646354925', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (97, N'duyvva97', N'$2a$11$KkaMqV/lhYKy7Sx83bB4p.wZzA5Iqxp1F8Uv40ueGOUVjTzeulCYO', N'', N'0645989018', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (98, N'thieuvva98', N'$2a$11$4IeX36J0Sq67GuIzKeG2QOmm6ajP6wK2WNZKbTCav7e38bHTviaOa', N'', N'0517612862', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (99, N'dinhdva99', N'$2a$11$AW2J9LuRwaWUvpXoL75GnuSXJwGuwqQq9wpKIMsQ8r1WtKbTf8Q9O', N'', N'0109912961', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (100, N'anhva100', N'$2a$11$SLIUHCrp.B9Y8ptZA.g4xOmkrXXT40Wdmlmcr6VESm251Lc.KNzCS', N'', N'0955247771', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (101, N'tondki101', N'$2a$11$QKvcscH5pVKMsD2aGOgvfOlZo.b6cWndYqJNodP3c5HXE1r2gAuYG', N'', N'0621288019', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (102, N'thinhpva102', N'$2a$11$oB0.JAIiBxNX26kHnP0zXuGzWcU2lgxQJEWom4RiNCbminiahH9Ta', N'', N'0810783243', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (103, N'cuongnva103', N'$2a$11$PUd3nyC/p7BiNVbaLQVbSOcqxRtpqOMBfvTUSCHEbng64iBmFXN8a', N'', N'0598199671', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (104, N'binhnva104', N'$2a$11$APdvczJhFKjc4wNI/UH3TeATJrlailuCknLcXqCM06OaxVPQ92u0y', N'', N'0111994087', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (105, N'hanhnva105', N'$2a$11$L7HKN6Ui/gLxeY4ideY93ek0JodhChItBvX3INGy8mYSC0b0isdJq', N'', N'0611229735', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (106, N'hoahva106', N'$2a$11$c1GAeINdJROsvCoc.gxhjO2gqMxMT7OXa1kZc1YxcG3CB1trlRsS6', N'', N'0585416102', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (107, N'tulva107', N'$2a$11$SuUI8sKfhO277fmo1fMyJeCK5IwbZqce9SmBAlBOCwPDQzKMTIcC2', N'', N'0465927070', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (108, N'sangnqu108', N'$2a$11$16HBgcijGFd2UhxmTi68de0HyzTRrw9nEo4lskv6glSOJvAgVHuRG', N'', N'0458933961', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (109, N'haibva109', N'$2a$11$0zRZR71BgIQSEmFkJAe/P.DeH7b6qdf026jxD9bEvkkZY7/DHoBkW', N'', N'0845043903', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (110, N'locnva110', N'$2a$11$jj7tv9EXTJcvBk875m2Jc.BbqNegeTcylNW2m4fK2WsnXbKVB1OSS', N'', N'0785072231', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (111, N'tienpva111', N'$2a$11$80xiHTW1ketu2lCag2qSD.wbud9rOG6KIMUJOllh/1lOKaGPqlQu6', N'', N'0988662785', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (112, N'minhnva112', N'$2a$11$u0ZqgVG5UZHGcLcmhbZfZuFWf7I9urQNqSIaaXTB9eHTLVhvtWD8S', N'', N'0250614511', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (113, N'tuannva113', N'$2a$11$yEVKn2U3l7./CrTHo5nxkezVU5r/tkmEa/TcEocdN0cg0T1WyGWcu', N'', N'0315339165', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (114, N'thuongvva114', N'$2a$11$7Rc7pTQs8UW7DQSh.Q2rju1JFrnymvyDMigB3kDyHEHx/5/7ChGsq', N'', N'0663175702', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (115, N'thanhtva115', N'$2a$11$f1BTmFO0D7J76m4HIPJByuR64dQFQE5EE1XUtQj0vnACHzNs9Gbs6', N'', N'0412384599', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (116, N'hanhnva116', N'$2a$11$63o.u0gxO7xzsf1Ls5G01O4z1Lre9fIIQax4cQE987uus2Ig42f/y', N'', N'0530352389', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (117, N'hungtva117', N'$2a$11$WgukN9zWGYgfkMD2pKM.iuWQ0vyXhrAAFN6itLrvw6eqYRTV98CxK', N'', N'0448415571', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (118, N'tungnva118', N'$2a$11$Io0YvK8xYsa/cU1ivlSwtuPphMyOeQI2w5vhVWjWo3hx5HFKhUuja', N'', N'0272076368', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (119, N'banva119', N'$2a$11$S5UpJCPbIF0sEwddJlNfhufuyDmI0xoXAuS9JXHruxXmkYsgG.gQy', N'', N'0886292952', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (120, N'tuanpva120', N'$2a$11$ZyzIQpw4LtfBSO7Z5B5n.u5.Hq4mwlJ.pORttelTlYV63zCzUhZua', N'', N'0531796276', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (121, N'dinhnva121', N'$2a$11$UjhgLFznOGgN5kAX053/JuZqYE2HSMytaCYiJzt6Zb/3AqR2cS84K', N'', N'0899577075', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (122, N'cuongvva122', N'$2a$11$s.hUasx6bHB2WLYESOmVi.NdFw1AAf9Pv/KuuguWkTezStGzEuamm', N'', N'0477112364', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (123, N'hanhnph123', N'$2a$11$l6uD.l8Ns5Gg7Znn471uJOdz1hN3MuN5RqkV4s7ghTJBUn/Mf/g72', N'', N'0320623439', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (124, N'hopnva124', N'$2a$11$s2WhOksY2/EH5j0SqTB8jewZ1sSpS2iq5MAOpWRR7OwZ/aMsrwnEe', N'', N'0505489385', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (125, N'giangnva125', N'$2a$11$04rAN0/.xiHs/mDoTXMmt.q8LXZINy4k3rhQhcJFZrNLKhNGq98Ya', N'', N'0973444038', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (126, N'tannma126', N'$2a$11$t3HQ3v0oliza8XdYdrZjtuCwpMy27zInNfIquGRTXHTlSUU5Ny0g.', N'', N'0495672607', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (127, N'nhanva127', N'$2a$11$OFIFFk3e32erGISf/k.Ffe/iry9dUuLVQHozo0.TFDme/eUXo9Rtq', N'', N'0669459313', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (128, N'quangnth128', N'$2a$11$SkMHX1eMS1BbedzFb//bsu7vJEJw/NN5PYfgOE2pU9hsOoU.VIGj6', N'', N'0270431959', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (129, N'lamnva129', N'$2a$11$6.Sd0l96flBpnZ.5mImEHOv3c5PpqRzYjeXOEA0voVeddrJHLKGMO', N'', N'0369265729', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (130, N'hopvva130', N'$2a$11$UuWxXbH4YhtQZ2znOqSJ8eiavV1kEnMLk.RXNsAmqF4gStF4MjLdK', N'', N'0161259293', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (131, N'chinhmva131', N'$2a$11$S6/egf0WYLi0.2ErD64txegfWhgXIPe4rK/lMHLgAOPd4ITA34ZnW', N'', N'0238543981', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (132, N'tuant132', N'$2a$11$Fox2F4sad2VqOcvFIBcnhuPnfn16wWblXo4oGvfZTIAJWW5CRbGGK', N'', N'0690643775', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (133, N'tuandma133', N'$2a$11$INsVw5xDG5RQVI0YiT1dh.DNRQMGh79L3WtY0Z3OVn5JNV0QQu0mm', N'', N'0151232820', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (134, N'chivva134', N'$2a$11$RKsk6fqU86psfidcEdx8Vu42VCmzD6R4o3ZbZgJVmou.Ws6bxFgxS', N'', N'0612550401', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (135, N'haitva135', N'$2a$11$41IJ8hOh12iGZu1Q5Tj59eW9fKCqi6VEuLqEFT88W0V195ItPOTOO', N'', N'0618093508', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (136, N'doanhmva136', N'$2a$11$vDqMQTyW/rLQ/Dzs3j.mM.Wh/inXaJaEdURY.jSnW7rdM6qcX161u', N'', N'0876726639', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (137, N'thanhvva137', N'$2a$11$W5zEKX/ss4NOTDNs8R7VGewi04fqMucDzX2h.bBnZ8szp3YpJ6FZe', N'', N'0343789893', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (138, N'thoivan138', N'$2a$11$fJeE1fgTDS7pDTEZ7j1R4.U.Nh1s33pzEgvBEODFk.mAhAg21uZtG', N'', N'0707462191', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (139, N'datdva139', N'$2a$11$nFCg87RSq.DMVm0aX0TbtuMiUUFkwO6j/AWKZCyOXkTRDGdELJyD2', N'', N'0340609115', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (140, N'tangtva140', N'$2a$11$fxI8XHudnrvIMMD3HpWCDeUaeTpodfdgR7D6iWQwXj.Vtc6BJjOZe', N'', N'0112426888', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (141, N'ngoctva141', N'$2a$11$iPMSxlEndYtpOZMAf8f8KONmC36Qdmf0FgB1ZMEFdWFgyvqlDrRyu', N'', N'0451947933', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (142, N'quyetnva142', N'$2a$11$zmwGzZEYPoL7xSaTKPxCwu9RaWXNOPNVLgDiz6/iGetOZfqBUYdb.', N'', N'0862211704', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (143, N'duynva143', N'$2a$11$63DAZNdEEwK3n3S3GdGxb.y19KZnLFxDb8xzikpkML5Xnvz1hDLPu', N'', N'0463689678', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (144, N'sangnva144', N'$2a$11$RUaynkk1fgWwMR0XCYoYduu74IyOd8YN9BvWKkGyd34cC4lJHT4VC', N'', N'0906197893', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (145, N'tantva145', N'$2a$11$w4UdBd051qQfkyxh9bsHCOO44rUV/ax1TqfQdFOTlJWgZ36eq1bY2', N'', N'0206596833', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (146, N'chieudva146', N'$2a$11$WVys0WctE0i6PTJIWhcK8eym4Bg/usJXMPBaq8r4vijJ8GHXoONuy', N'', N'0815379261', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (147, N'ngonva147', N'$2a$11$uV9pVGGInBdk7nenMkzopOFeQ3./El4arKS1/A2GR9vqud.v99GjC', N'', N'0477844232', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (148, N'chieuvva148', N'$2a$11$6iD53x7honHjBBR4y95NnOfCWmFBLVlXpD/TK5Ghz7IAIAJZO4F4e', N'', N'0567762553', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (149, N'luyenvdi149', N'$2a$11$fZai/VWANT0UEXDZz12hnufF/RPo4yZ7HDFT5tVa.wtvc7SOuKqO6', N'', N'0534817892', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (150, N'tinhvva150', N'$2a$11$sYSIc7hlE4rxVSTJcMwspOkQyJJg3lJVlmJJsuFyBTQJYcz8a0EHC', N'', N'0316970968', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (151, N'duongnva151', N'$2a$11$9t1xygCzk21kAELdBafcieuaQbUGCvZtCAQMrjkKWXX4vjXFREvX.', N'', N'0739304465', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (152, N'thuannva152', N'$2a$11$mUSlWOVZhsLNz9j3pG14S.0YCqIi7BiYQe3cZ6.rClRzcnIc0zAqu', N'', N'0109675800', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (153, N'kettva153', N'$2a$11$i31kvWOxbl4OX7QTcg7nW.a57xpiz28fvS5j8ONNRgCygJxnqyp5a', N'', N'0255196315', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (154, N'dongvva154', N'$2a$11$VSouU.ZyS55oRpNopV/S4uW7oqWHRy7zgEP5kOvLoy/t1uXO6IosS', N'', N'0942481207', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (155, N'luongvva155', N'$2a$11$HKUx3aruLDI3Rr2w23p4cOtqK0a4DMr4PJW6qL8kAAS7Sh.3cvhX6', N'', N'0627837222', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (156, N'habva156', N'$2a$11$AlOdjeZdNCnnouykK2JpFuOlQKbjVJAmji2kyi.qva60qsxAXUvXy', N'', N'0773887097', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (157, N'dongnva157', N'$2a$11$FMU.dMQjviRCiWlTlz7JM.58bP9aAfshYkNrnbsTn30x5IreC6Iiu', N'', N'0198133713', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (158, N'chinhvva158', N'$2a$11$OaeWwZR9gsmj.BB/arr7UOpFIeyTRsCVgmX0TpICdOCdwEphQe4Va', N'', N'0931955146', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (159, N'thanhtvi159', N'$2a$11$qLlkKLvdNa/d/7ty.xYcT.SOXBwHeLbSAGBJoShZfAzy34be60v1C', N'', N'0529557007', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (160, N'luyenpva160', N'$2a$11$8XBpNE7NLnKq7uQiOBHsuOJNaKfemNhUWkLfM6lTncqWIhS9qABgO', N'', N'0783302938', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (161, N'chinhtva161', N'$2a$11$.FRv4PugTzLHcF8Ts8tYbejT4flA5ZRtMkB82RNf278zp9ia/TV7K', N'', N'0351160603', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (162, N'hungnva162', N'$2a$11$rx1HYy/wimIaQfhSjpXQv.A9Z2c8/Yda03WcldVDnm6VR1ekSU/TC', N'', N'0154051232', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (163, N'thucpva163', N'$2a$11$IKZirF9nmh4h5npT7/IZSuy4HDC.j/mYX48qCd5IQeW3GS9b.koBW', N'', N'0444738477', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (164, N'hoangnva164', N'$2a$11$rzigeXpsfUlV01Vzxn4lfOOE2CvySrRQRKpKqS1a0OruosWd02Qg.', N'', N'0418349349', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (165, N'ninhlva165', N'$2a$11$F1Zi.5CQM89s9T4aofOmbes/rONtIPpFfnK/GVg21YZGBtCbMFGjG', N'', N'0829248911', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (166, N'hopnva166', N'$2a$11$uGdE0OzqGO4ER1g6Vs7S1OS15vN9UNmnIR666AjAdElUxm1A9Pse2', N'', N'0795103693', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (167, N'thuyenvdi167', N'$2a$11$XH63M6fFZyHwtfIY6Z3NgefAlwfk56OGC87AXPqdMdrVm3XUTTSiW', N'', N'0620628947', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (168, N'dampva168', N'$2a$11$O5XNmhrLRmod5NJ4jaT45OYdZz/BlvEjUgwOyjQNFECeXfuWRU7ZK', N'', N'0118534386', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (169, N'haolva169', N'$2a$11$Mz9E70VqUqIbDUjfx2gmNefUKlGlgY5rMq9XLFyzTgiz8pFKUxr5G', N'', N'0370124465', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (170, N'hiennva170', N'$2a$11$rHc451Nu.8t6QtTBWUHVVOdpWpKRtdoh5TV8Dc9bM7P2Z501mYBs.', N'', N'0173185038', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (171, N'damvva171', N'$2a$11$iPDyrdFNsctJFEfg0BFO8.GaKkkkoh.an65sj0jFfESARlO2Sm1V2', N'', N'0609260886', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (172, N'giangnva172', N'$2a$11$HtNZGLkrbc47UXt72Qxrt.sw/b8LDPuyBJ7YfkWfdalwu2weIGn.K', N'', N'0609010636', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (173, N'huongtng173', N'$2a$11$nTeC3kxOwZ2iuQTgAfKqFOt3dBAyVyTOcPfOkSUx4eQjAfNYvFx6m', N'', N'0454579502', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (174, N'bonvva174', N'$2a$11$RYSNptiwf5G4/0Cq4SMAzuItMc1cfpEJMaTcXxjXnY1fKtzxAMYbm', N'', N'0877168560', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (175, N'thangnva175', N'$2a$11$KqAJe/zlkCXz9pU8dx9Ope8sbmT.CD5FweKoB6bsQaQHic7QdudYq', N'', N'0350264424', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (176, N'loipva176', N'$2a$11$c2Xmxdx1AzYnnD7vZMONgelFpCS6HtS8xw/uQkbZBuSURfWlBW10C', N'', N'0552137720', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (177, N'truongnva177', N'$2a$11$rw5LvAYp6cNGDwA7qlFufui3LmqK61v/mBlHG/FcNyel6lK8PXI6a', N'', N'0651785165', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (178, N'thamnde178', N'$2a$11$MysePrV4jhGFQE.6ZkGmRePeIiEbEIhisTn4GXBHqZwojqfFMuoqm', N'', N'0430790829', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (179, N'huypva179', N'$2a$11$TVJhX4DkvZfcc79rFrk/ze0ofCAAfSCMnykUK9C5Gi.2jyVhAIuyK', N'', N'0578679841', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (180, N'tuvdi180', N'$2a$11$PArud79WxgPK1Yr9D7juGOzIlzj6kFgVC2PDUi0qhvj6YFewm/sVm', N'', N'0724044787', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (181, N'duyvva181', N'$2a$11$3XoRzIU7ucGWAktjFxHb2ummBjkOlc.bd0FjwElUiL3sKomLp3hJ2', N'', N'0489604002', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (182, N'havva182', N'$2a$11$R6/eIfjIoR7zcgCS1KU.7eKMv/A/GCGXFPtKnw9F/hH3AeVnah6kC', N'', N'0981714200', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (183, N'hinhpva183', N'$2a$11$tBHrjQTSZhjatjCPbfXy.eVo7RPOX6uzJd..kjcuGyGGku8xZ7l5.', N'', N'0757770079', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (184, N'dungpva184', N'$2a$11$CAJywXprJ9IMg0Sdu9ahTuX8EjyYA.v1VGkwmgQjFLkxOrik.T38G', N'', N'0316193020', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (185, N'thinlva185', N'$2a$11$.hchyLTLjszkOxBo2mf/8.gnvMjGRAqHNRYIpGXIUQRxs7mFj.iSC', N'', N'0649902468', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (186, N'vinhnva186', N'$2a$11$zCPPM064pNHZ8e2d1GbheuKfxrjSLS38cpzl6Agu1Z85V2K55ud26', N'', N'0315589308', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (187, N'daivdi187', N'$2a$11$OA6uQJZHsMXN/lWsGjCa9uRUEV6WugsHrOcYtY3ZajHGbmk8i3ici', N'', N'0836833232', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (188, N'thuyvva188', N'$2a$11$LpE6fLDb42QLSTtzql5/IOUSdEs8bDYf.3En5sUwIAaP6JBMWmi9K', N'', N'0661938130', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (189, N'tuantva189', N'$2a$11$GtDAARCMInxfcHXFYp0KseNe/Kzv4QvNPx2GDWgtiilknVz2cuTB2', N'', N'0788863426', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (190, N'dieunva190', N'$2a$11$Hjm/sl7fgkAk6AIhcYK7veW1vclkl8NDQfHJMieyJSl0UyTZ.LTlm', N'', N'0195117568', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (191, N'phuongbti191', N'$2a$11$F3pN.bw6bAhZ35Rk1J5OFu3w2TP.F.0YcfgwJQYazuenKhEHfdaPy', N'', N'0534015053', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (192, N'luyennva192', N'$2a$11$2Tpxr4m5.eCS2MKghk7bXewK0agxx5.KzRwcSPGkkKAieT7Q12Pu2', N'', N'0188092863', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (193, N'manhvva193', N'$2a$11$oynp3Nib04Nc6qr9YzqCZeBHHjpH6MRY67RfnQFKA95K24pxbDKEK', N'', N'0282298642', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (194, N'ninhbva194', N'$2a$11$35blzNqKkRMZ84MWnPOKSeQzvbrMVY93CjSZeDwJwK9PNGiO4F7Oq', N'', N'0524113678', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (195, N'lampxu195', N'$2a$11$YD5JWemm/HANz/uC4oG5WunjBLiueDt0zTDs2.RpSItleyjlssDJi', N'', N'0644121450', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (196, N'aitva196', N'$2a$11$54HJc4sy/gQCtqiptjcFqueUnOan5a6La5ENPxE4Ldh2EmXq5Y70K', N'', N'0391489493', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (197, N'kiennva197', N'$2a$11$/iA4IrkOaeTWQ9GUEcnKZOcY6g4rYPmUijZmFD4XUAUBxsyKIArQ2', N'', N'0995961290', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (198, N'letva198', N'$2a$11$WmgVHL3u5tpO3S/AIq.DaunulKpo2yD4dYiUZxlI0F49mJ3BC27.q', N'', N'0936568117', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (199, N'hoanva199', N'$2a$11$Hxl9fHSwkXtxfBFcVekZJuDx.fD96xyKk.mJeUyFWQJPpeZt74GB.', N'', N'0971430987', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (200, N'tanvdi200', N'$2a$11$np3mq2axzqEilptP5J3JgeLxlOjFkpRD2WUVp.RVBbf//JByY1EOu', N'', N'0340542328', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (201, N'quyenvva201', N'$2a$11$7F8IV1M2jbQGrcPzhvKXU.sosV8WyaCPbyU.lOjWgUiZC9ZFyIODK', N'', N'0380858451', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (202, N'hungpva202', N'$2a$11$K0q.1.ZlkeO9qDbNjvgE5e8osiBrrUb019otU4BMvWtYPll6BYdIe', N'', N'0810709643', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (203, N'tuantva203', N'$2a$11$tY2peRATUORdWwwC5Mfq5.EjnGx0fw8//gnQySmEt4pOrGjy/9Ldq', N'', N'0287507385', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (204, N'ducnva204', N'$2a$11$YlUpfcn2XeSaUVCokiGbUeHvS3H1hnLYZsDMpRiKBS5K2mB0F4XXm', N'', N'0401810204', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (205, N'truongvva205', N'$2a$11$pLh0alT0vAfG2TiE9udy2eohjK3YPiNG/pfYBL0QPLJzOin6LL2uK', N'', N'0993563610', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (206, N'phuongtva206', N'$2a$11$ByngRf4d3uS/R7rK9hDxpeEvGr7fiF/hGFsIrFdDW6BDdif1Gttqa', N'', N'0408067798', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (207, N'truongbva207', N'$2a$11$AC2vMSNkBkGcFWivNlzdZuenwKOoIUb5NiexGMybLhnw4IVklS3X6', N'', N'0114012020', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (208, N'luannhu208', N'$2a$11$6pS.L9HLKs2oJ5gQy.NmK.5f.LLoY3l0CoHdj8n8BME6hgGUhO0AC', N'', N'0791558992', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (209, N'truyenpva209', N'$2a$11$59n66uleXeSQvIwxGJ59q.yq3usqfFNs2xK4ExEAlRF68xcjzng3S', N'', N'0441529160', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (210, N'thangvva210', N'$2a$11$Oi6NO9VZW5QX1VJwn91BQ.5z6G.p9AtAIistilKR/rrfH.JkObT16', N'', N'0437535405', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (211, N'hungnva211', N'$2a$11$9.Bdna9EYsLQxVG9wnqCw.PjXG.aBH3nRmTRkZzZ5NeG4RIT2tkWC', N'', N'0630516427', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (212, N'toitva212', N'$2a$11$8nmybOwj3Ju5RaZXo0Clt.qXRwd7uyMREnt.3yvNc3PKjfiaUH3Au', N'', N'0352324092', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (213, N'thieumva213', N'$2a$11$HBFIw3s08junaAXps7YZIesVcJqOh0Kd6/E4pGguGcp3g0t.G4vdK', N'', N'0453398901', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (214, N'hanhnva214', N'$2a$11$My.Q2RRJxQ2/rIvmlh20bux8e8/83gbMYCThKZj8j5ZoLD4xs4fsq', N'', N'0944431066', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (215, N'sypva215', N'$2a$11$5Ve0foG2e7nnm7da/nDnD.IgIVhqzfzXdVWmWILiATnM.3PHZNpNW', N'', N'0407314795', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (216, N'phuctva216', N'$2a$11$.SRv/HTHnhsdl.nHHSQVI.zd3Gy.1/Ab1X4huuEQZP740.HgmDsFO', N'', N'0954472935', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (217, N'thoaitva217', N'$2a$11$lHJQudfCS/kH1SvhPb2vieX1TTF1YlNCCgQx.UmU749IEHpKSxUEq', N'', N'0874320536', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (218, N'khanhlva218', N'$2a$11$uOzmyZKRb9YUDsoj5BtHpefniX10jYvtFIm1eoJvc0EyEoEBMtppa', N'', N'0424561667', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (219, N'hainva219', N'$2a$11$nFLX3XW8pj6nMUwjd/ex1usVOs7X.oZTOt2ljy.55SzOW3g22j8re', N'', N'0863236469', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (220, N'taindi220', N'$2a$11$bBN3Xi5EndAMWOlVvi/2Ee1/cGN/CBHljYzqZrXRIGsWym4MPw96e', N'', N'0422767484', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (221, N'linhnma221', N'$2a$11$deWcxRTTf0gLfEgLD1PW6e6xeqKl7A2KoEvXVsT/cbzD0sXTeF2oe', N'', N'0186258178', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (222, N'anhbxu222', N'$2a$11$QQgxxZtGdWhZuoVhgtegDu/UhrMV8BXNrmvmVIr1okDHL5y5KCNPq', N'', N'0563284873', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (223, N'nghiattr223', N'$2a$11$8xD.AiTOg6heXdgYxjEBZuHK7we5PXbE1hm6XLzMEKPaM2ESxrP1S', N'', N'0148458451', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (224, N'quangpva224', N'$2a$11$KRkV6OM.X9EoCjt3CreskObfEPiDEgni4rOblB9ym4hOWPcUb3knu', N'', N'0987926733', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (225, N'duymkh225', N'$2a$11$SHdiypW9e/uQz8G/GVlie.g1rRmHHS6CUNsXxGLxYTlVKifVlQTAG', N'', N'0753304845', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (226, N'thanhpdu226', N'$2a$11$DXlji7QJA35PY6wH0uoC1OGeTeBuxfsMNqNJN1cUwFvDZcCCToFd.', N'', N'0399571633', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (227, N'giangbva227', N'$2a$11$VtAZVxAYiXUWM2hKo8otB.kJq2bxGH.WMkhBB5bOoaX7nJcGrVJuC', N'', N'0752317899', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (228, N'dinhvti228', N'$2a$11$xRkqFUyOSqNvpsIeeTgaNePSel9Gjm8ehNUCVow0YRbapMGekabiS', N'', N'0863189208', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (229, N'thuyenvva229', N'$2a$11$Pl6zqvARA.z8aIxAi12gmejsZq.TEvUpVWmYD7FK9yhfdn1YalAXG', N'', N'0791994959', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (230, N'thonva230', N'$2a$11$pFFgtJWiNwoV.gc07GALpeNpTlwX9yOLSLlvxz5W4gu0yT4X./79S', N'', N'0615068674', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (231, N'hadqu231', N'$2a$11$62uYTECwCexpaJPbcgdaEeXHAjhesjiGk4mxfDLTKOiJH8x3Kms4q', N'', N'0236124628', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (232, N'thuongvva232', N'$2a$11$A.pHPJvfnYyy74l7b0bMse/NJhEv9LRTrqn5.CDiT8OqI2xoAQHy6', N'', N'0445875465', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (233, N'tinhpva233', N'$2a$11$OsoULOw3qrrkS2Pn8cr7d.FzmkVCF/s3TVHUoXm1GDUYfj3fOzDI2', N'', N'0966616851', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (234, N'thanhntr234', N'$2a$11$Z20Dg2ZwKpmzCOlV7yM90.FxJwG0g7c4IcGoPW4XoUsTcPcC4YgKy', N'', N'0848161005', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (235, N'chungtva235', N'$2a$11$6BblR7TtawCBTR8NKMUCOej86ZE65Yeqz1tBilmfYoILouejUhOKG', N'', N'0190973609', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (236, N'bactva236', N'$2a$11$BT8I6sfuUbf6HWwrAOtaq.QmRcr7bR6s7wizDParY3qjTE5oKiu6e', N'', N'0643950593', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (237, N'hiennva237', N'$2a$11$.QQQZzWON9KDxcEuqjvda.YkMfTNx7XvwK0AMhahQXbKzV1iyuICu', N'', N'0409525257', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (238, N'duynva238', N'$2a$11$B7hbkCZr4nfIV8Kw/uckfOKM56dbh.DYhOEKMoo.rE3RaBwJRLw6W', N'', N'0833034420', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (239, N'sangnqu239', N'$2a$11$/vvp48eNsJKZAJ.ROvEbjebkGXwvWxr/HqhhQCasQEnlMh2fZrkWG', N'', N'0120557469', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (240, N'sangnva240', N'$2a$11$RCMDP8nxLZCSQJYTlLhW4.Xn89uIwPCG1dpPmDTKSEVEG/qzlt7ja', N'', N'0527586710', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (241, N'nampva241', N'$2a$11$HPF9m4wiMVDy911rIirct.z/adoKZ.w9L5zDvM1HGCXNpG0hVQyBO', N'', N'0166453820', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (242, N'dungnva242', N'$2a$11$VdLIodiG80A0o9i6HqAHhe.zdtKXGTDp/wLzLHLfX4cM8EY6v4ngq', N'', N'0863737988', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (243, N'baonva243', N'$2a$11$zXxJlU2dOIXmtSe1ryH.v.E.OLLStfb7PmkrV8ez/Hg4mcFS/SkPq', N'', N'0548978990', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (244, N'thinhpva244', N'$2a$11$XdwW.3XSJwIcx0sbCdgvbus1zlLixCUVjgfrIaWy2linRvzn5qu7G', N'', N'0605725467', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (245, N'quanpva245', N'$2a$11$5DhNVxUVOIjXObXGSfXNKOTou0WRNEKeYUsp/UVY0T6INCuJt2OCC', N'', N'0566827589', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (246, N'suotdva246', N'$2a$11$F5plFzN.e/OyCR8xyEoGquAZeZ3gf9trzcnHbdvLC3FtJ8672UTHy', N'', N'0683246564', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (247, N'bangnva247', N'$2a$11$HKJunegQ0fXYnTOi2K/aZ.dV9XejzAS8VR..LGsUrNCFEbDRthr0u', N'', N'0953563612', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (248, N'phongmva248', N'$2a$11$i4ZE1QOCRUP8Ve8VmWPxEu5ZRLPkwlWfuXkYFiiSK8pTWJswGdhum', N'', N'0927640545', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (249, N'phienpva249', N'$2a$11$.LKzpHpgozM4btsJyVQobucnLgyZI7W8FiGw8BXeV3eRpW80uqU6C', N'', N'0619075518', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (250, N'thonva250', N'$2a$11$UguYnMaoKgouyqiUypS/auRdqlRH5RQAayLk4aH.v7/471gUOebpa', N'', N'0397523283', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (251, N'chinhhva251', N'$2a$11$wAIEYHO8Kh8pzvnKB.3Yde61gYzhTzVgMjibjiL2T8XkqbMc2RPA2', N'', N'0622671931', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (252, N'quanpva252', N'$2a$11$7WvQqgyfPC.iB9dpP7qJ6e4C0f.wXx45LeLcrzsT.mfwIt6To2zSi', N'', N'0309500157', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (253, N'thinhpva253', N'$2a$11$no1fD/ckWjcfIn6lXITa3OslQZzKEcpxpqGRHhsauodTmzxvmy1RS', N'', N'0330942970', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (254, N'phucvva254', N'$2a$11$iILIlWrkXnJWL4nG1m2.qe1c2y2GG./zZZxSRVhzOIxPtxyFBFuRm', N'', N'0714582061', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (255, N'thoipva255', N'$2a$11$YyCIJG2uVniw3c9bSFDyFev.Ue.NoziRIFM/Mwz8oWQQeh3KM/joa', N'', N'0638512200', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (256, N'nhatvva256', N'$2a$11$6GDbc1p/TOQEaeAOp4OgdOMeZR2TBIMVOefW69F8cuzedGGG/Wp8e', N'', N'0285929548', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (257, N'dangvdi257', N'$2a$11$IuuqPfC2X5x/faeLCMQUBOeDSBtWj4niZJb/QzLDq1pRzbLj0uTCa', N'', N'0554804211', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (258, N'namnva258', N'$2a$11$Rvtew7JISR87IlxwIAop2eKFL29CP3rtLa8jfNr7VOr5csoY0OBYy', N'', N'0708550095', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (259, N'thinhnva259', N'$2a$11$s23vBOcJ6H9DoWhCGH1Ck.CFr/VzyUE/7PTDY6g6e8OO5ExvBYci2', N'', N'0684952825', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (260, N'tutva260', N'$2a$11$57d3.LJCGqHgXabDw5oHuu23xmkw8DD.awDRcFp4jP6I6z/son0iy', N'', N'0286573493', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (261, N'khiemnva261', N'$2a$11$o8/u8BZeNRqkRltSTIlVPOt/opRErVEgnbJJynypMVkrlDnP4SQ1C', N'', N'0357974916', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (262, N'thuyenpva262', N'$2a$11$4BGfwzNgypC.b9pnEh2T5.8GaF7tBO5iea3XX8N2uc69NZi7joo2u', N'', N'0308730363', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (263, N'batva263', N'$2a$11$vqQXDyndXOysVre/KnGu9.dbvnVcJuTGWoAyj4aVW4tVg0eiEfiC.', N'', N'0580334872', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (264, N'hoahva264', N'$2a$11$TDqgZb5ASxlOCXOl1iPAR.egk0t7.HSU3u/aCNqD4o.Quqk/ZB4wO', N'', N'0237658798', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (265, N'duongvva265', N'$2a$11$pMr3W2kT10Oowx3jTVxdTedsjy9SUTyv9bldmP68/IV4iITR8p1Uu', N'', N'0918024665', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (266, N'diemvmi266', N'$2a$11$04Xbiu.CQHvAfWd/sPVskuKttSMIQgQMglXLt326xlazIwSdDFPdm', N'', N'0313664126', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (267, N'thachvqu267', N'$2a$11$Q7cpG0.JwORNRdpQWIAnEOSZg8NKSy0T0sqOAdhITPYNpLmE9MTjO', N'', N'0235284560', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (268, N'tuyenpva268', N'$2a$11$QJPVeWcvh712tvqsBaA65eff80xB23kDsiK0fN/xFk70pyTVOr4ie', N'', N'0688556802', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (269, N'hienpva269', N'$2a$11$yycnhoh7H8/iA/pxHppHC.v8u2UZlA9D8OaleW3DJ2qcbEi2Eix5W', N'', N'0643089443', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (270, N'tuannva270', N'$2a$11$dPfHvo9AWbYi8gEEHm3lRuxCLUbnfgU.D0gyDh8A.mp.ZaZHOq0DS', N'', N'0830193424', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (271, N'tiennva271', N'$2a$11$H.b84ZHhxRvT4k6yaMT86uuD8cVlbwIUXLxGNywitNBuHR5Vf/she', N'', N'0735525768', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (272, N'chungpva272', N'$2a$11$AroHfXTL.D0SInJMQuDdu.NVQ1S1IQDb85slswhUZULCDToEETRK2', N'', N'0463345873', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (273, N'duvva273', N'$2a$11$aOQLkT/U3EzPEAp0TWRXnOpAFVZYH6jVKL5l4tWBCl29RPXNlnBNu', N'', N'0417184144', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (274, N'hinhdxu274', N'$2a$11$DSTfcCxeaLa1roOQmpxtie1rD8PAvV2Hw8LPT7jhbTjR7My9/kkOC', N'', N'0837523579', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (275, N'phongmva275', N'$2a$11$wv918LiqUN2gsKfm9M/j9uFbeb0/jh234z9/Fu7gqKgnKms5CcUOy', N'', N'0249692529', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (276, N'huyvdi276', N'$2a$11$y8oz6sNwwNb4Z/l6PQlazee57ICcf8VMLmksLlac9rLseN9PTj9Zi', N'', N'0837373912', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (277, N'loipva277', N'$2a$11$CliOXOHsd2yl6PSuNM2ku.6CdkoMy83jr0Yrf8DUTGX/PVaH9wsP.', N'', N'0970515066', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (278, N'chinhpva278', N'$2a$11$xt96/A54IbPqGaoCK8s4m.0JCVOrUMto5lWn9P8DQJIgV.l6tm/ES', N'', N'0676285696', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (279, N'quantva279', N'$2a$11$iiat5ZFswAUwmXuNeFZ05.T06b7cfRGhHG666Ut09eNBALjLSWbl6', N'', N'0962141531', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (280, N'hainva280', N'$2a$11$qH9R3jxFAdNd61jb53HES.w19IuLAJtw28xJoPgt81kvYTlcHSH0e', N'', N'0938820850', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (281, N'luongpva281', N'$2a$11$1KePIb3.RxzHama8e3B8zubKu0tR5CgxfrOf2EhbVyn0Up5sqAbCK', N'', N'0474792414', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (282, N'hienvva282', N'$2a$11$h4O6vtmAStIDQY1Su3O76ehg8vm8zC6lIyr9.kVbL19RumKwN7MLu', N'', N'0262599158', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (283, N'tuyenpva283', N'$2a$11$rWm8OKgl0UiCc.6X0J8onO49ucopnXB89RJxil.T3na2wC/DuLKCu', N'', N'0930764621', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (284, N'tuyenlva284', N'$2a$11$sQNJJDKwzbuZseCi2YU5vuTv.9HRJPBUICdVty9z6a/bHBIm/w4wi', N'', N'0575261151', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (285, N'tientva285', N'$2a$11$5nys6wmbcWWH0N9ssa8QkecHAHpfT4qNE34jLmf1Kd9koVezjoUba', N'', N'0463542801', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (286, N'supva286', N'$2a$11$x774EQztMp6Umy8jdsGwt.FEaTVGg0.loe3gb/SDUCmnApQdgMz5e', N'', N'0227134132', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (287, N'hanhnhu287', N'$2a$11$8ZO2J9tiwldsB9Wn6SthSeG9iI8CxM4FHin.MZiOZuv/adZFwc6j.', N'', N'0569801300', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (288, N'thinng288', N'$2a$11$4Ly.iIlJDjRaeF5IEzGZp.5.DGtdO.o8N4ssMQKjKUDT6TP2NzWDy', N'', N'0585226309', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (289, N'locnva289', N'$2a$11$GmtS.e/CfbdyVNGc.P2o6erkZfXVRKDsooEJJvSn8vbfwXH9dyMMC', N'', N'0277421742', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (290, N'dangvva290', N'$2a$11$Db29DM7zx7kKXrXMIIBhs.q0IpCeS8/IxLWx3cmeex1X1kCVEXDfW', N'', N'0579174065', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (291, N'tieunva291', N'$2a$11$QUHY.XZqLMvv/gE3iGxqpu/gyFKJyTT1vaoLtOeLRzHp6r.qeU/WO', N'', N'0357651227', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (292, N'thutva292', N'$2a$11$K1B/ON2FaEj8S/i9ZzKlE.ywPT2MUZYJ659eDewgJcOR80SUXHE9a', N'', N'0714767348', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (293, N'henva293', N'$2a$11$VNVYoVoL8EJtgUhiFW3ktue.H4sZQIa9nwYW.ifhwjWpLWdJ0H6t6', N'', N'0734526163', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (294, N'vinhnva294', N'$2a$11$oHpr15p8v.FvrshCfpUJ2ep19tr/kfwsHpo1EDI6x7X3K3juZbMw.', N'', N'0495678186', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (295, N'hieuvva295', N'$2a$11$H8k/zQPfxB7LEibt7Py0XOJ6UFe7VOvKq/VpBvaOviQTWWZie0GAi', N'', N'0569686716', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (296, N'quynhvva296', N'$2a$11$7VEeCxWtQup4N8cSelv6b.V4Z5Ll9FZTl4s1cZ9uTQaQZSGCbXiaG', N'', N'0419017326', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (297, N'bonnva297', N'$2a$11$XCWC91fywKcy2hPag86Ese6U3nzRqCTNs0SvKfNodbqpOD/HZn1B.', N'', N'0825706893', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (298, N'tientva298', N'$2a$11$3CH34DIZoBBtnPmnvss6nePexA3zankHMbwEfw/1qPsL.dvjPa3ne', N'', N'0135344004', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (299, N'luongthi299', N'$2a$11$AC1EOY6Vn7WyoGUENsLkfubNH9jqM9SciyuKkAVB.j8TUMteXukbe', N'', N'0336065238', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (300, N'thuydth300', N'$2a$11$SbI88ZL4rx7roTAVu37bzOS4AL7mgUOTuCSYdNUAE7Z9pL36G1U86', N'', N'0457926738', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (301, N'chinhnva301', N'$2a$11$m1cgz6bIOupkhRfgncW6N.JMq/AKURlgqOsWAVqLcQcAljQilOthO', N'', N'0934881168', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (302, N'hungpqu302', N'$2a$11$19kGHKcJZhDLrpdui4koaul2L.iJB0C2UC0pq7tNAzvZsjrUoqBAu', N'', N'0877669811', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (303, N'sylva303', N'$2a$11$gTP1WgX2T5iSAggblpnzWebzEdOaugVyAH67lP8vzHNA/yQ9Eb5PK', N'', N'0524541920', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (304, N'daitva304', N'$2a$11$Atk.yRUOVTVMA8pWv7lmzeByuKKdXC7pkXeGk.TC8/EwC6sW9zVyy', N'', N'0401961481', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (305, N'truongvxu305', N'$2a$11$zA0xMy2a7SN205Lvm.PmRO95xRIiI7yELJXOH29RTaROqNk36KPAy', N'', N'0884345132', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (306, N'hichlth306', N'$2a$11$7Pk0qNE48/7OZeepQ911nOvh1LmN0MNMdLNB/8B.fRPdIgcD9Xlkm', N'', N'0387017178', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (307, N'luctva307', N'$2a$11$rMx5uwaAJ9mRiMyX1JV87eXrbSZPLo8NwwShH/tJ5U1mxG0EypGtO', N'', N'0548282045', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (308, N'oanhvng308', N'$2a$11$KIJjUEE0WBa0uWUd3EKRPuA81Sdp1xBV7j1pcVZZ1.1t/UQP0eLfi', N'', N'0400394427', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (309, N'quangpva309', N'$2a$11$aGKSQBfCv2CKQFf6nfbfdOnzDNf.bdDeygtIlKoUs3fS.Dhr1SJNm', N'', N'0905086332', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (310, N'bandva310', N'$2a$11$YF7eAjnat3apN2kT6m1ffelTxqmHFl0c0811g144xA6Er8Em.POGu', N'', N'0501673460', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (311, N'xuongnva311', N'$2a$11$vFI.th6LT4KTAjg7h/kHCO3IsHWdS2H.5XPQn3YGnAGsjFGjPMVH2', N'', N'0798673552', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (312, N'hiennmi312', N'$2a$11$mGigPCUk4Q5by8UWrGgnn.gyASqulFQTmOTkyuWnBuu1E60a/hTuG', N'', N'0791138851', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (313, N'thangnva313', N'$2a$11$6BZtV9Cw.PKHiWBbj/bJOuhrqtjB/IcvS1IKYZG/cu59RrFMnqEB2', N'', N'0132096225', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (314, N'manhvva314', N'$2a$11$k2mMJS5ayXDqWTl6Z6LfRekVaKErZ2LpYp5crEp21ERW/b1Mckdja', N'', N'0947914290', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (315, N'baynva315', N'$2a$11$EZCNNdOcrrclGIhu.UbNpur6.1ydV3v49ngnikIHlzzL4jN1R6Eaq', N'', N'0803139537', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (316, N'phuocthu316', N'$2a$11$PQ/k4dnanDXVtsIBZz/WEeBSXjhsY52Nd5gPgsmYa73/PDwoL5Lhe', N'', N'0573175466', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (317, N'mainth317', N'$2a$11$O7GTwZSq2.dZQbXIfECYc.qzDSE2dpe3lz5a64lM.KAJbk0bKg8cG', N'', N'0865201669', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (318, N'xuongtva318', N'$2a$11$xvKpEQMFDabfkMXRqsAk/.LjHsJdohY9AqbIOXlHSRn02C2i0JBAO', N'', N'0479066085', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (319, N'bangvva319', N'$2a$11$Bvz56oOGxmP4SrVgY/loquJD6wR2FZ2XJ4S6J/DcyuwSh8h8PLdnC', N'', N'0307950752', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (320, N'sontva320', N'$2a$11$Yc6vaFr1zuRydBwsPOgoiuFt69yEAIJ9I8HwQ7L5zry9LpPFO16dG', N'', N'0788942015', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (321, N'quannva321', N'$2a$11$Zo6DpGUvYUhPoQsurovU8.Iqp2gHK/0IIYKkvnwRvfj0eh.KMjRlq', N'', N'0511782437', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (322, N'hieuvva322', N'$2a$11$zgNLoe7z93J28f4ZlZGt.eZNkUo7m5cXdjUqYShj/sLaGzW2xyttS', N'', N'0939568543', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (323, N'hoangvva323', N'$2a$11$V5HALbyT73THowEGxXLS9.nGrpx9QXBTgoWPLhgEPxUJGAqIKafo2', N'', N'0586038106', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (324, N'huypva324', N'$2a$11$iQ8C2wVuT/VNWsegUmwgluQVYGaPis8au7mAv2T7OKM0lZJME/iNe', N'', N'0600895774', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (325, N'tuanbva325', N'$2a$11$JN5ofu0.dPUiEZhHA00HZegc1PM6BFPpznQeFN.Y3AsZ6oiZLzsKC', N'', N'0262273699', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (326, N'nhoivva326', N'$2a$11$xRPxlTCvjDx/lcTLgKz6ZemuFDTRV/xR.JI/FCUFN6E1u5GcDq.Ji', N'', N'0754037141', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (327, N'nhanva327', N'$2a$11$4PuVksIijRzUkWXwHyf0dOsmyBPx/pUT8BzVOPtyzVe8pFGc3LPxe', N'', N'0569805163', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (328, N'toitva328', N'$2a$11$giDrfQ3nyUrnUygPXpfgXeho//8DwCI0tfNESqXaz3nQju91XF5oK', N'', N'0793076050', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (329, N'hiennva329', N'$2a$11$RP0O7bcCMiOaLKwIex0vBeYggYc4YKcjPtFss/efC4.OwtW8tC6HS', N'', N'0380288535', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (330, N'thamnde330', N'$2a$11$NUgd8eg8VAPkOjh2Xqf.CetKH0kL5g3NnsnwxMYJAKVwli1FjDZFy', N'', N'0341325640', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (331, N'huyendho331', N'$2a$11$hPb2//0IcG25KOL0I5fzc.2Moj7WxHDCwvZc3Nxk2OCMVkXBIIkK6', N'', N'0408940643', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (332, N'camdng332', N'$2a$11$vwbIuZ33p1ogFxHYApFpNeDc23fBC4LByCGkZQ7GILUCCdyZJlj.u', N'', N'0297666680', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (333, N'duongpva333', N'$2a$11$kCG7yzer2t7B2eo2.Z7WH.kRiTUp9W9tmk9jq5gBR3BIgEk2Tkd/6', N'', N'0525525432', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (334, N'tinhnva334', N'$2a$11$Y.d7eLC5YUBU6K6AygIA0OtnxS9IJeqC8xrNR9J4w1ncqKv1Kf6k2', N'', N'0553588581', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (335, N'tondki335', N'$2a$11$/UsFwsyiDnbpumzQihOX1OZGFKBMIbhlEm5cF0EhsPUpPZjpjA.uW', N'', N'0753230923', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (336, N'phongmva336', N'$2a$11$hPTdM8yVAxhInMKhaDURLuHKi3HdEebXOzOExLkrWCQT712rNITGW', N'', N'0146558535', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (337, N'thapvva337', N'$2a$11$nEBVfqGYPNYmlotZbW1k6e3Q6aLci/3dtCBnT6kDrRjeM.akquqDK', N'', N'0358561784', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (338, N'thanhtva338', N'$2a$11$MpChrGA36i8ZL6OSj/rEUeXVQdkRt3uk0U6fJLlpK2m08TRgGapcy', N'', N'0440343785', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (339, N'huydng339', N'$2a$11$AM.sADLyHqRn5iVtqLy49uXUCSlaQy1ytHqEtiT1EQcr0Im89bHe6', N'', N'0460372537', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (340, N'doanhtva340', N'$2a$11$ov.Tnhqs7tWawNrNmiBIyOW/X62vDmJvzbsDLZhgWTWMVUU1zLV7a', N'', N'0447912013', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (341, N'hanhvdi341', N'$2a$11$Hh08.J1fFCVtkJVJnykjz.hBJNTSwwprru1ihUNmdHK7VzxEPUr5m', N'', N'0511166387', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (342, N'taonva342', N'$2a$11$NWUA6a5OSWqmLPB1CWUkSewyngytFX93CEEmbCuyuEy6pxppXIgle', N'', N'0637534856', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (343, N'tutva343', N'$2a$11$WQIKsamSVVXUyAu78Nii5OYKIejYAuSlJHufDIhexCHlm1XdnUeje', N'', N'0828784674', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (344, N'thuyenvdi344', N'$2a$11$arUdd4K52Qx8KjFQnNJe6.Otmu6QV1dtzgTSMBL8jmhM24OancTaW', N'', N'0287719547', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (345, N'thuanpva345', N'$2a$11$0VrUzchhj9.zeCRUd.w.7.gICBi03xZqolR3SCj7oAOM/f0KsEA2K', N'', N'0181611949', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (346, N'phienpva346', N'$2a$11$kHA24bXiTo1KNrR2vQgC3eCXR4z91oTscLUFmLyyFPLAtyMgPPtYe', N'', N'0681593680', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (347, N'oanhvng347', N'$2a$11$fqstvLtC8S9V.0YDUibKCusOrO10Fzak46DTew53aU5VGMVTjZVIm', N'', N'0155421680', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (348, N'danglva348', N'$2a$11$bIqqrRXFAFRpZtGP1WfYi.x7OgF5snz2axXxSW.JHFdHYppHhodlq', N'', N'0556368100', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (349, N'depva349', N'$2a$11$sbVQWf4PU1WQBNPzPiKKkuQgpQXPUZUg.2WOJzQgRjh/8drzh7ZUa', N'', N'0354987579', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (350, N'damnva350', N'$2a$11$VDsGKcJc1g/3unhL3JkZr.3nkXzcxu7VVRukjTyJkG.v562ucyIeS', N'', N'0148502922', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (351, N'dungtth351', N'$2a$11$GCHsvv9LBTOdo.73Kf4u9OrWP46cfQ9ARd0PjffLRcc5NDZrf2vaG', N'', N'0398521441', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (352, N'duyentth352', N'$2a$11$Y306Wqn8ohFwtRmdRQB7leh2LtoFI6GQt7yr9SAToGIsrMNGCxaVG', N'', N'0849895966', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (353, N'binhtng353', N'$2a$11$.axNWBoZYE0LdevJjevMdufJondueAphK0zUXn8c2B2MGwlkVByhe', N'', N'0948511296', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (354, N'trieuvqu354', N'$2a$11$34oFdtrECe/Dlockfl38OeG8sLHmvh3TY08/ZeY.Ixq7T4R4VWmKe', N'', N'0917858529', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (355, N'kynva355', N'$2a$11$qpL2e.GxeswkEoFtFHCraOYeasURaYMO3mFCKzGVILy.CSqhB.EMK', N'', N'0385738462', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (356, N'huybva356', N'$2a$11$SRwDgXt5cop5LFPOVBGFEOtEkJRLTFE79d4stEzng9fymBIX3p64a', N'', N'0198083770', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (357, N'dongnva357', N'$2a$11$tz7BT6wj06XVDH1NXv3hFeZezNTmFpXrAAKTr0T4XurCs6m0WYSse', N'', N'0881842523', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (358, N'duynva358', N'$2a$11$zoLlWdbE0EaL37FXoRUWCO3l8/BTq8sOSq1ojKPGBdtk3KzohwTea', N'', N'0661932229', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (359, N'tuyennva359', N'$2a$11$jKEi83uMaMBIeL12rtvRG.gta5hw7UaOzSha3ihnmqGe.R68Jbs7O', N'', N'0946315211', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (360, N'khuyendva360', N'$2a$11$MxaVPQH1Y3M/KU.46ABD/.t3P0ot7Qse4YOLEzNYU0b2jDah31RCm', N'', N'0262959969', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (361, N'chungnth361', N'$2a$11$TkRqDZLwqfYksH1InK6NWOKQYGKoavASY7wNcBqAon2X/xmV7apZC', N'', N'0172394591', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (362, N'trungtva362', N'$2a$11$wxsDgKE1YddpNFD66cxcW.xqgDkPs5iTQczBtFF4ppUv/JeE95itu', N'', N'0959734034', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (363, N'khoavdi363', N'$2a$11$J3QF23Ad3g.UMOePv4fHO.0HYvcWtBsr91fxhcVyEhJbq8H3W6OeK', N'', N'0369535773', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (364, N'lamtth364', N'$2a$11$wpc.JcpGnkkoRQiAVuH1mew6/6O2yRG8LERgaSZHOL06/4JjLpaPK', N'', N'0368420350', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (365, N'doandva365', N'$2a$11$ROnjJhtB3M/AP64XgvJa3.Tf45GCwMe8V3S3YYz4ZW9bSFWF6zo66', N'', N'0455647236', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (366, N'tuyennva366', N'$2a$11$YvfjLkpn2jd1pGErV2F/tOnJNnlpt2wwHlQBCB/UCBOc3dUlaWtAu', N'', N'0336074733', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (367, N'manhnva367', N'$2a$11$nekjVki5X2TC7lgLTcphHu9EuB2sC4jWcnNh.WTxylAxL0KXzGhCO', N'', N'0887217777', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (368, N'kientva368', N'$2a$11$x6R5BmGPjcJ1Zf5K4Zmk3eOrp7IW0x8zoKpHCF.Q1WN0pWGWSvS.q', N'', N'0400262033', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (369, N'tainva369', N'$2a$11$wHSfkofiUBTEOIuXiNyyVew9zoyY90XWIfcMg9/3hr3jEdC0zxz3y', N'', N'0330459100', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (370, N'tungtth370', N'$2a$11$oc1dJx.9oIJHN9uQOVRFpuz3rfIsKpkmv4.cUczMvoNqgX52331xy', N'', N'0125628399', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (371, N'trungnva371', N'$2a$11$tqu4zl3z9VUUr4KLqhkdReov1g08D73qFkHs0lY9.HqLk6FPoGCki', N'', N'0801589006', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (372, N'thuyenntr372', N'$2a$11$wya/ALHv3tk.PI0PLjDGz.I65v.IIAfOO26Dr8STmZAloC5Obufra', N'', N'0333051669', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (373, N'thucpva373', N'$2a$11$8aDf1BQsQifvCMOJhWMxb.vydTrqqHtOxC8TqJFUQOw.fqi/adFbW', N'', N'0829380232', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (374, N'ducntr374', N'$2a$11$4DoYLOJpdJA3aFFRqmTEJOKq/mTSqkfqW15pZ2ac.UC4cHAGypQam', N'', N'0661994886', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (375, N'thanhnva375', N'$2a$11$7raEBNblgbbd7ukyMEd6KeKRidtEl6DJjF1rJz0iY5plWkCOlN4EG', N'', N'0518099898', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (376, N'truongnva376', N'$2a$11$5Y79uEvA0VuK/bniLsfX2u4mgdDcZVg2cERLRhaggIAx2qAPTzFR.', N'', N'0131687939', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (377, N'dungnva377', N'$2a$11$mQCp8KcvqpRFge89LVp68e91WyvdzJJ.QJO6ao4c89ScQR9TCC2Q2', N'', N'0313964587', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (378, N'thoivan378', N'$2a$11$ulWKlobWS4AILXid6gTej.BRsD7SjTrP7YSJtZk8oAqBXWt3TDKGW', N'', N'0391762328', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (379, N'dunva379', N'$2a$11$DVpTVclzqvmehX6pUubMi.AoeC5m12PZad38kN5awRsnc7UcjIetm', N'', N'0703236049', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (380, N'thuctmi380', N'$2a$11$Pam22HHgf81hVJWDW6mAE.JMZYyGy086kHwje0AXPBvZv.Jj4A6ey', N'', N'0777964055', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (381, N'tiennva381', N'$2a$11$tQHq3Bp.1DA8MDMHzSDsYORknPkc53nT4cGTP3MOlcoQBlGTjFr7a', N'', N'0395082527', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (382, N'camvdi382', N'$2a$11$K5xn/y5BF0.9dy7D1lC40eRNsQFKg9E9chM2nDgpjFpB6lPIX0W1C', N'', N'0613569641', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (383, N'duyvva383', N'$2a$11$Cn7TBrQ7Qa2gTKotEiSouOK2Oh2qiAcAgmiFSdWx8NTcUpFcB61NO', N'', N'0463733025', 3, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (384, N'kimnho1', N'$2a$11$EcDIF8znA2ra63ljFhYHEefrhSi2noW7G7Bl/aT.gcj8.qdaZZ7LW', N'ngohoangkim2002222@gmail.com', N'0945761516', 5, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (385, N'kimnho2', N'$2a$11$mQ.j8T5SsnJ8xJzxyHWldufNJj2zPombB92Jf.0lsd808jBHZQBsu', N'ngohoangkim2002@gmail.com', N'0945761523', 5, N'Hoạt động')
GO
INSERT [dbo].[Users] ([UserID], [Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status]) VALUES (386, N'kimfsadasnho1', N'$2a$11$yZPAvIVKcTeleEEcx7laCefyAhV9dvt4Pdpantjo6UTUv5iEDUaN6', N'ngohoangkim20022322@gmail.com', N'0945761616', 5, N'Hoạt động')
GO
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Academic__294C4DA98DD9D1B7]    Script Date: 4/22/2025 2:30:52 AM ******/
ALTER TABLE [dbo].[AcademicYears] ADD UNIQUE NONCLUSTERED 
(
	[YearName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ_Attendance]    Script Date: 4/22/2025 2:30:52 AM ******/
ALTER TABLE [dbo].[Attendances] ADD  CONSTRAINT [UQ_Attendance] UNIQUE NONCLUSTERED 
(
	[StudentID] ASC,
	[TimetableDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Classes__F8BF561BB87350E5]    Script Date: 4/22/2025 2:30:52 AM ******/
ALTER TABLE [dbo].[Classes] ADD UNIQUE NONCLUSTERED 
(
	[ClassName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ_Conduct_Student_Semester]    Script Date: 4/22/2025 2:30:52 AM ******/
ALTER TABLE [dbo].[Conducts] ADD  CONSTRAINT [UQ_Conduct_Student_Semester] UNIQUE NONCLUSTERED 
(
	[StudentId] ASC,
	[SemesterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ_GradeLevel_Subject]    Script Date: 4/22/2025 2:30:52 AM ******/
ALTER TABLE [dbo].[GradeLevelSubjects] ADD  CONSTRAINT [UQ_GradeLevel_Subject] UNIQUE NONCLUSTERED 
(
	[GradeLevelID] ASC,
	[SubjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ_HomeroomAssignments]    Script Date: 4/22/2025 2:30:52 AM ******/
ALTER TABLE [dbo].[HomeroomAssignments] ADD  CONSTRAINT [UQ_HomeroomAssignments] UNIQUE NONCLUSTERED 
(
	[ClassID] ASC,
	[SemesterID] ASC,
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ__Parents__1788CCADA3226877]    Script Date: 4/22/2025 2:30:52 AM ******/
ALTER TABLE [dbo].[Parents] ADD UNIQUE NONCLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Roles__8A2B6160B3331CEA]    Script Date: 4/22/2025 2:30:52 AM ******/
ALTER TABLE [dbo].[Roles] ADD UNIQUE NONCLUSTERED 
(
	[RoleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ_Semesters]    Script Date: 4/22/2025 2:30:52 AM ******/
ALTER TABLE [dbo].[Semesters] ADD  CONSTRAINT [UQ_Semesters] UNIQUE NONCLUSTERED 
(
	[AcademicYearID] ASC,
	[SemesterName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ__StudentC__3E91EDDAFE2DCA81]    Script Date: 4/22/2025 2:30:52 AM ******/
ALTER TABLE [dbo].[StudentClasses] ADD UNIQUE NONCLUSTERED 
(
	[StudentID] ASC,
	[AcademicYearID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Students__2CEB98367381393B]    Script Date: 4/22/2025 2:30:52 AM ******/
ALTER TABLE [dbo].[Students] ADD UNIQUE NONCLUSTERED 
(
	[IDCardNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Subjects__4C5A7D55E8D00C24]    Script Date: 4/22/2025 2:30:52 AM ******/
ALTER TABLE [dbo].[Subjects] ADD UNIQUE NONCLUSTERED 
(
	[SubjectName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ__Teachers__1788CCADB780D06A]    Script Date: 4/22/2025 2:30:52 AM ******/
ALTER TABLE [dbo].[Teachers] ADD UNIQUE NONCLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Teachers__2CEB9836D2317A72]    Script Date: 4/22/2025 2:30:52 AM ******/
ALTER TABLE [dbo].[Teachers] ADD UNIQUE NONCLUSTERED 
(
	[IDCardNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Users__536C85E4384D0910]    Script Date: 4/22/2025 2:30:52 AM ******/
ALTER TABLE [dbo].[Users] ADD UNIQUE NONCLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Users__536C85E4D7D5788A]    Script Date: 4/22/2025 2:30:52 AM ******/
ALTER TABLE [dbo].[Users] ADD UNIQUE NONCLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Attendances] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Classes] ADD  DEFAULT (N'Hoạt động') FOR [Status]
GO
ALTER TABLE [dbo].[ExamProposals] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[ExamProposals] ADD  DEFAULT ('Ch? duy?t') FOR [Status]
GO
ALTER TABLE [dbo].[GradeBatches] ADD  DEFAULT (N'Hoạt động') FOR [Status]
GO
ALTER TABLE [dbo].[GradeLevelSubjects] ADD  DEFAULT ((0)) FOR [PeriodsPerWeek_HKI]
GO
ALTER TABLE [dbo].[GradeLevelSubjects] ADD  DEFAULT ((0)) FOR [PeriodsPerWeek_HKII]
GO
ALTER TABLE [dbo].[GradeLevelSubjects] ADD  DEFAULT ((0)) FOR [ContinuousAssessments_HKI]
GO
ALTER TABLE [dbo].[GradeLevelSubjects] ADD  DEFAULT ((0)) FOR [ContinuousAssessments_HKII]
GO
ALTER TABLE [dbo].[GradeLevelSubjects] ADD  DEFAULT ((0)) FOR [MidtermAssessments]
GO
ALTER TABLE [dbo].[GradeLevelSubjects] ADD  DEFAULT ((0)) FOR [FinalAssessments]
GO
ALTER TABLE [dbo].[HomeroomAssignments] ADD  DEFAULT (N'Hoạt động') FOR [Status]
GO
ALTER TABLE [dbo].[LessonPlans] ADD  DEFAULT (getdate()) FOR [SubmittedDate]
GO
ALTER TABLE [dbo].[Notifications] ADD  DEFAULT (getdate()) FOR [CreateDate]
GO
ALTER TABLE [dbo].[Notifications] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Students] ADD  DEFAULT ((0)) FOR [RepeatingYear]
GO
ALTER TABLE [dbo].[Students] ADD  DEFAULT (N'Đang học') FOR [Status]
GO
ALTER TABLE [dbo].[SubstituteTeachings] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Teachers] ADD  DEFAULT ((0)) FOR [IsHeadOfDepartment]
GO
ALTER TABLE [dbo].[TeacherSubjects] ADD  DEFAULT ((0)) FOR [IsMainSubject]
GO
ALTER TABLE [dbo].[Timetables] ADD  DEFAULT (N'Hoạt động') FOR [Status]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (N'Hoạt động') FOR [Status]
GO
ALTER TABLE [dbo].[Attendances]  WITH CHECK ADD  CONSTRAINT [FK_Attendances_Students] FOREIGN KEY([StudentID])
REFERENCES [dbo].[Students] ([StudentID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Attendances] CHECK CONSTRAINT [FK_Attendances_Students]
GO
ALTER TABLE [dbo].[Attendances]  WITH CHECK ADD  CONSTRAINT [FK_Attendances_TimetableDetails] FOREIGN KEY([TimetableDetailId])
REFERENCES [dbo].[TimetableDetails] ([TimetableDetailId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Attendances] CHECK CONSTRAINT [FK_Attendances_TimetableDetails]
GO
ALTER TABLE [dbo].[Classes]  WITH CHECK ADD  CONSTRAINT [FK_Classes_GradeLevels] FOREIGN KEY([GradeLevelId])
REFERENCES [dbo].[GradeLevels] ([GradeLevelId])
GO
ALTER TABLE [dbo].[Classes] CHECK CONSTRAINT [FK_Classes_GradeLevels]
GO
ALTER TABLE [dbo].[Conducts]  WITH CHECK ADD  CONSTRAINT [FK_Conducts_Semesters] FOREIGN KEY([SemesterId])
REFERENCES [dbo].[Semesters] ([SemesterID])
GO
ALTER TABLE [dbo].[Conducts] CHECK CONSTRAINT [FK_Conducts_Semesters]
GO
ALTER TABLE [dbo].[Conducts]  WITH CHECK ADD  CONSTRAINT [FK_Conducts_Students] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Students] ([StudentID])
GO
ALTER TABLE [dbo].[Conducts] CHECK CONSTRAINT [FK_Conducts_Students]
GO
ALTER TABLE [dbo].[ExamProposals]  WITH CHECK ADD  CONSTRAINT [FK_ExamProposals_Semesters] FOREIGN KEY([SemesterID])
REFERENCES [dbo].[Semesters] ([SemesterID])
GO
ALTER TABLE [dbo].[ExamProposals] CHECK CONSTRAINT [FK_ExamProposals_Semesters]
GO
ALTER TABLE [dbo].[ExamProposals]  WITH CHECK ADD  CONSTRAINT [FK_ExamProposals_Subjects] FOREIGN KEY([SubjectID])
REFERENCES [dbo].[Subjects] ([SubjectID])
GO
ALTER TABLE [dbo].[ExamProposals] CHECK CONSTRAINT [FK_ExamProposals_Subjects]
GO
ALTER TABLE [dbo].[ExamProposals]  WITH CHECK ADD  CONSTRAINT [FK_ExamProposals_Teachers] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[Teachers] ([TeacherID])
GO
ALTER TABLE [dbo].[ExamProposals] CHECK CONSTRAINT [FK_ExamProposals_Teachers]
GO
ALTER TABLE [dbo].[GradeBatches]  WITH CHECK ADD  CONSTRAINT [FK_GradeBatches_Semesters] FOREIGN KEY([SemesterID])
REFERENCES [dbo].[Semesters] ([SemesterID])
GO
ALTER TABLE [dbo].[GradeBatches] CHECK CONSTRAINT [FK_GradeBatches_Semesters]
GO
ALTER TABLE [dbo].[GradeLevelSubjects]  WITH CHECK ADD  CONSTRAINT [FK_GLS_GradeLevel] FOREIGN KEY([GradeLevelID])
REFERENCES [dbo].[GradeLevels] ([GradeLevelId])
GO
ALTER TABLE [dbo].[GradeLevelSubjects] CHECK CONSTRAINT [FK_GLS_GradeLevel]
GO
ALTER TABLE [dbo].[GradeLevelSubjects]  WITH CHECK ADD  CONSTRAINT [FK_GLS_Subject] FOREIGN KEY([SubjectID])
REFERENCES [dbo].[Subjects] ([SubjectID])
GO
ALTER TABLE [dbo].[GradeLevelSubjects] CHECK CONSTRAINT [FK_GLS_Subject]
GO
ALTER TABLE [dbo].[Grades]  WITH CHECK ADD FOREIGN KEY([AssignmentID])
REFERENCES [dbo].[TeachingAssignments] ([AssignmentID])
GO
ALTER TABLE [dbo].[Grades]  WITH CHECK ADD FOREIGN KEY([BatchID])
REFERENCES [dbo].[GradeBatches] ([BatchID])
GO
ALTER TABLE [dbo].[Grades]  WITH CHECK ADD FOREIGN KEY([StudentClassID])
REFERENCES [dbo].[StudentClasses] ([ID])
GO
ALTER TABLE [dbo].[HomeroomAssignments]  WITH CHECK ADD  CONSTRAINT [FK_HomeroomAssignments_Classes] FOREIGN KEY([ClassID])
REFERENCES [dbo].[Classes] ([ClassID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[HomeroomAssignments] CHECK CONSTRAINT [FK_HomeroomAssignments_Classes]
GO
ALTER TABLE [dbo].[HomeroomAssignments]  WITH CHECK ADD  CONSTRAINT [FK_HomeroomAssignments_Semesters] FOREIGN KEY([SemesterID])
REFERENCES [dbo].[Semesters] ([SemesterID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[HomeroomAssignments] CHECK CONSTRAINT [FK_HomeroomAssignments_Semesters]
GO
ALTER TABLE [dbo].[HomeroomAssignments]  WITH CHECK ADD  CONSTRAINT [FK_HomeroomAssignments_Teachers] FOREIGN KEY([TeacherID])
REFERENCES [dbo].[Teachers] ([TeacherID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[HomeroomAssignments] CHECK CONSTRAINT [FK_HomeroomAssignments_Teachers]
GO
ALTER TABLE [dbo].[LeaveRequests]  WITH CHECK ADD  CONSTRAINT [FK_LeaveRequests_Teachers] FOREIGN KEY([TeacherID])
REFERENCES [dbo].[Teachers] ([TeacherID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[LeaveRequests] CHECK CONSTRAINT [FK_LeaveRequests_Teachers]
GO
ALTER TABLE [dbo].[LessonPlans]  WITH CHECK ADD  CONSTRAINT [FK_LessonPlans_Reviewer_Teachers] FOREIGN KEY([ReviewerId])
REFERENCES [dbo].[Teachers] ([TeacherID])
GO
ALTER TABLE [dbo].[LessonPlans] CHECK CONSTRAINT [FK_LessonPlans_Reviewer_Teachers]
GO
ALTER TABLE [dbo].[LessonPlans]  WITH CHECK ADD  CONSTRAINT [FK_LessonPlans_Semesters] FOREIGN KEY([SemesterID])
REFERENCES [dbo].[Semesters] ([SemesterID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[LessonPlans] CHECK CONSTRAINT [FK_LessonPlans_Semesters]
GO
ALTER TABLE [dbo].[LessonPlans]  WITH CHECK ADD  CONSTRAINT [FK_LessonPlans_Subjects] FOREIGN KEY([SubjectID])
REFERENCES [dbo].[Subjects] ([SubjectID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[LessonPlans] CHECK CONSTRAINT [FK_LessonPlans_Subjects]
GO
ALTER TABLE [dbo].[LessonPlans]  WITH CHECK ADD  CONSTRAINT [FK_LessonPlans_Teachers] FOREIGN KEY([TeacherID])
REFERENCES [dbo].[Teachers] ([TeacherID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[LessonPlans] CHECK CONSTRAINT [FK_LessonPlans_Teachers]
GO
ALTER TABLE [dbo].[Parents]  WITH CHECK ADD  CONSTRAINT [FK_Parents_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[Parents] CHECK CONSTRAINT [FK_Parents_Users]
GO
ALTER TABLE [dbo].[Semesters]  WITH CHECK ADD  CONSTRAINT [FK_Semesters_AcademicYears] FOREIGN KEY([AcademicYearID])
REFERENCES [dbo].[AcademicYears] ([AcademicYearID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Semesters] CHECK CONSTRAINT [FK_Semesters_AcademicYears]
GO
ALTER TABLE [dbo].[StudentClasses]  WITH CHECK ADD  CONSTRAINT [FK_StudentClasses_AcademicYears] FOREIGN KEY([AcademicYearID])
REFERENCES [dbo].[AcademicYears] ([AcademicYearID])
GO
ALTER TABLE [dbo].[StudentClasses] CHECK CONSTRAINT [FK_StudentClasses_AcademicYears]
GO
ALTER TABLE [dbo].[StudentClasses]  WITH CHECK ADD  CONSTRAINT [FK_StudentClasses_Classes] FOREIGN KEY([ClassID])
REFERENCES [dbo].[Classes] ([ClassID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StudentClasses] CHECK CONSTRAINT [FK_StudentClasses_Classes]
GO
ALTER TABLE [dbo].[StudentClasses]  WITH CHECK ADD  CONSTRAINT [FK_StudentClasses_Students] FOREIGN KEY([StudentID])
REFERENCES [dbo].[Students] ([StudentID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StudentClasses] CHECK CONSTRAINT [FK_StudentClasses_Students]
GO
ALTER TABLE [dbo].[Students]  WITH CHECK ADD  CONSTRAINT [FK_Students_Parents] FOREIGN KEY([ParentID])
REFERENCES [dbo].[Parents] ([ParentID])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Students] CHECK CONSTRAINT [FK_Students_Parents]
GO
ALTER TABLE [dbo].[SubstituteTeachings]  WITH CHECK ADD FOREIGN KEY([OriginalTeacherId])
REFERENCES [dbo].[Teachers] ([TeacherID])
GO
ALTER TABLE [dbo].[SubstituteTeachings]  WITH CHECK ADD FOREIGN KEY([SubstituteTeacherId])
REFERENCES [dbo].[Teachers] ([TeacherID])
GO
ALTER TABLE [dbo].[SubstituteTeachings]  WITH CHECK ADD FOREIGN KEY([TimetableDetailId])
REFERENCES [dbo].[TimetableDetails] ([TimetableDetailId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Teachers]  WITH CHECK ADD  CONSTRAINT [FK_Teachers_Users] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Teachers] CHECK CONSTRAINT [FK_Teachers_Users]
GO
ALTER TABLE [dbo].[TeacherSubjects]  WITH CHECK ADD  CONSTRAINT [FK_TeacherSubjects_Subjects] FOREIGN KEY([SubjectID])
REFERENCES [dbo].[Subjects] ([SubjectID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TeacherSubjects] CHECK CONSTRAINT [FK_TeacherSubjects_Subjects]
GO
ALTER TABLE [dbo].[TeacherSubjects]  WITH CHECK ADD  CONSTRAINT [FK_TeacherSubjects_Teachers] FOREIGN KEY([TeacherID])
REFERENCES [dbo].[Teachers] ([TeacherID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TeacherSubjects] CHECK CONSTRAINT [FK_TeacherSubjects_Teachers]
GO
ALTER TABLE [dbo].[TeachingAssignments]  WITH CHECK ADD  CONSTRAINT [FK_TeachingAssignments_Classes] FOREIGN KEY([ClassID])
REFERENCES [dbo].[Classes] ([ClassID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TeachingAssignments] CHECK CONSTRAINT [FK_TeachingAssignments_Classes]
GO
ALTER TABLE [dbo].[TeachingAssignments]  WITH CHECK ADD  CONSTRAINT [FK_TeachingAssignments_Semesters] FOREIGN KEY([SemesterID])
REFERENCES [dbo].[Semesters] ([SemesterID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TeachingAssignments] CHECK CONSTRAINT [FK_TeachingAssignments_Semesters]
GO
ALTER TABLE [dbo].[TeachingAssignments]  WITH CHECK ADD  CONSTRAINT [FK_TeachingAssignments_Subjects] FOREIGN KEY([SubjectID])
REFERENCES [dbo].[Subjects] ([SubjectID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TeachingAssignments] CHECK CONSTRAINT [FK_TeachingAssignments_Subjects]
GO
ALTER TABLE [dbo].[TeachingAssignments]  WITH CHECK ADD  CONSTRAINT [FK_TeachingAssignments_Teachers] FOREIGN KEY([TeacherID])
REFERENCES [dbo].[Teachers] ([TeacherID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TeachingAssignments] CHECK CONSTRAINT [FK_TeachingAssignments_Teachers]
GO
ALTER TABLE [dbo].[TimetableDetails]  WITH CHECK ADD FOREIGN KEY([ClassId])
REFERENCES [dbo].[Classes] ([ClassID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TimetableDetails]  WITH CHECK ADD FOREIGN KEY([PeriodId])
REFERENCES [dbo].[Periods] ([PeriodId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TimetableDetails]  WITH CHECK ADD FOREIGN KEY([SubjectId])
REFERENCES [dbo].[Subjects] ([SubjectID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TimetableDetails]  WITH CHECK ADD FOREIGN KEY([TeacherId])
REFERENCES [dbo].[Teachers] ([TeacherID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TimetableDetails]  WITH CHECK ADD FOREIGN KEY([TimetableId])
REFERENCES [dbo].[Timetables] ([TimetableId])
GO
ALTER TABLE [dbo].[Timetables]  WITH CHECK ADD FOREIGN KEY([SemesterId])
REFERENCES [dbo].[Semesters] ([SemesterID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Roles] FOREIGN KEY([RoleID])
REFERENCES [dbo].[Roles] ([RoleID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Roles]
GO
ALTER TABLE [dbo].[Attendances]  WITH CHECK ADD CHECK  (([Status]='X' OR [Status]='K' OR [Status]='P' OR [Status]='C'))
GO
ALTER TABLE [dbo].[Classes]  WITH CHECK ADD CHECK  (([Status]=N'Không Hoạt động' OR [Status]=N'Hoạt động'))
GO
ALTER TABLE [dbo].[Conducts]  WITH CHECK ADD CHECK  (([ConductType]=N'Yếu' OR [ConductType]=N'Trung bình' OR [ConductType]=N'Khá' OR [ConductType]=N'Tốt'))
GO
ALTER TABLE [dbo].[HomeroomAssignments]  WITH CHECK ADD CHECK  (([Status]=N'Không Hoạt động' OR [Status]=N'Hoạt động'))
GO
ALTER TABLE [dbo].[Periods]  WITH CHECK ADD  CONSTRAINT [CHK_EndTime_After_StartTime] CHECK  (([EndTime]>[StartTime]))
GO
ALTER TABLE [dbo].[Periods] CHECK CONSTRAINT [CHK_EndTime_After_StartTime]
GO
ALTER TABLE [dbo].[Periods]  WITH CHECK ADD CHECK  (([Shift]=(2) OR [Shift]=(1)))
GO
ALTER TABLE [dbo].[Students]  WITH CHECK ADD  CONSTRAINT [CHK_Student_Status] CHECK  (([Status]=N'Chuyển trường' OR [Status]=N'Tốt nghiệp' OR [Status]=N'Nghỉ học' OR [Status]=N'Bảo lưu' OR [Status]=N'Đang học'))
GO
ALTER TABLE [dbo].[Students] CHECK CONSTRAINT [CHK_Student_Status]
GO
ALTER TABLE [dbo].[Subjects]  WITH CHECK ADD CHECK  (([TypeOfGrade]=N'Nhận xét' OR [TypeOfGrade]=N'Tính điểm'))
GO
ALTER TABLE [dbo].[TimetableDetails]  WITH CHECK ADD CHECK  (([DayOfWeek]=N'Chủ Nhật' OR [DayOfWeek]=N'Thứ Bảy' OR [DayOfWeek]=N'Thứ Sáu' OR [DayOfWeek]=N'Thứ Năm' OR [DayOfWeek]=N'Thứ Tư' OR [DayOfWeek]=N'Thứ Ba' OR [DayOfWeek]=N'Thứ Hai'))
GO
USE [master]
GO
ALTER DATABASE [HGSDB] SET  READ_WRITE 
GO
