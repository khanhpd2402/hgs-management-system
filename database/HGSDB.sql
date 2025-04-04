USE [master]
GO

CREATE DATABASE [HGSDB]
GO

USE [HGSDB]
SET QUOTED_IDENTIFIER ON

-- Tạo các bảng cơ bản không phụ thuộc
CREATE TABLE [dbo].[AcademicYears] (
    [AcademicYearID] INT IDENTITY(1,1) NOT NULL,
    [YearName] VARCHAR(9) NOT NULL,
    [StartDate] DATE NOT NULL,
    [EndDate] DATE NOT NULL,
    PRIMARY KEY CLUSTERED ([AcademicYearID] ASC),
    UNIQUE NONCLUSTERED ([YearName] ASC)
)

CREATE TABLE [dbo].[Semesters] (
    [SemesterID] INT IDENTITY(1,1) NOT NULL,
    [AcademicYearID] INT NOT NULL ,
    [SemesterName] NVARCHAR(20) NOT NULL,
    [StartDate] DATE NOT NULL,
    [EndDate] DATE NOT NULL,
    PRIMARY KEY CLUSTERED ([SemesterID] ASC),
    CONSTRAINT [FK_Semesters_AcademicYears] FOREIGN KEY ([AcademicYearID]) REFERENCES [dbo].[AcademicYears] ([AcademicYearID]) ON DELETE CASCADE,
    CONSTRAINT [UQ_Semesters] UNIQUE ([AcademicYearID], [SemesterName])
)

