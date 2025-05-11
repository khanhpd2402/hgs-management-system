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
