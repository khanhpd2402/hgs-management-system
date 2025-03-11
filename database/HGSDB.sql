USE [master]
GO

-- Tạo cơ sở dữ liệu
CREATE DATABASE [HGSDB]
GO

USE [HGSDB]
GO

SET QUOTED_IDENTIFIER ON
GO

-- Bảng AcademicYears (Quản lý năm học)
CREATE TABLE [dbo].[AcademicYears] (
    [AcademicYearID] INT IDENTITY(1,1) NOT NULL,
    [YearName]       VARCHAR(9)       NOT NULL, -- Ví dụ: '2024-2025'
    [StartDate]      DATE             NOT NULL,
    [EndDate]        DATE             NOT NULL,
    PRIMARY KEY CLUSTERED ([AcademicYearID] ASC),
    UNIQUE NONCLUSTERED ([YearName] ASC)
)
GO

-- Bảng Semesters (Quản lý kỳ học)
CREATE TABLE [dbo].[Semesters] (
    [SemesterID]     INT IDENTITY(1,1) NOT NULL,
    [AcademicYearID] INT               NOT NULL,
    [SemesterName]   NVARCHAR(20)      NOT NULL, -- Ví dụ: 'Học kỳ 1', 'Học kỳ 2'
    [StartDate]      DATE              NOT NULL,
    [EndDate]        DATE              NOT NULL,
    PRIMARY KEY CLUSTERED ([SemesterID] ASC),
    CONSTRAINT [FK_Semesters_AcademicYears] FOREIGN KEY ([AcademicYearID]) REFERENCES [dbo].[AcademicYears] ([AcademicYearID]),
    CONSTRAINT [UQ_Semesters] UNIQUE ([AcademicYearID], [SemesterName])
)
GO

-- Bảng Classes
CREATE TABLE [dbo].[Classes] (
    [ClassID]   INT IDENTITY(1,1) NOT NULL,
    [ClassName] NVARCHAR(50)      NOT NULL,
    [Grade]     INT               NOT NULL,
    PRIMARY KEY CLUSTERED ([ClassID] ASC),
    UNIQUE NONCLUSTERED ([ClassName] ASC)
)
GO

-- Bảng Exams
CREATE TABLE [dbo].[Exams] (
    [ExamID]      INT IDENTITY(1,1) NOT NULL,
    [SubjectID]   INT               NOT NULL,
    [CreatedBy]   INT               NOT NULL,
    [ExamContent] NVARCHAR(1000)    NOT NULL,
    [CreatedDate] DATETIME          NULL DEFAULT GETDATE(),
    [SemesterID]  INT               NOT NULL,
    PRIMARY KEY CLUSTERED ([ExamID] ASC)
)
GO

-- Bảng Grades
CREATE TABLE [dbo].[Grades] (
    [GradeID]   INT IDENTITY(1,1) NOT NULL,
    [StudentID] INT               NOT NULL,
    [SubjectID] INT               NOT NULL,
    [Score]     DECIMAL(4, 2)     NOT NULL CHECK ([Score] >= 0 AND [Score] <= 10),
    [ExamID]    INT               NOT NULL,
    [SemesterID] INT               NOT NULL,
    PRIMARY KEY CLUSTERED ([GradeID] ASC)
)
GO

-- Bảng LeaveRequests
CREATE TABLE [dbo].[LeaveRequests] (
    [RequestID]     INT IDENTITY(1,1) NOT NULL,
    [TeacherID]     INT               NOT NULL,
    [RequestDate]   DATE              NOT NULL,
    [LeaveFromDate] DATE              NOT NULL,
    [LeaveToDate]   DATE              NOT NULL,
    [Reason]        NVARCHAR(1000)    NOT NULL,
    [Status]        NVARCHAR(20)      NULL,
    [ApprovedBy]    INT               NULL,
    PRIMARY KEY CLUSTERED ([RequestID] ASC)
)
GO

-- Bảng LessonPlans
CREATE TABLE [dbo].[LessonPlans] (
    [PlanID]      INT IDENTITY(1,1) NOT NULL,
    [TeacherID]   INT               NOT NULL,
    [SubjectID]   INT               NOT NULL,
    [PlanContent] NVARCHAR(1000)    NOT NULL,
    [Status]      NVARCHAR(20)      NULL,
    [SemesterID]  INT               NOT NULL,
    PRIMARY KEY CLUSTERED ([PlanID] ASC)
)
GO

-- Bảng Notifications
CREATE TABLE [dbo].[Notifications] (
    [NotificationID] INT IDENTITY(1,1) NOT NULL,
    [UserID]         INT               NOT NULL,
    [Message]        NVARCHAR(1000)    NOT NULL,
    [SentDate]       DATETIME          NULL DEFAULT GETDATE(),
    PRIMARY KEY CLUSTERED ([NotificationID] ASC)
)
GO

-- Bảng Parents
CREATE TABLE [dbo].[Parents] (
    [ParentID]     INT IDENTITY(1,1) NOT NULL,
    [UserID]       INT               NULL,
    [FullName]     NVARCHAR(100)     NOT NULL,
    [DOB]          DATE              NULL,
    [Occupation]   NVARCHAR(100)     NULL,
    [Relationship] NVARCHAR(50)      NOT NULL CHECK ([Relationship] IN (N'Bố', N'Mẹ', N'Người bảo hộ')),
    PRIMARY KEY CLUSTERED ([ParentID] ASC),
    UNIQUE NONCLUSTERED ([UserID] ASC)
)
GO

-- Bảng Rewards
CREATE TABLE [dbo].[Rewards] (
    [RewardID]     INT IDENTITY(1,1) NOT NULL,
    [TeacherID]    INT               NOT NULL,
    [RewardType]   NVARCHAR(50)      NULL,
    [DateReceived] DATE              NULL,
    [SemesterID]   INT               NOT NULL,
    PRIMARY KEY CLUSTERED ([RewardID] ASC)
)
GO

-- Bảng Roles
CREATE TABLE [dbo].[Roles] (
    [RoleID]   INT IDENTITY(1,1) NOT NULL,
    [RoleName] NVARCHAR(50)      NOT NULL,
    PRIMARY KEY CLUSTERED ([RoleID] ASC),
    UNIQUE NONCLUSTERED ([RoleName] ASC)
)
GO