CREATE TABLE [dbo].[GradeLevels] (
    [GradeLevelId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [GradeName] NVARCHAR(20) NOT NULL
);


CREATE TABLE [dbo].[Classes] (
    [ClassID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ClassName] NVARCHAR(50) NOT NULL UNIQUE,
    [GradeLevelId] INT NOT NULL,
    CONSTRAINT FK_Classes_GradeLevels FOREIGN KEY ([GradeLevelId]) REFERENCES [GradeLevels]([GradeLevelId])
);
CREATE TABLE GradeLevelSubjects (
    GradeLevelSubjectID INT IDENTITY(1,1) PRIMARY KEY,
    GradeLevelID INT NOT NULL,
    SubjectID INT NOT NULL,

    PeriodsPerWeek_HKI INT NOT NULL DEFAULT 0,
    PeriodsPerWeek_HKII INT NOT NULL DEFAULT 0,

    ContinuousAssessments_HKI INT NOT NULL DEFAULT 0,
    ContinuousAssessments_HKII INT NOT NULL DEFAULT 0,

    CONSTRAINT FK_GLS_GradeLevel FOREIGN KEY (GradeLevelID)
        REFERENCES GradeLevels(GradeLevelID),
    CONSTRAINT FK_GLS_Subject FOREIGN KEY (SubjectID)
        REFERENCES Subjects(SubjectID),
    CONSTRAINT UQ_GradeLevel_Subject UNIQUE (GradeLevelID, SubjectID) -- để tránh nhập trùng
);

CREATE TABLE [dbo].[Subjects] (
    [SubjectID] INT IDENTITY(1,1) NOT NULL,
    [SubjectName] NVARCHAR(100) NOT NULL,
    [SubjectCategory] NVARCHAR(50) NOT NULL,
    [TypeOfGrade] NVARCHAR(50) NOT NULL CHECK ([TypeOfGrade] IN (N'	Tính điểm', N'Nhận xét')),
    PRIMARY KEY CLUSTERED ([SubjectID] ASC),
    UNIQUE NONCLUSTERED ([SubjectName] ASC)
)

CREATE TABLE [dbo].[Roles] (
    [RoleID] INT IDENTITY(1,1) NOT NULL,
    [RoleName] NVARCHAR(50) NOT NULL,
    PRIMARY KEY CLUSTERED ([RoleID] ASC),
    UNIQUE NONCLUSTERED ([RoleName] ASC)
)

CREATE TABLE [dbo].[Users] (
    [UserID] INT IDENTITY(1,1) NOT NULL,
    [Username] NVARCHAR(50) UNIQUE NOT NULL,
    [PasswordHash] NVARCHAR(255) NOT NULL,
    [Email] NVARCHAR(100) NULL,
    [PhoneNumber] NVARCHAR(15) NULL,
    [RoleID] INT NOT NULL,
    [Status] NVARCHAR(20) NULL DEFAULT N'Hoạt Động',
    PRIMARY KEY CLUSTERED ([UserID] ASC),
    CONSTRAINT [FK_Users_Roles] FOREIGN KEY ([RoleID]) REFERENCES [dbo].[Roles] ([RoleID]) ON DELETE CASCADE,
    UNIQUE NONCLUSTERED ([Username] ASC)
);
CREATE NONCLUSTERED INDEX [IX_Users_Email] ON [dbo].[Users] ([Email]);
CREATE NONCLUSTERED INDEX [IX_Users_PhoneNumber] ON [dbo].[Users] ([PhoneNumber]);
CREATE TABLE [dbo].[Teachers] (
    [TeacherID] INT IDENTITY(1,1) NOT NULL,
    [UserID] INT NULL, -- Liên kết với bảng Users
    [FullName] NVARCHAR(100) NOT NULL, -- Họ và tên
    [DOB] DATE NOT NULL, -- Ngày sinh
    [Gender] NVARCHAR(10) NOT NULL, -- Giới tính
    [Ethnicity] NVARCHAR(50) NULL, -- Dân tộc
    [Religion] NVARCHAR(50) NULL, -- Tôn giáo
    [MaritalStatus] NVARCHAR(50) NULL, -- Tình trạng hôn nhân
    [IDCardNumber] NVARCHAR(20) NULL, -- CMND/CCCD
    [InsuranceNumber] NVARCHAR(20) NULL, -- Số sổ bảo hiểm
    [EmploymentType] NVARCHAR(100) NULL, -- Hình thức hợp đồng
    [Position] NVARCHAR(100) NULL, -- Vị trí việc làm
    [Department] NVARCHAR(100) NULL, -- Tổ bộ môn
    [MainSubject] NVARCHAR(100) NULL, -- Môn dạy chính (thay cho AdditionalDuties)
    [IsHeadOfDepartment] BIT NULL DEFAULT 0, -- Là tổ trưởng
    [EmploymentStatus] NVARCHAR(50) NULL, -- Trạng thái cán bộ
    [RecruitmentAgency] NVARCHAR(255) NULL, -- Cơ quan tuyển dụng
    [HiringDate] DATE NULL, -- Ngày tuyển dụng
    [PermanentEmploymentDate] DATE NULL, -- Ngày vào biên chế
    [SchoolJoinDate] DATE NOT NULL, -- Ngày vào trường
    [PermanentAddress] NVARCHAR(255) NULL, -- Địa chỉ thường trú
    [Hometown] NVARCHAR(255) NULL, -- Quê quán
    PRIMARY KEY CLUSTERED ([TeacherID] ASC),
    CONSTRAINT [FK_Teachers_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([UserID]) ON DELETE CASCADE,
    UNIQUE NONCLUSTERED ([IDCardNumber] ASC),
    UNIQUE NONCLUSTERED ([UserID] ASC)
);
CREATE TABLE [dbo].[Parents] (
    [ParentID] INT IDENTITY(1,1) NOT NULL,
    [UserID] INT NULL,
    [FullNameFather] NVARCHAR(100) NULL,
    [YearOfBirthFather] DATE NULL,
    [OccupationFather] NVARCHAR(100) NULL,
    [PhoneNumberFather] NVARCHAR(15) NULL,
    [EmailFather] NVARCHAR(100) NULL,
    [IdcardNumberFather] NVARCHAR(50) NULL,
    [FullNameMother] NVARCHAR(100) NULL,
    [YearOfBirthMother] DATE NULL,
    [OccupationMother] NVARCHAR(100) NULL,
    [PhoneNumberMother] NVARCHAR(15) NULL,
    [EmailMother] NVARCHAR(100) NULL,
    [IdcardNumberMother] NVARCHAR(50) NULL,
    [FullNameGuardian] NVARCHAR(100) NULL,
    [YearOfBirthGuardian] DATE NULL,
    [OccupationGuardian] NVARCHAR(100) NULL,
    [PhoneNumberGuardian] NVARCHAR(15) NULL,
    [EmailGuardian] NVARCHAR(100) NULL,
    [IdcardNumberGuardian] NVARCHAR(50) NULL,
    PRIMARY KEY CLUSTERED ([ParentID] ASC),
    UNIQUE NONCLUSTERED ([UserID] ASC),
	CONSTRAINT FK_Parents_Users FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users]([UserID])
);

CREATE TABLE [dbo].[Students] (
    [StudentID] INT IDENTITY(1,1) NOT NULL,
    [FullName] NVARCHAR(100) NOT NULL,
    [DOB] DATE NOT NULL,
    [Gender] NVARCHAR(10) NOT NULL,
    [AdmissionDate] DATE NOT NULL,
    [EnrollmentType] NVARCHAR(50) NULL,
    [Ethnicity] NVARCHAR(50) NULL,
    [PermanentAddress] NVARCHAR(255) NULL,
    [BirthPlace] NVARCHAR(255) NULL,
    [Religion] NVARCHAR(50) NULL,
    [RepeatingYear] BIT NULL DEFAULT 0,
    [IDCardNumber] NVARCHAR(20) NULL,
    [Status] NVARCHAR(50) NULL DEFAULT N'Đang học',
    [ParentID] INT NULL,
    PRIMARY KEY CLUSTERED ([StudentID] ASC),
    UNIQUE NONCLUSTERED ([IDCardNumber] ASC),
    CONSTRAINT [FK_Students_Parents] FOREIGN KEY ([ParentID]) REFERENCES [dbo].[Parents] ([ParentID]) ON DELETE SET NULL
)

-- Tạo chỉ mục UNIQUE cho các trường có giá trị không NULL sau khi tạo bảng
CREATE UNIQUE NONCLUSTERED INDEX [UQ_Parents_IdcardNumberFather]
ON [dbo].[Parents] ([IdcardNumberFather])
WHERE [IdcardNumberFather] IS NOT NULL;

CREATE UNIQUE NONCLUSTERED INDEX [UQ_Parents_IdcardNumberMother]
ON [dbo].[Parents] ([IdcardNumberMother])
WHERE [IdcardNumberMother] IS NOT NULL;

CREATE UNIQUE NONCLUSTERED INDEX [UQ_Parents_IdcardNumberGuardian]
ON [dbo].[Parents] ([IdcardNumberGuardian])
WHERE [IdcardNumberGuardian] IS NOT NULL;

CREATE UNIQUE NONCLUSTERED INDEX [UQ_Parents_EmailFather]
ON [dbo].[Parents] ([EmailFather])
WHERE [EmailFather] IS NOT NULL;

CREATE UNIQUE NONCLUSTERED INDEX [UQ_Parents_EmailMother]
ON [dbo].[Parents] ([EmailMother])
WHERE [EmailMother] IS NOT NULL;

CREATE UNIQUE NONCLUSTERED INDEX [UQ_Parents_EmailGuardian]
ON [dbo].[Parents] ([EmailGuardian])
WHERE [EmailGuardian] IS NOT NULL;

-- Tạo các bảng phụ thuộc
CREATE TABLE [dbo].[StudentClasses] (
    [ID] INT IDENTITY(1,1) NOT NULL,
    [StudentID] INT NOT NULL,
    [ClassID] INT NOT NULL,
    [AcademicYearID] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_StudentClasses_Students] FOREIGN KEY ([StudentID]) REFERENCES [dbo].[Students] ([StudentID]) ON DELETE CASCADE,
    CONSTRAINT [FK_StudentClasses_Classes] FOREIGN KEY ([ClassID]) REFERENCES [dbo].[Classes] ([ClassID]) ON DELETE CASCADE,
    CONSTRAINT [FK_StudentClasses_AcademicYears] FOREIGN KEY ([AcademicYearID]) REFERENCES [dbo].[AcademicYears] ([AcademicYearID]),
    UNIQUE NONCLUSTERED ([StudentID], [AcademicYearID])
)