-- Bảng StudentParents
CREATE TABLE [dbo].[StudentParents] (
    [StudentID] INT NOT NULL,
    [ParentID]  INT NOT NULL,
    PRIMARY KEY CLUSTERED ([StudentID] ASC, [ParentID] ASC)
)
GO

-- Bảng Students
CREATE TABLE [dbo].[Students] (
    [StudentID]       INT IDENTITY(1,1) NOT NULL,
    [FullName]        NVARCHAR(100)     NOT NULL,
    [DOB]             DATE              NOT NULL,
    [Gender]          NVARCHAR(10)      NOT NULL,
    [AdmissionDate]   DATE              NOT NULL,
    [EnrollmentType]  NVARCHAR(50)      NULL,
    [Ethnicity]       NVARCHAR(50)      NULL,
    [PermanentAddress] NVARCHAR(255)    NULL,
    [BirthPlace]      NVARCHAR(255)     NULL,
    [Religion]        NVARCHAR(50)      NULL,
    [RepeatingYear]   BIT               NULL DEFAULT 0,
    [IDCardNumber]    NVARCHAR(20)      NULL,
    [Status]          NVARCHAR(50)      NULL DEFAULT N'Đang học',
    PRIMARY KEY CLUSTERED ([StudentID] ASC),
    UNIQUE NONCLUSTERED ([IDCardNumber] ASC)
)
GO

-- Bảng StudentClasses (Quản lý lớp của học sinh theo năm học)
CREATE TABLE [dbo].[StudentClasses] (
    [ID]             INT IDENTITY(1,1) NOT NULL,
    [StudentID]      INT               NOT NULL,
    [ClassID]        INT               NOT NULL,
    [AcademicYearID] INT               NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    UNIQUE NONCLUSTERED ([StudentID], [AcademicYearID])
)
GO

-- Bảng Subjects
CREATE TABLE [dbo].[Subjects] (
    [SubjectID]       INT IDENTITY(1,1) NOT NULL,
    [SubjectName]     NVARCHAR(100)     NOT NULL,
    [SubjectCategory] NVARCHAR(50)      NOT NULL CHECK ([SubjectCategory] IN ('KHTN', 'KHXH')),
    PRIMARY KEY CLUSTERED ([SubjectID] ASC),
    UNIQUE NONCLUSTERED ([SubjectName] ASC)
)
GO

-- Bảng TeacherClasses
CREATE TABLE [dbo].[TeacherClasses] (
    [ID]               INT IDENTITY(1,1) NOT NULL,
    [TeacherID]        INT               NOT NULL,
    [ClassID]          INT               NOT NULL,
    [IsHomeroomTeacher] BIT              NULL DEFAULT 0,
    [AcademicYearID]   INT               NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
)
GO

-- Bảng Teachers
CREATE TABLE [dbo].[Teachers] (
    [TeacherID]            INT IDENTITY(1,1) NOT NULL,
    [UserID]               INT               NULL,
    [FullName]             NVARCHAR(100)     NOT NULL,
    [DOB]                  DATE              NOT NULL,
    [Gender]               NVARCHAR(10)      NOT NULL,
    [Ethnicity]            NVARCHAR(50)      NULL,
    [Religion]             NVARCHAR(50)      NULL,
    [MaritalStatus]        NVARCHAR(50)      NULL,
    [IDCardNumber]         NVARCHAR(20)      NULL,
    [InsuranceNumber]      NVARCHAR(20)      NULL,
    [EmploymentType]       NVARCHAR(100)     NULL,
    [Position]             NVARCHAR(100)     NULL,
    [Department]           NVARCHAR(100)     NULL,
    [AdditionalDuties]     NVARCHAR(255)     NULL,
    [IsHeadOfDepartment]   BIT               NULL DEFAULT 0,
    [EmploymentStatus]     NVARCHAR(50)      NULL,
    [RecruitmentAgency]    NVARCHAR(255)     NULL,
    [HiringDate]           DATE              NULL,
    [PermanentEmploymentDate] DATE           NULL,
    [SchoolJoinDate]       DATE              NOT NULL,
    [PermanentAddress]     NVARCHAR(255)     NULL,
    [Hometown]             NVARCHAR(255)     NULL,
    PRIMARY KEY CLUSTERED ([TeacherID] ASC),
    UNIQUE NONCLUSTERED ([InsuranceNumber] ASC),
    UNIQUE NONCLUSTERED ([UserID] ASC),
    UNIQUE NONCLUSTERED ([IDCardNumber] ASC)
)
GO

-- Bảng TeacherSubjects
CREATE TABLE [dbo].[TeacherSubjects] (
    [ID]           INT IDENTITY(1,1) NOT NULL,
    [TeacherID]    INT               NOT NULL,
    [SubjectID]    INT               NOT NULL,
    [IsMainSubject] BIT              NULL DEFAULT 0,
    PRIMARY KEY CLUSTERED ([ID] ASC)
)
GO

-- Bảng TeachingAssignments
CREATE TABLE [dbo].[TeachingAssignments] (
    [AssignmentID] INT IDENTITY(1,1) NOT NULL,
    [TeacherID]    INT               NOT NULL,
    [SubjectID]    INT               NOT NULL,
    [ClassID]      INT               NOT NULL,
    [SemesterID]   INT               NOT NULL,
    PRIMARY KEY CLUSTERED ([AssignmentID] ASC)
)
GO

-- Bảng Timetables
CREATE TABLE [dbo].[Timetables] (
    [TimetableID]  INT IDENTITY(1,1) NOT NULL,
    [ClassID]      INT               NOT NULL,
    [SubjectID]    INT               NOT NULL,
    [TeacherID]    INT               NOT NULL,
    [DayOfWeek]    NVARCHAR(20)      NOT NULL,
    [Shift]        NVARCHAR(20)      NOT NULL CHECK ([Shift] IN (N'Sáng', N'Chiều')),
    [Period]       INT               NOT NULL,
    [EffectiveDate] DATE             NOT NULL,
    [SemesterID]   INT               NOT NULL,
    PRIMARY KEY CLUSTERED ([TimetableID] ASC)
)
GO

-- Bảng Users
CREATE TABLE [dbo].[Users] (
    [UserID]       INT IDENTITY(1,1) NOT NULL,
    [Username]     NVARCHAR(50)      NOT NULL,
    [PasswordHash] NVARCHAR(255)     NOT NULL,
    [Email]        NVARCHAR(100)     NULL,
    [PhoneNumber]  NVARCHAR(15)      NULL,
    [RoleID]       INT               NOT NULL,
    [Status]       NVARCHAR(20)      NULL DEFAULT N'Hoạt Động',
    PRIMARY KEY CLUSTERED ([UserID] ASC),
    UNIQUE NONCLUSTERED ([Username] ASC),
    UNIQUE NONCLUSTERED ([Email] ASC)
)
GO

-- Bảng Attendances
CREATE TABLE [dbo].[Attendances] (
    [AttendanceID] INT IDENTITY(1,1) NOT NULL,
    [StudentID]    INT               NOT NULL,
    [Date]         DATE              NOT NULL,
    [Status]       NVARCHAR(1)       NOT NULL CHECK ([Status] IN ('C', 'P', 'K', 'X')),
    [Note]         NVARCHAR(255)     NULL,
    [CreatedAt]    DATETIME          DEFAULT GETDATE(),
    [CreatedBy]    INT               NULL,
    [Shift]        NVARCHAR(20)      NOT NULL CHECK ([Shift] IN (N'Sáng', N'Chiều')),
    [SemesterID]   INT               NOT NULL,
    PRIMARY KEY CLUSTERED ([AttendanceID] ASC),
    CONSTRAINT [UQ_Attendance] UNIQUE ([StudentID], [Date], [Shift], [SemesterID])
)
GO