CREATE TABLE [dbo].[TeacherClasses] (
    [ID] INT IDENTITY(1,1) NOT NULL,
    [TeacherID] INT NOT NULL,
    [ClassID] INT NOT NULL,
    [IsHomeroomTeacher] BIT NULL DEFAULT 0,
    [AcademicYearID] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_TeacherClasses_Teachers] FOREIGN KEY ([TeacherID]) REFERENCES [dbo].[Teachers] ([TeacherID]) ON DELETE CASCADE,
    CONSTRAINT [FK_TeacherClasses_Classes] FOREIGN KEY ([ClassID]) REFERENCES [dbo].[Classes] ([ClassID]) ON DELETE CASCADE,
    CONSTRAINT [FK_TeacherClasses_AcademicYears] FOREIGN KEY ([AcademicYearID]) REFERENCES [dbo].[AcademicYears] ([AcademicYearID])
) 

CREATE TABLE [dbo].[TeacherSubjects] (
    [ID] INT IDENTITY(1,1) NOT NULL,
    [TeacherID] INT,
    [SubjectID] INT,
    [IsMainSubject] BIT NULL DEFAULT 0,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_TeacherSubjects_Teachers] FOREIGN KEY ([TeacherID]) REFERENCES [dbo].[Teachers] ([TeacherID]) ON DELETE CASCADE,
    CONSTRAINT [FK_TeacherSubjects_Subjects] FOREIGN KEY ([SubjectID]) REFERENCES [dbo].[Subjects] ([SubjectID]) ON DELETE CASCADE
)
CREATE TABLE [dbo].[TeachingAssignments] (
    [AssignmentID] INT IDENTITY(1,1) NOT NULL,
    [TeacherID] INT NOT NULL,
    [SubjectID] INT NOT NULL,
    [ClassID] INT NOT NULL,
    [SemesterID] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([AssignmentID] ASC),
    CONSTRAINT [FK_TeachingAssignments_Teachers] FOREIGN KEY ([TeacherID]) REFERENCES [dbo].[Teachers] ([TeacherID]) ON DELETE CASCADE,
    CONSTRAINT [FK_TeachingAssignments_Subjects] FOREIGN KEY ([SubjectID]) REFERENCES [dbo].[Subjects] ([SubjectID]) ON DELETE CASCADE,
    CONSTRAINT [FK_TeachingAssignments_Classes] FOREIGN KEY ([ClassID]) REFERENCES [dbo].[Classes] ([ClassID]) ON DELETE CASCADE,
    CONSTRAINT [FK_TeachingAssignments_Semesters] FOREIGN KEY ([SemesterID]) REFERENCES [dbo].[Semesters] ([SemesterID]) ON DELETE CASCADE
)
CREATE TABLE GradeBatches (
    BatchID INT PRIMARY KEY IDENTITY(1,1),
	[BatchName] NVARCHAR(255) NOT NULL,
	[SemesterID] INT NOT NULL,
	StartDate DATE,
    EndDate DATE,
	[Status] NVARCHAR(10) NOT NULL DEFAULT N'Active'
	CONSTRAINT [FK_GradeBatches_Semesters] FOREIGN KEY ([SemesterID]) REFERENCES [dbo].[Semesters] ([SemesterID])
)

CREATE TABLE [dbo].[Grades] (
    [GradeID] INT IDENTITY(1,1) NOT NULL,
    [BatchID] INT NULL,
    [StudentClassID] INT NULL,
    [AssignmentID] INT NULL,
    [Score] NVARCHAR(10) NULL,
    [AssessmentsTypeName] NVARCHAR(100) NOT NULL,
    [TeacherComment] NVARCHAR(MAX) NULL,
    PRIMARY KEY CLUSTERED ([GradeID] ASC),
    FOREIGN KEY ([BatchID]) REFERENCES [dbo].[GradeBatches ] ([BatchID]),
    FOREIGN KEY ([StudentClassID]) REFERENCES [dbo].[StudentClasses] ([ID]) ON DELETE NO ACTION,
    FOREIGN KEY ([AssignmentID]) REFERENCES [dbo].[TeachingAssignments] ([AssignmentID]) ON DELETE NO ACTION
);

CREATE TABLE [dbo].[Exams] (
    [ExamID] INT IDENTITY(1,1) NOT NULL,
    [SubjectID] INT NOT NULL,
    [CreatedBy] INT NOT NULL,
    [ExamContent] NVARCHAR(max) NOT NULL,
    [CreatedDate] DATETIME NULL DEFAULT GETDATE(),
    [SemesterID] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([ExamID] ASC),
    CONSTRAINT [FK_Exams_Subjects] FOREIGN KEY ([SubjectID]) REFERENCES [dbo].[Subjects] ([SubjectID]) ON DELETE CASCADE,
    CONSTRAINT [FK_Exams_Teachers] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Teachers] ([TeacherID]) ON DELETE CASCADE,
    CONSTRAINT [FK_Exams_Semesters] FOREIGN KEY ([SemesterID]) REFERENCES [dbo].[Semesters] ([SemesterID]) ON DELETE CASCADE
)