-- Thêm các ràng buộc khóa ngoại
ALTER TABLE [dbo].[Attendances] WITH CHECK ADD CONSTRAINT [FK_Attendance_Student] FOREIGN KEY ([StudentID]) REFERENCES [dbo].[Students] ([StudentID]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Attendances] WITH CHECK ADD CONSTRAINT [FK_Attendance_Teacher] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Teachers] ([TeacherID]) ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Attendances] WITH CHECK ADD CONSTRAINT [FK_Attendances_Semesters] FOREIGN KEY ([SemesterID]) REFERENCES [dbo].[Semesters] ([SemesterID])
GO
ALTER TABLE [dbo].[Exams] WITH CHECK ADD FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Teachers] ([TeacherID])
GO
ALTER TABLE [dbo].[Exams] WITH CHECK ADD FOREIGN KEY ([SubjectID]) REFERENCES [dbo].[Subjects] ([SubjectID])
GO
ALTER TABLE [dbo].[Exams] WITH CHECK ADD CONSTRAINT [FK_Exams_Semesters] FOREIGN KEY ([SemesterID]) REFERENCES [dbo].[Semesters] ([SemesterID])
GO
ALTER TABLE [dbo].[Grades] WITH CHECK ADD FOREIGN KEY ([ExamID]) REFERENCES [dbo].[Exams] ([ExamID])
GO
ALTER TABLE [dbo].[Grades] WITH CHECK ADD FOREIGN KEY ([StudentID]) REFERENCES [dbo].[Students] ([StudentID])
GO
ALTER TABLE [dbo].[Grades] WITH CHECK ADD FOREIGN KEY ([SubjectID]) REFERENCES [dbo].[Subjects] ([SubjectID])
GO
ALTER TABLE [dbo].[Grades] WITH CHECK ADD CONSTRAINT [FK_Grades_Semesters] FOREIGN KEY ([SemesterID]) REFERENCES [dbo].[Semesters] ([SemesterID])
GO
ALTER TABLE [dbo].[LeaveRequests] WITH CHECK ADD FOREIGN KEY ([ApprovedBy]) REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[LeaveRequests] WITH CHECK ADD FOREIGN KEY ([TeacherID]) REFERENCES [dbo].[Teachers] ([TeacherID])
GO
ALTER TABLE [dbo].[LessonPlans] WITH CHECK ADD FOREIGN KEY ([SubjectID]) REFERENCES [dbo].[Subjects] ([SubjectID])
GO
ALTER TABLE [dbo].[LessonPlans] WITH CHECK ADD FOREIGN KEY ([TeacherID]) REFERENCES [dbo].[Teachers] ([TeacherID])
GO
ALTER TABLE [dbo].[LessonPlans] WITH CHECK ADD CONSTRAINT [FK_LessonPlans_Semesters] FOREIGN KEY ([SemesterID]) REFERENCES [dbo].[Semesters] ([SemesterID])
GO
ALTER TABLE [dbo].[Notifications] WITH CHECK ADD FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[Parents] WITH CHECK ADD CONSTRAINT [FK_Parents_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([UserID]) ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Rewards] WITH CHECK ADD FOREIGN KEY ([TeacherID]) REFERENCES [dbo].[Teachers] ([TeacherID])
GO
ALTER TABLE [dbo].[Rewards] WITH CHECK ADD CONSTRAINT [FK_Rewards_Semesters] FOREIGN KEY ([SemesterID]) REFERENCES [dbo].[Semesters] ([SemesterID])
GO
ALTER TABLE [dbo].[StudentParents] WITH CHECK ADD FOREIGN KEY ([ParentID]) REFERENCES [dbo].[Parents] ([ParentID]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StudentParents] WITH CHECK ADD FOREIGN KEY ([StudentID]) REFERENCES [dbo].[Students] ([StudentID]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StudentClasses] WITH CHECK ADD CONSTRAINT [FK_StudentClasses_Students] FOREIGN KEY ([StudentID]) REFERENCES [dbo].[Students] ([StudentID]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StudentClasses] WITH CHECK ADD CONSTRAINT [FK_StudentClasses_Classes] FOREIGN KEY ([ClassID]) REFERENCES [dbo].[Classes] ([ClassID])
GO
ALTER TABLE [dbo].[StudentClasses] WITH CHECK ADD CONSTRAINT [FK_StudentClasses_AcademicYears] FOREIGN KEY ([AcademicYearID]) REFERENCES [dbo].[AcademicYears] ([AcademicYearID])
GO
ALTER TABLE [dbo].[TeacherClasses] WITH CHECK ADD FOREIGN KEY ([ClassID]) REFERENCES [dbo].[Classes] ([ClassID]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TeacherClasses] WITH CHECK ADD FOREIGN KEY ([TeacherID]) REFERENCES [dbo].[Teachers] ([TeacherID]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TeacherClasses] WITH CHECK ADD CONSTRAINT [FK_TeacherClasses_AcademicYears] FOREIGN KEY ([AcademicYearID]) REFERENCES [dbo].[AcademicYears] ([AcademicYearID])
GO
ALTER TABLE [dbo].[Teachers] WITH CHECK ADD CONSTRAINT [FK_Teachers_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([UserID]) ON DELETE SET NULL
GO
ALTER TABLE [dbo].[TeacherSubjects] WITH CHECK ADD FOREIGN KEY ([SubjectID]) REFERENCES [dbo].[Subjects] ([SubjectID])
GO
ALTER TABLE [dbo].[TeacherSubjects] WITH CHECK ADD FOREIGN KEY ([TeacherID]) REFERENCES [dbo].[Teachers] ([TeacherID])
GO
ALTER TABLE [dbo].[TeachingAssignments] WITH CHECK ADD FOREIGN KEY ([ClassID]) REFERENCES [dbo].[Classes] ([ClassID])
GO
ALTER TABLE [dbo].[TeachingAssignments] WITH CHECK ADD FOREIGN KEY ([SubjectID]) REFERENCES [dbo].[Subjects] ([SubjectID])
GO
ALTER TABLE [dbo].[TeachingAssignments] WITH CHECK ADD FOREIGN KEY ([TeacherID]) REFERENCES [dbo].[Teachers] ([TeacherID])
GO
ALTER TABLE [dbo].[TeachingAssignments] WITH CHECK ADD CONSTRAINT [FK_TeachingAssignments_Semesters] FOREIGN KEY ([SemesterID]) REFERENCES [dbo].[Semesters] ([SemesterID])
GO
ALTER TABLE [dbo].[Timetables] WITH CHECK ADD FOREIGN KEY ([ClassID]) REFERENCES [dbo].[Classes] ([ClassID])
GO
ALTER TABLE [dbo].[Timetables] WITH CHECK ADD FOREIGN KEY ([SubjectID]) REFERENCES [dbo].[Subjects] ([SubjectID])
GO
ALTER TABLE [dbo].[Timetables] WITH CHECK ADD FOREIGN KEY ([TeacherID]) REFERENCES [dbo].[Teachers] ([TeacherID])
GO
ALTER TABLE [dbo].[Timetables] WITH CHECK ADD CONSTRAINT [FK_Timetables_Semesters] FOREIGN KEY ([SemesterID]) REFERENCES [dbo].[Semesters] ([SemesterID])
GO
ALTER TABLE [dbo].[Users] WITH CHECK ADD FOREIGN KEY ([RoleID]) REFERENCES [dbo].[Roles] ([RoleID])
GO

-- Cài đặt chế độ đọc/ghi
USE [master]
GO
ALTER DATABASE [HGSDB] SET READ_WRITE
GO
USE [HGSDB]
GO

-- 1. Chèn dữ liệu vào AcademicYears
INSERT INTO [dbo].[AcademicYears] ([YearName], [StartDate], [EndDate])
VALUES 
    ('2024-2025', '2024-08-01', '2025-07-31'),
    ('2023-2024', '2023-08-01', '2024-07-31');
GO

-- 2. Chèn dữ liệu vào Semesters
INSERT INTO [dbo].[Semesters] ([AcademicYearID], [SemesterName], [StartDate], [EndDate])
VALUES 
    (1, N'Học kỳ 1', '2024-08-01', '2024-12-31'),
    (1, N'Học kỳ 2', '2025-01-01', '2025-05-31'),
    (2, N'Học kỳ 1', '2023-08-01', '2023-12-31'),
    (2, N'Học kỳ 2', '2024-01-01', '2024-05-31');
GO

-- 3. Chèn dữ liệu vào Classes (8 lớp)
INSERT INTO [dbo].[Classes] ([ClassName], [Grade])
VALUES 
    (N'6A', 6),
    (N'6B', 6),
    (N'7A', 7),
    (N'7B', 7),
    (N'8A', 8),
    (N'8B', 8),
    (N'9A', 9),
    (N'9B', 9);
GO

-- 4. Chèn dữ liệu vào Subjects (Các môn học cấp THCS Việt Nam)
INSERT INTO [dbo].[Subjects] ([SubjectName], [SubjectCategory])
VALUES 
    (N'Toán', 'KHTN'),
    (N'Ngữ văn', 'KHXH'),
    (N'Anh văn', 'KHXH'),
    (N'Vật lý', 'KHTN'),
    (N'Hóa học', 'KHTN'),
    (N'Sinh học', 'KHTN'),
    (N'Lịch sử', 'KHXH'),
    (N'Địa lý', 'KHXH'),
    (N'Giáo dục công dân', 'KHXH'),
    (N'Thể dục', 'KHTN'),
    (N'Công nghệ', 'KHTN'),
    (N'Tin học', 'KHTN'),
    (N'Âm nhạc', 'KHXH'),
    (N'Mỹ thuật', 'KHXH'),
    (N'Hoạt động trải nghiệm', 'KHXH');
GO

-- 5. Chèn dữ liệu vào Roles (Không có Student)
INSERT INTO [dbo].[Roles] ([RoleName])
VALUES 
    (N'Admin'),
    (N'Teacher'),
    (N'Parent');
GO

-- 6. Chèn dữ liệu vào Users (1 Admin + 30 Teachers + 1 Parent mẫu)
INSERT INTO [dbo].[Users] ([Username], [PasswordHash], [Email], [PhoneNumber], [RoleID], [Status])
VALUES 
    ('admin', 'hashedpassword123', 'admin@hgs.edu.vn', '0901234567', 1, N'Hoạt Động'),
    ('teacher1', 'hashedpassword123', 'teacher1@hgs.edu.vn', '0901234568', 2, N'Hoạt Động'),
    ('teacher2', 'hashedpassword123', 'teacher2@hgs.edu.vn', '0901234569', 2, N'Hoạt Động'),
    ('teacher3', 'hashedpassword123', 'teacher3@hgs.edu.vn', '0901234570', 2, N'Hoạt Động'),
    ('teacher4', 'hashedpassword123', 'teacher4@hgs.edu.vn', '0901234571', 2, N'Hoạt Động'),
    ('teacher5', 'hashedpassword123', 'teacher5@hgs.edu.vn', '0901234572', 2, N'Hoạt Động'),
    ('teacher6', 'hashedpassword123', 'teacher6@hgs.edu.vn', '0901234573', 2, N'Hoạt Động'),
    ('teacher7', 'hashedpassword123', 'teacher7@hgs.edu.vn', '0901234574', 2, N'Hoạt Động'),
    ('teacher8', 'hashedpassword123', 'teacher8@hgs.edu.vn', '0901234575', 2, N'Hoạt Động'),
    ('teacher9', 'hashedpassword123', 'teacher9@hgs.edu.vn', '0901234576', 2, N'Hoạt Động'),
    ('teacher10', 'hashedpassword123', 'teacher10@hgs.edu.vn', '0901234577', 2, N'Hoạt Động'),
    ('teacher11', 'hashedpassword123', 'teacher11@hgs.edu.vn', '0901234578', 2, N'Hoạt Động'),
    ('teacher12', 'hashedpassword123', 'teacher12@hgs.edu.vn', '0901234579', 2, N'Hoạt Động'),
    ('teacher13', 'hashedpassword123', 'teacher13@hgs.edu.vn', '0901234580', 2, N'Hoạt Động'),
    ('teacher14', 'hashedpassword123', 'teacher14@hgs.edu.vn', '0901234581', 2, N'Hoạt Động'),
    ('teacher15', 'hashedpassword123', 'teacher15@hgs.edu.vn', '0901234582', 2, N'Hoạt Động'),
    ('teacher16', 'hashedpassword123', 'teacher16@hgs.edu.vn', '0901234583', 2, N'Hoạt Động'),
    ('teacher17', 'hashedpassword123', 'teacher17@hgs.edu.vn', '0901234584', 2, N'Hoạt Động'),
    ('teacher18', 'hashedpassword123', 'teacher18@hgs.edu.vn', '0901234585', 2, N'Hoạt Động'),
    ('teacher19', 'hashedpassword123', 'teacher19@hgs.edu.vn', '0901234586', 2, N'Hoạt Động'),
    ('teacher20', 'hashedpassword123', 'teacher20@hgs.edu.vn', '0901234587', 2, N'Hoạt Động'),
    ('teacher21', 'hashedpassword123', 'teacher21@hgs.edu.vn', '0901234588', 2, N'Hoạt Động'),
    ('teacher22', 'hashedpassword123', 'teacher22@hgs.edu.vn', '0901234589', 2, N'Hoạt Động'),
    ('teacher23', 'hashedpassword123', 'teacher23@hgs.edu.vn', '0901234590', 2, N'Hoạt Động'),
    ('teacher24', 'hashedpassword123', 'teacher24@hgs.edu.vn', '0901234591', 2, N'Hoạt Động'),
    ('teacher25', 'hashedpassword123', 'teacher25@hgs.edu.vn', '0901234592', 2, N'Hoạt Động'),
    ('teacher26', 'hashedpassword123', 'teacher26@hgs.edu.vn', '0901234593', 2, N'Hoạt Động'),
    ('teacher27', 'hashedpassword123', 'teacher27@hgs.edu.vn', '0901234594', 2, N'Hoạt Động'),
    ('teacher28', 'hashedpassword123', 'teacher28@hgs.edu.vn', '0901234595', 2, N'Hoạt Động'),
    ('teacher29', 'hashedpassword123', 'teacher29@hgs.edu.vn', '0901234596', 2, N'Hoạt Động'),
    ('teacher30', 'hashedpassword123', 'teacher30@hgs.edu.vn', '0901234597', 2, N'Hoạt Động'),
    ('parent1', 'hashedpassword123', 'parent1@hgs.edu.vn', '0901234598', 3, N'Hoạt Động');
GO

-- 7. Chèn dữ liệu vào Teachers (30 giáo viên)
INSERT INTO [dbo].[Teachers] (
    [UserID], [FullName], [DOB], [Gender], [Ethnicity], [Religion], [MaritalStatus], 
    [IDCardNumber], [InsuranceNumber], [EmploymentType], [Position], [Department], 
    [AdditionalDuties], [IsHeadOfDepartment], [EmploymentStatus], [RecruitmentAgency], 
    [HiringDate], [PermanentEmploymentDate], [SchoolJoinDate], [PermanentAddress], [Hometown]
)
VALUES 
    (2, N'Nguyễn Văn A', '1980-05-15', N'Nam', N'Kinh', N'Không', N'Đã kết hôn', '123456789', 'BH123456', N'Cơ hữu', N'Giáo viên', N'Toán', N'Tổ trưởng', 1, N'Đang làm việc', N'Trường THCS HGS', '2010-08-01', '2012-08-01', '2010-08-01', N'123 Đường Láng, Hà Nội', N'Hà Nội'),
    (3, N'Trần Thị B', '1982-07-20', N'Nữ', N'Kinh', N'Phật giáo', N'Đã kết hôn', '123456790', 'BH123457', N'Cơ hữu', N'Giáo viên', N'Ngữ văn', NULL, 0, N'Đang làm việc', N'Trường THCS HGS', '2011-08-01', '2013-08-01', '2011-08-01', N'456 Đường Láng, Hà Nội', N'Hà Nội'),
    (4, N'Phạm Văn C', '1978-03-10', N'Nam', N'Kinh', N'Không', N'Đã kết hôn', '123456791', 'BH123458', N'Cơ hữu', N'Giáo viên', N'Anh văn', NULL, 0, N'Đang làm việc', N'Trường THCS HGS', '2009-08-01', '2011-08-01', '2009-08-01', N'789 Đường Láng, Hà Nội', N'Hà Nội'),
    (5, N'Lê Thị D', '1985-09-25', N'Nữ', N'Kinh', N'Không', N'Độc thân', '123456792', 'BH123459', N'Cơ hữu', N'Giáo viên', N'Vật lý', NULL, 0, N'Đang làm việc', N'Trường THCS HGS', '2012-08-01', '2014-08-01', '2012-08-01', N'101 Đường Láng, Hà Nội', N'Hà Nội'),
    (6, N'Hoàng Văn E', '1983-11-30', N'Nam', N'Kinh', N'Không', N'Đã kết hôn', '123456793', 'BH123460', N'Cơ hữu', N'Giáo viên', N'Hóa học', NULL, 0, N'Đang làm việc', N'Trường THCS HGS', '2013-08-01', '2015-08-01', '2013-08-01', N'202 Đường Láng, Hà Nội', N'Hà Nội'),
    (7, N'Nguyễn Thị F', '1981-02-14', N'Nữ', N'Kinh', N'Phật giáo', N'Đã kết hôn', '123456794', 'BH123461', N'Cơ hữu', N'Giáo viên', N'Sinh học', N'Tổ phó', 0, N'Đang làm việc', N'Trường THCS HGS', '2010-08-01', '2012-08-01', '2010-08-01', N'303 Đường Láng, Hà Nội', N'Hà Nội'),
    (8, N'Trần Văn G', '1979-06-18', N'Nam', N'Kinh', N'Không', N'Đã kết hôn', '123456795', 'BH123462', N'Cơ hữu', N'Giáo viên', N'Lịch sử', NULL, 0, N'Đang làm việc', N'Trường THCS HGS', '2011-08-01', '2013-08-01', '2011-08-01', N'404 Đường Láng, Hà Nội', N'Hà Nội'),
    (9, N'Phạm Thị H', '1984-08-22', N'Nữ', N'Kinh', N'Không', N'Độc thân', '123456796', 'BH123463', N'Cơ hữu', N'Giáo viên', N'Địa lý', NULL, 0, N'Đang làm việc', N'Trường THCS HGS', '2012-08-01', '2014-08-01', '2012-08-01', N'505 Đường Láng, Hà Nội', N'Hà Nội'),
    (10, N'Lê Văn I', '1980-12-05', N'Nam', N'Kinh', N'Không', N'Đã kết hôn', '123456797', 'BH123464', N'Cơ hữu', N'Giáo viên', N'Giáo dục công dân', NULL, 0, N'Đang làm việc', N'Trường THCS HGS', '2010-08-01', '2012-08-01', '2010-08-01', N'606 Đường Láng, Hà Nội', N'Hà Nội'),
    (11, N'Hoàng Thị K', '1983-04-17', N'Nữ', N'Kinh', N'Phật giáo', N'Đã kết hôn', '123456798', 'BH123465', N'Cơ hữu', N'Giáo viên', N'Thể dục', NULL, 0, N'Đang làm việc', N'Trường THCS HGS', '2011-08-01', '2013-08-01', '2011-08-01', N'707 Đường Láng, Hà Nội', N'Hà Nội'),
    (12, N'Nguyễn Văn L', '1982-10-10', N'Nam', N'Kinh', N'Không', N'Đã kết hôn', '123456799', 'BH123466', N'Cơ hữu', N'Giáo viên', N'Công nghệ', NULL, 0, N'Đang làm việc', N'Trường THCS HGS', '2012-08-01', '2014-08-01', '2012-08-01', N'808 Đường Láng, Hà Nội', N'Hà Nội'),
    (13, N'Trần Thị M', '1985-01-25', N'Nữ', N'Kinh', N'Không', N'Độc thân', '123456800', 'BH123467', N'Cơ hữu', N'Giáo viên', N'Tin học', NULL, 0, N'Đang làm việc', N'Trường THCS HGS', '2013-08-01', '2015-08-01', '2013-08-01', N'909 Đường Láng, Hà Nội', N'Hà Nội'),
    (14, N'Phạm Văn N', '1979-07-15', N'Nam', N'Kinh', N'Không', N'Đã kết hôn', '123456801', 'BH123468', N'Cơ hữu', N'Giáo viên', N'Âm nhạc', NULL, 0, N'Đang làm việc', N'Trường THCS HGS', '2010-08-01', '2012-08-01', '2010-08-01', N'1010 Đường Láng, Hà Nội', N'Hà Nội'),
    (15, N'Lê Thị O', '1981-03-20', N'Nữ', N'Kinh', N'Phật giáo', N'Đã kết hôn', '123456802', 'BH123469', N'Cơ hữu', N'Giáo viên', N'Mỹ thuật', NULL, 0, N'Đang làm việc', N'Trường THCS HGS', '2011-08-01', '2013-08-01', '2011-08-01', N'1111 Đường Láng, Hà Nội', N'Hà Nội'),
    (16, N'Hoàng Văn P', '1984-09-30', N'Nam', N'Kinh', N'Không', N'Đã kết hôn', '123456803', 'BH123470', N'Cơ hữu', N'Giáo viên', N'Hoạt động trải nghiệm', NULL, 0, N'Đang làm việc', N'Trường THCS HGS', '2012-08-01', '2014-08-01', '2012-08-01', N'1212 Đường Láng, Hà Nội', N'Hà Nội'),
    (17, N'Nguyễn Thị Q', '1980-05-05', N'Nữ', N'Kinh', N'Không', N'Độc thân', '123456804', 'BH123471', N'Cơ hữu', N'Giáo viên', N'Toán', NULL, 0, N'Đang làm việc', N'Trường THCS HGS', '2013-08-01', '2015-08-01', '2013-08-01', N'1313 Đường Láng, Hà Nội', N'Hà Nội'),
    (18, N'Trần Văn R', '1982-11-11', N'Nam', N'Kinh', N'Phật giáo', N'Đã kết hôn', '123456805', 'BH123472', N'Cơ hữu', N'Giáo viên', N'Ngữ văn', NULL, 0, N'Đang làm việc', N'Trường THCS HGS', '2010-08-01', '2012-08-01', '2010-08-01', N'1414 Đường Láng, Hà Nội', N'Hà Nội'),
    (19, N'Phạm Thị S', '1985-02-28', N'Nữ', N'Kinh', N'Không', N'Đã kết hôn', '123456806', 'BH123473', N'Cơ hữu', N'Giáo viên', N'Anh văn', NULL, 0, N'Đang làm việc', N'Trường THCS HGS', '2011-08-01', '2013-08-01', '2011-08-01', N'1515 Đường Láng, Hà Nội', N'Hà Nội'),
    (20, N'Lê Văn T', '1978-08-15', N'Nam', N'Kinh', N'Không', N'Đã kết hôn', '123456807', 'BH123474', N'Cơ hữu', N'Giáo viên', N'Vật lý', NULL, 0, N'Đang làm việc', N'Trường THCS HGS', '2012-08-01', '2014-08-01', '2012-08-01', N'1616 Đường Láng, Hà Nội', N'Hà Nội'),
    (21, N'Hoàng Thị U', '1983-04-10', N'Nữ', N'Kinh', N'Phật giáo', N'Độc thân', '123456808', 'BH123475', N'Cơ hữu', N'Giáo viên', N'Hóa học', NULL, 0, N'Đang làm việc', N'Trường THCS HGS', '2013-08-01', '2015-08-01', '2013-08-01', N'1717 Đường Láng, Hà Nội', N'Hà Nội'),
    (22, N'Nguyễn Văn V', '1980-12-20', N'Nam', N'Kinh', N'Không', N'Đã kết hôn', '123456809', 'BH123476', N'Cơ hữu', N'Giáo viên', N'Sinh học', NULL, 0, N'Đang làm việc', N'Trường THCS HGS', '2010-08-01', '2012-08-01', '2010-08-01', N'1818 Đường Láng, Hà Nội', N'Hà Nội'),
    (23, N'Trần Thị X', '1982-06-25', N'Nữ', N'Kinh', N'Không', N'Đã kết hôn', '123456810', 'BH123477', N'Cơ hữu', N'Giáo viên', N'Lịch sử', NULL, 0, N'Đang làm việc', N'Trường THCS HGS', '2011-08-01', '2013-08-01', '2011-08-01', N'1919 Đường Láng, Hà Nội', N'Hà Nội'),
    (24, N'Phạm Văn Y', '1985-10-15', N'Nam', N'Kinh', N'Phật giáo', N'Đã kết hôn', '123456811', 'BH123478', N'Cơ hữu', N'Giáo viên', N'Địa lý', NULL, 0, N'Đang làm việc', N'Trường THCS HGS', '2012-08-01', '2014-08-01', '2012-08-01', N'2020 Đường Láng, Hà Nội', N'Hà Nội'),
    (25, N'Lê Thị Z', '1979-03-30', N'Nữ', N'Kinh', N'Không', N'Độc thân', '123456812', 'BH123479', N'Cơ hữu', N'Giáo viên', N'Giáo dục công dân', NULL, 0, N'Đang làm việc', N'Trường THCS HGS', '2013-08-01', '2015-08-01', '2013-08-01', N'2121 Đường Láng, Hà Nội', N'Hà Nội'),
    (26, N'Hoàng Văn AA', '1981-09-10', N'Nam', N'Kinh', N'Không', N'Đã kết hôn', '123456813', 'BH123480', N'Cơ hữu', N'Giáo viên', N'Thể dục', NULL, 0, N'Đang làm việc', N'Trường THCS HGS', '2010-08-01', '2012-08-01', '2010-08-01', N'2222 Đường Láng, Hà Nội', N'Hà Nội'),
    (27, N'Nguyễn Thị BB', '1984-05-25', N'Nữ', N'Kinh', N'Phật giáo', N'Đã kết hôn', '123456814', 'BH123481', N'Cơ hữu', N'Giáo viên', N'Công nghệ', NULL, 0, N'Đang làm việc', N'Trường THCS HGS', '2011-08-01', '2013-08-01', '2011-08-01', N'2323 Đường Láng, Hà Nội', N'Hà Nội'),
    (28, N'Trần Văn CC', '1980-11-15', N'Nam', N'Kinh', N'Không', N'Đã kết hôn', '123456815', 'BH123482', N'Cơ hữu', N'Giáo viên', N'Tin học', NULL, 0, N'Đang làm việc', N'Trường THCS HGS', '2012-08-01', '2014-08-01', '2012-08-01', N'2424 Đường Láng, Hà Nội', N'Hà Nội'),
    (29, N'Phạm Thị DD', '1983-07-20', N'Nữ', N'Kinh', N'Không', N'Độc thân', '123456816', 'BH123483', N'Cơ hữu', N'Giáo viên', N'Âm nhạc', NULL, 0, N'Đang làm việc', N'Trường THCS HGS', '2013-08-01', '2015-08-01', '2013-08-01', N'2525 Đường Láng, Hà Nội', N'Hà Nội'),
    (30, N'Lê Văn EE', '1985-01-10', N'Nam', N'Kinh', N'Phật giáo', N'Đã kết hôn', '123456817', 'BH123484', N'Cơ hữu', N'Giáo viên', N'Mỹ thuật', NULL, 0, N'Đang làm việc', N'Trường THCS HGS', '2010-08-01', '2012-08-01', '2010-08-01', N'2626 Đường Láng, Hà Nội', N'Hà Nội'),
    (31, N'Hoàng Thị FF', '1982-04-05', N'Nữ', N'Kinh', N'Không', N'Đã kết hôn', '123456818', 'BH123485', N'Cơ hữu', N'Giáo viên', N'Hoạt động trải nghiệm', N'Tổ phó', 0, N'Đang làm việc', N'Trường THCS HGS', '2011-08-01', '2013-08-01', '2011-08-01', N'2727 Đường Láng, Hà Nội', N'Hà Nội');
GO

-- 8. Chèn dữ liệu vào TeacherSubjects (Phân bổ môn học cho 30 giáo viên)
INSERT INTO [dbo].[TeacherSubjects] ([TeacherID], [SubjectID], [IsMainSubject])
VALUES 
    (1, 1, 1),   -- Nguyễn Văn A dạy Toán (chính)
    (2, 2, 1),   -- Trần Thị B dạy Ngữ văn (chính)
    (3, 3, 1),   -- Phạm Văn C dạy Anh văn (chính)
    (4, 4, 1),   -- Lê Thị D dạy Vật lý (chính)
    (5, 5, 1),   -- Hoàng Văn E dạy Hóa học (chính)
    (6, 6, 1),   -- Nguyễn Thị F dạy Sinh học (chính)
    (7, 7, 1),   -- Trần Văn G dạy Lịch sử (chính)
    (8, 8, 1),   -- Phạm Thị H dạy Địa lý (chính)
    (9, 9, 1),   -- Lê Văn I dạy Giáo dục công dân (chính)
    (10, 10, 1), -- Hoàng Thị K dạy Thể dục (chính)
    (11, 11, 1), -- Nguyễn Văn L dạy Công nghệ (chính)
    (12, 12, 1), -- Trần Thị M dạy Tin học (chính)
    (13, 13, 1), -- Phạm Văn N dạy Âm nhạc (chính)
    (14, 14, 1), -- Lê Thị O dạy Mỹ thuật (chính)
    (15, 15, 1), -- Hoàng Văn P dạy Hoạt động trải nghiệm (chính)
    (16, 1, 1),  -- Nguyễn Thị Q dạy Toán (chính)
    (17, 2, 1),  -- Trần Văn R dạy Ngữ văn (chính)
    (18, 3, 1),  -- Phạm Thị S dạy Anh văn (chính)
    (19, 4, 1),  -- Lê Văn T dạy Vật lý (chính)
    (20, 5, 1),  -- Hoàng Thị U dạy Hóa học (chính)
    (21, 6, 1),  -- Nguyễn Văn V dạy Sinh học (chính)
    (22, 7, 1),  -- Trần Thị X dạy Lịch sử (chính)
    (23, 8, 1),  -- Phạm Văn Y dạy Địa lý (chính)
    (24, 9, 1),  -- Lê Thị Z dạy Giáo dục công dân (chính)
    (25, 10, 1), -- Hoàng Văn AA dạy Thể dục (chính)
    (26, 11, 1), -- Nguyễn Thị BB dạy Công nghệ (chính)
    (27, 12, 1), -- Trần Văn CC dạy Tin học (chính)
    (28, 13, 1), -- Phạm Thị DD dạy Âm nhạc (chính)
    (29, 14, 1), -- Lê Văn EE dạy Mỹ thuật (chính)
    (30, 15, 1); -- Hoàng Thị FF dạy Hoạt động trải nghiệm (chính)
GO

-- 9. Chèn dữ liệu vào Students (Mẫu, để có dữ liệu liên quan)
INSERT INTO [dbo].[Students] (
    [FullName], [DOB], [Gender], [AdmissionDate], [EnrollmentType], [Ethnicity], 
    [PermanentAddress], [BirthPlace], [Religion], [RepeatingYear], [IDCardNumber], [Status]
)
VALUES 
    (N'Trần Thị G', '2010-03-20', N'Nữ', '2024-08-01', N'Tuyển sinh', N'Kinh', 
     N'456 Đường Láng, Hà Nội', N'Hà Nội', N'Không', 0, 'STU123456', N'Đang học'),
    (N'Phạm Văn H', '2010-07-15', N'Nam', '2024-08-01', N'Tuyển sinh', N'Kinh', 
     N'789 Đường Láng, Hà Nội', N'Hà Nội', N'Không', 0, 'STU123457', N'Đang học');
GO

-- 10. Chèn dữ liệu vào StudentClasses
INSERT INTO [dbo].[StudentClasses] ([StudentID], [ClassID], [AcademicYearID])
VALUES 
    (1, 1, 1), -- Trần Thị G học lớp 6A năm 2024-2025
    (2, 1, 1); -- Phạm Văn H học lớp 6A năm 2024-2025
GO

-- 11. Chèn dữ liệu vào Parents (Mẫu)
INSERT INTO [dbo].[Parents] ([UserID], [FullName], [DOB], [Occupation], [Relationship])
VALUES 
    (32, N'Lê Thị I', '1985-10-10', N'Nội trợ', N'Mẹ');
GO

-- 12. Chèn dữ liệu vào StudentParents
INSERT INTO [dbo].[StudentParents] ([StudentID], [ParentID])
VALUES 
    (1, 1); -- Trần Thị G là con của Lê Thị I
GO

-- 13. Chèn dữ liệu vào TeachingAssignments (Mẫu cho 8 lớp)
INSERT INTO [dbo].[TeachingAssignments] ([TeacherID], [SubjectID], [ClassID], [SemesterID])
VALUES 
    (1, 1, 1, 1),  -- Nguyễn Văn A dạy Toán lớp 6A, HK1
    (2, 2, 1, 1),  -- Trần Thị B dạy Ngữ văn lớp 6A, HK1
    (3, 3, 2, 1),  -- Phạm Văn C dạy Anh văn lớp 6B, HK1
    (4, 4, 3, 1),  -- Lê Thị D dạy Vật lý lớp 7A, HK1
    (5, 5, 4, 1),  -- Hoàng Văn E dạy Hóa học lớp 7B, HK1
    (6, 6, 5, 1),  -- Nguyễn Thị F dạy Sinh học lớp 8A, HK1
    (7, 7, 6, 1),  -- Trần Văn G dạy Lịch sử lớp 8B, HK1
    (8, 8, 7, 1),  -- Phạm Thị H dạy Địa lý lớp 9A, HK1
    (9, 9, 8, 1);  -- Lê Văn I dạy Giáo dục công dân lớp 9B, HK1
GO

-- 14. Chèn dữ liệu vào Timetables (Mẫu cho lớp 6A)
INSERT INTO [dbo].[Timetables] (
    [ClassID], [SubjectID], [TeacherID], [DayOfWeek], [Shift], [Period], [EffectiveDate], [SemesterID]
)
VALUES 
    (1, 1, 1, N'Monday', N'Sáng', 1, '2024-09-02', 1), -- Lớp 6A học Toán tiết 1 sáng thứ 2
    (1, 2, 2, N'Tuesday', N'Sáng', 2, '2024-09-03', 1); -- Lớp 6A học Ngữ văn tiết 2 sáng thứ 3
GO

-- 15. Chèn dữ liệu vào Attendances (Mẫu)
INSERT INTO [dbo].[Attendances] (
    [StudentID], [Date], [Status], [Note], [CreatedAt], [CreatedBy], [Shift], [SemesterID]
)
VALUES 
    (1, '2024-09-02', 'C', N'Có mặt', GETDATE(), 1, N'Sáng', 1), -- Trần Thị G có mặt
    (2, '2024-09-02', 'P', N'Nghỉ phép', GETDATE(), 1, N'Sáng', 1); -- Phạm Văn H nghỉ phép
GO