CREATE TABLE [dbo].[LeaveRequests] (
    [RequestID] INT IDENTITY(1,1) NOT NULL,
    [TeacherID] INT NOT NULL,
    [RequestDate] DATE NOT NULL,
    [LeaveFromDate] DATE NOT NULL,
    [LeaveToDate] DATE NOT NULL,
    [Reason] NVARCHAR(max) NOT NULL,
    [Status] NVARCHAR(20) NULL,
    PRIMARY KEY CLUSTERED ([RequestID] ASC),
    CONSTRAINT [FK_LeaveRequests_Teachers] FOREIGN KEY ([TeacherID]) REFERENCES [dbo].[Teachers] ([TeacherID]) ON DELETE CASCADE
)

CREATE TABLE [dbo].[LessonPlans] (
    [PlanID] INT IDENTITY(1,1) NOT NULL,
    [TeacherID] INT NOT NULL,
    [SubjectID] INT NOT NULL,
    [PlanContent] NVARCHAR(max) NOT NULL,
    [Status] NVARCHAR(20) NULL,
    [SemesterID] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([PlanID] ASC),
    CONSTRAINT [FK_LessonPlans_Teachers] FOREIGN KEY ([TeacherID]) REFERENCES [dbo].[Teachers] ([TeacherID]) ON DELETE CASCADE,
    CONSTRAINT [FK_LessonPlans_Subjects] FOREIGN KEY ([SubjectID]) REFERENCES [dbo].[Subjects] ([SubjectID]) ON DELETE CASCADE,
    CONSTRAINT [FK_LessonPlans_Semesters] FOREIGN KEY ([SemesterID]) REFERENCES [dbo].[Semesters] ([SemesterID]) ON DELETE CASCADE
)

CREATE TABLE [dbo].[Notifications] (
    [NotificationID] INT IDENTITY(1,1) NOT NULL,
    [Title] NVARCHAR(1000) NOT NULL,
    [Message] NVARCHAR(max) NOT NULL,
    [CreateDate] DATETIME NULL DEFAULT GETDATE(),
    [IsActive] BIT NULL DEFAULT 1,
    PRIMARY KEY CLUSTERED ([NotificationID] ASC)
	)

go
CREATE TABLE Timetables (
    TimetableId INT PRIMARY KEY IDENTITY(1,1),
    SemesterId INT NOT NULL,
    EffectiveDate DATE NOT NULL,
	[Status] NVARCHAR(10) NOT NULL DEFAULT N'Active'
    FOREIGN KEY (SemesterId) REFERENCES [Semesters](SemesterId) ON DELETE CASCADE
);
GO
CREATE TABLE TimetableDetails (
    TimetableDetailId INT PRIMARY KEY IDENTITY(1,1),
    TimetableId INT NOT NULL,
    ClassId INT NOT NULL,
    SubjectId INT NOT NULL,
    TeacherId INT NOT NULL,
    DayOfWeek TINYINT NOT NULL CHECK (DayOfWeek BETWEEN 1 AND 7), -- 1: Chủ Nhật, 2: Thứ Hai, ..., 7: Thứ Bảy
    Shift TINYINT NOT NULL CHECK (Shift IN (1, 2)), -- 1: Sáng, 2: Chiều
    Period TINYINT NOT NULL CHECK (Period BETWEEN 1 AND 10), -- Giả sử mỗi ca có tối đa 10 tiết
    FOREIGN KEY (TimetableId) REFERENCES Timetables(TimetableId),
    FOREIGN KEY (ClassId) REFERENCES Classes(ClassId) ON DELETE CASCADE,
    FOREIGN KEY (SubjectId) REFERENCES [Subjects](SubjectId) ON DELETE CASCADE,
    FOREIGN KEY (TeacherId) REFERENCES Teachers(TeacherId) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[Attendances] (
    [AttendanceID] INT IDENTITY(1,1) NOT NULL,
    [StudentID] INT NOT NULL,
    [Date] DATE NOT NULL,
    [Status] NVARCHAR(1) NOT NULL CHECK ([Status] IN ('C', 'P', 'K', 'X')),
    [Note] NVARCHAR(255) NULL,
    [CreatedAt] DATETIME DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [Shift] NVARCHAR(20) NOT NULL CHECK ([Shift] IN (N'Sáng', N'Chiều')),
    [SemesterID] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([AttendanceID] ASC),
    CONSTRAINT [FK_Attendances_Students] FOREIGN KEY ([StudentID]) REFERENCES [dbo].[Students] ([StudentID]) ON DELETE CASCADE,
    CONSTRAINT [FK_Attendances_Teachers] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Teachers] ([TeacherID]) ON DELETE CASCADE,
    CONSTRAINT [FK_Attendances_Semesters] FOREIGN KEY ([SemesterID]) REFERENCES [dbo].[Semesters] ([SemesterID]) ON DELETE CASCADE,
    CONSTRAINT [UQ_Attendance] UNIQUE ([StudentID], [Date], [Shift], [SemesterID])
)
ALTER TABLE [dbo].[LessonPlans]
ADD
    [Title] NVARCHAR(255) NULL,
    [AttachmentUrl] NVARCHAR(500) NULL,
    [Feedback] NVARCHAR(max) NULL,
    [SubmittedDate] DATETIME NULL DEFAULT GETDATE(),
    [ReviewedDate] DATETIME NULL,
    [ReviewerId] INT NULL;

-- Thêm ràng buộc FOREIGN KEY cho ReviewerId
ALTER TABLE [dbo].[LessonPlans]
ADD CONSTRAINT [FK_LessonPlans_Reviewer_Teachers] FOREIGN KEY ([ReviewerId]) REFERENCES [dbo].[Teachers] ([TeacherID]);