--create database HGSDB
-- Bảng 2: Roles (Vai trò người dùng)
CREATE TABLE Roles (
    RoleID INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(50) UNIQUE NOT NULL
);
-- Bảng 1: Users (Tài khoản người dùng)
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Email NVARCHAR(100) UNIQUE,
    PhoneNumber NVARCHAR(15),
    RoleID INT NOT NULL,
    Status NVARCHAR(20) DEFAULT 'Active',
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);	 

-- Bảng 4: Teachers (Giáo viên)
CREATE TABLE Teachers (
    TeacherID INT IDENTITY(1,1) PRIMARY KEY, -- Mã cán bộ
	UserID INT UNIQUE,
    FullName NVARCHAR(100) NOT NULL, -- Họ và tên
    DOB DATE NOT NULL, -- Ngày sinh
    Gender NVARCHAR(10) NOT NULL, -- Giới tính
    Ethnicity NVARCHAR(50), -- Dân tộc
    Religion NVARCHAR(50), -- Tôn giáo
    MaritalStatus NVARCHAR(50), -- Tình trạng hôn nhân
    IDCardNumber NVARCHAR(20) UNIQUE, -- Số CMND/CCCD
    InsuranceNumber NVARCHAR(20) UNIQUE, -- Số sổ bảo hiểm
    EmploymentType NVARCHAR(100), -- Tên hình thức hợp đồng
    Position NVARCHAR(100), -- Vị trí việc làm
    Department NVARCHAR(100), -- Tổ bộ môn
    AdditionalDuties NVARCHAR(255), -- Nhiệm vụ kiêm nhiệm (nếu có)
    IsHeadOfDepartment BIT DEFAULT 0, -- Là tổ trưởng (1: Có, 0: Không)
    EmploymentStatus NVARCHAR(50), -- Trạng thái cán bộ (Đang công tác, Nghỉ hưu, Nghỉ việc,...)
    RecruitmentAgency NVARCHAR(255), -- Cơ quan tuyển dụng
    HiringDate DATE, -- Ngày tuyển dụng
    PermanentEmploymentDate DATE, -- Ngày vào biên chế (nếu có)
    SchoolJoinDate DATE NOT NULL, -- Ngày vào trường
    PermanentAddress NVARCHAR(255), -- Địa chỉ thường trú
    Hometown NVARCHAR(255), -- Quê quán
	CONSTRAINT FK_Teachers_Users FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE SET NULL
);

-- Bảng 5: Classes (Lớp học)
CREATE TABLE Classes (
    ClassID INT IDENTITY(1,1) PRIMARY KEY,
    ClassName NVARCHAR(50) UNIQUE NOT NULL,
    Grade INT NOT NULL,
);
CREATE TABLE TeacherClasses (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    TeacherID INT NOT NULL,
    ClassID INT NOT NULL,
	IsHomeroomTeacher BIT DEFAULT 0,
    FOREIGN KEY (TeacherID) REFERENCES Teachers(TeacherID) ON DELETE CASCADE,
    FOREIGN KEY (ClassID) REFERENCES Classes(ClassID) ON DELETE CASCADE
);

CREATE TABLE Parents (
    ParentID INT IDENTITY(1,1) PRIMARY KEY,
	UserID INT UNIQUE,
    FullName NVARCHAR(100) NOT NULL,  -- Họ và tên
    DOB DATE,  -- Ngày sinh
    Occupation NVARCHAR(100),  -- Nghề nghiệp
    Relationship NVARCHAR(50) CHECK (Relationship IN (N'Bố', N'Mẹ', N'Người bảo hộ')) NOT NULL,  -- Quan hệ với học sinh
	CONSTRAINT FK_Parents_Users FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE SET NULL
);

CREATE TABLE Students (
    StudentID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    DOB DATE NOT NULL,
    Gender NVARCHAR(10) NOT NULL,
    ClassID INT NOT NULL, 
    AdmissionDate DATE NOT NULL,  -- Ngày vào trường
    EnrollmentType NVARCHAR(50),  -- Hình thức nhập học
    Ethnicity NVARCHAR(50),  -- Dân tộc
    PermanentAddress NVARCHAR(255),  -- Địa chỉ thường trú
    BirthPlace NVARCHAR(255),  -- Nơi sinh
    Religion NVARCHAR(50),  -- Tôn giáo
    RepeatingYear BIT DEFAULT 0,  -- Lưu ban năm trước (0: Không, 1: Có)
    IDCardNumber NVARCHAR(20) UNIQUE,  -- Số CMND/CCCD
    Status NVARCHAR(50) DEFAULT N'Đang học',  -- Trạng thái (Đang học, Bảo lưu, Nghỉ học, etc.)
    FOREIGN KEY (ClassID) REFERENCES Classes(ClassID),
);

-- Bảng 6: Subjects (Môn học)
CREATE TABLE Subjects (
    SubjectID INT IDENTITY(1,1) PRIMARY KEY,
    SubjectName NVARCHAR(100) UNIQUE NOT NULL,
	SubjectCategory NVARCHAR(50) CHECK (SubjectCategory IN ('KHTN', 'KHXH')) NOT NULL DEFAULT 'KHTN'
);

-- Bảng 7: TeacherSubjects (Quan hệ giáo viên - môn học)
CREATE TABLE TeacherSubjects (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    TeacherID INT NOT NULL,
    SubjectID INT NOT NULL,
	IsMainSubject BIT DEFAULT 0,
    FOREIGN KEY (TeacherID) REFERENCES Teachers(TeacherID),
    FOREIGN KEY (SubjectID) REFERENCES Subjects(SubjectID)
);
CREATE TABLE TeachingAssignments (
    AssignmentID INT IDENTITY(1,1) PRIMARY KEY,
    TeacherID INT NOT NULL,
    SubjectID INT NOT NULL,
    ClassID INT NOT NULL,
    Semester TINYINT NOT NULL CHECK (Semester IN (1, 2)), -- 1: Kỳ 1, 2: Kỳ 2
    AcademicYear VARCHAR(9) NOT NULL, -- Ví dụ: "2024-2025"
    
    FOREIGN KEY (TeacherID) REFERENCES Teachers(TeacherID),
    FOREIGN KEY (SubjectID) REFERENCES Subjects(SubjectID),
    FOREIGN KEY (ClassID) REFERENCES Classes(ClassID)
);

-- Bảng 8: Exams (Đề thi)
CREATE TABLE Exams (
    ExamID INT IDENTITY(1,1) PRIMARY KEY,
    SubjectID INT NOT NULL,
    CreatedBy INT NOT NULL, -- ID của giáo viên tạo đề
    ExamContent NVARCHAR(MAX) NOT NULL,
    CreatedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (SubjectID) REFERENCES Subjects(SubjectID),
    FOREIGN KEY (CreatedBy) REFERENCES Teachers(TeacherID)
);

-- Bảng 9: Grades (Bảng điểm)
CREATE TABLE Grades (
    GradeID INT IDENTITY(1,1) PRIMARY KEY,
    StudentID INT NOT NULL,
    SubjectID INT NOT NULL,
    Score DECIMAL(5,2) NOT NULL,
    ExamID INT NOT NULL,
    FOREIGN KEY (StudentID) REFERENCES Students(StudentID),
    FOREIGN KEY (SubjectID) REFERENCES Subjects(SubjectID),
    FOREIGN KEY (ExamID) REFERENCES Exams(ExamID)
);



CREATE TABLE StudentParents (
    StudentID INT NOT NULL,
    ParentID INT NOT NULL,
    PRIMARY KEY (StudentID, ParentID),
    FOREIGN KEY (StudentID) REFERENCES Students(StudentID) ON DELETE CASCADE,
    FOREIGN KEY (ParentID) REFERENCES Parents(ParentID) ON DELETE CASCADE
);

-- Bảng 12: Rewards (Khen thưởng giáo viên)
CREATE TABLE Rewards (
    RewardID INT IDENTITY(1,1) PRIMARY KEY,
    TeacherID INT NOT NULL,
    RewardType NVARCHAR(50),
    DateReceived DATE,
    FOREIGN KEY (TeacherID) REFERENCES Teachers(TeacherID)
);

-- Bảng 13: LessonPlans (Kế hoạch bài giảng)
CREATE TABLE LessonPlans (
    PlanID INT IDENTITY(1,1) PRIMARY KEY,
    TeacherID INT NOT NULL,
    SubjectID INT NOT NULL,
    PlanContent NVARCHAR(MAX) NOT NULL,
    Status NVARCHAR(20) DEFAULT 'Pending', -- Pending, Approved, Rejected
    FOREIGN KEY (TeacherID) REFERENCES Teachers(TeacherID),
    FOREIGN KEY (SubjectID) REFERENCES Subjects(SubjectID)
);

CREATE TABLE Timetables (
    TimetableID INT IDENTITY(1,1) PRIMARY KEY,
    ClassID INT NOT NULL,
    SubjectID INT NOT NULL,
    TeacherID INT NOT NULL,
    [DayOfWeek] NVARCHAR(20) NOT NULL, -- Thứ trong tuần
    [Shift] NVARCHAR(20) NOT NULL, -- Sáng / Chiều
    [Period] INT NOT NULL, -- Tiết học
    SchoolYear NVARCHAR(10) NOT NULL, -- Năm học (VD: 2024-2025)
    Semester INT NOT NULL, -- Học kỳ (1 hoặc 2),
	EffectiveDate DATE NOT NULL, -- Ngày bắt đầu áp dụng
    FOREIGN KEY (ClassID) REFERENCES Classes(ClassID),
    FOREIGN KEY (SubjectID) REFERENCES Subjects(SubjectID),
    FOREIGN KEY (TeacherID) REFERENCES Teachers(TeacherID),
);


-- Bảng 15: Attendances (Điểm danh học sinh)
CREATE TABLE Attendances (
    AttendanceID INT IDENTITY(1,1) PRIMARY KEY,
    StudentID INT NOT NULL,
    Date DATE NOT NULL,
    Status NVARCHAR(20) NOT NULL, -- Present, Absent, Late
    FOREIGN KEY (StudentID) REFERENCES Students(StudentID)
);

-- Bảng 16: Notifications (Thông báo)
CREATE TABLE Notifications (
    NotificationID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    SentDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- Bảng 17: LeaveRequests (Đơn xin nghỉ của giáo viên)
CREATE TABLE LeaveRequests (
    RequestID INT IDENTITY(1,1) PRIMARY KEY,
    TeacherID INT NOT NULL,
    RequestDate DATE NOT NULL,
    LeaveFromDate DATE NOT NULL,
    LeaveToDate DATE NOT NULL,
    Reason NVARCHAR(MAX) NOT NULL,
    Status NVARCHAR(20) DEFAULT 'Pending', -- Pending, Approved, Rejected
    ApprovedBy INT, -- ID của người duyệt (Hiệu trưởng hoặc quản lý)
    FOREIGN KEY (TeacherID) REFERENCES Teachers(TeacherID),
    FOREIGN KEY (ApprovedBy) REFERENCES Users(UserID)
);
-- Thêm dữ liệu vào bảng Roles
INSERT INTO Roles (RoleName) VALUES ('Admin'), (N'Giáo viên'), (N'Tổ Trưởng'), (N'Phụ huynh');

-- Thêm dữ liệu vào bảng Users (đảm bảo UserID tồn tại trước khi thêm giáo viên)
INSERT INTO Users (Username, PasswordHash, Email, PhoneNumber, RoleID)
VALUES 
('teacher2', 'hashedpassword5', 'teacher2@example.com', '0356789012', 1),
('teacher3', 'hashedpassword6', 'teacher3@example.com', '0367890123', 2),
('teacher4', 'hashedpassword7', 'teacher4@example.com', '0378901234', 2),
('teacher5', 'hashedpassword8', 'teacher5@example.com', '0389012345', 3),
('parent', 'hashedpassword9', 'parent@example.com', '032424244', 4);
-- Thêm giáo viên vào bảng Teachers
INSERT INTO Teachers (
    UserID, FullName, DOB, Gender, Ethnicity, Religion, MaritalStatus, 
    IDCardNumber, InsuranceNumber, EmploymentType, Position, Department, 
    AdditionalDuties, IsHeadOfDepartment, EmploymentStatus, RecruitmentAgency, 
    HiringDate, PermanentEmploymentDate, SchoolJoinDate, PermanentAddress, Hometown
) 
VALUES 
-- Giáo viên Toán
(1, N'Lê Văn B', '1985-07-15', N'Nam', N'Kinh', N'Không', N'Đã kết hôn', '111222333', '444555666', 
N'Biên chế', N'Giáo viên Toán', N'Tổ Toán', NULL, 0, N'Đang công tác', N'Sở GD&ĐT Hà Nội', 
'2010-09-01', '2015-09-01', '2010-09-01', N'123 Đường ABC, Hà Nội', N'Hải Phòng'),

-- Giáo viên Văn
(2, N'Nguyễn Thị C', '1990-08-20', N'Nữ', N'Kinh', N'Không', N'Độc thân', '222333444', '555666777', 
N'Biên chế', N'Giáo viên Văn', N'Tổ Ngữ Văn', N'Chủ nhiệm lớp 7A', 0, N'Đang công tác', N'Sở GD&ĐT Hà Nội', 
'2012-09-01', '2014-08-20', '2012-09-01', N'456 Đường XYZ, Hà Nội', N'Ninh Bình'),

-- Giáo viên Vật Lý
(3, N'Trần Văn D', '1988-06-10', N'Nam', N'Kinh', N'Không', N'Đã kết hôn', '333444555', '666777888', 
N'Biên chế', N'Giáo viên Vật Lý', N'Tổ Khoa học Tự nhiên', NULL, 0, N'Đang công tác', N'Sở GD&ĐT Hà Nội', 
'2015-09-01', '2017-08-20', '2015-09-01', N'789 Đường LMN, Hà Nội', N'Bắc Giang'),

-- Giáo viên Tiếng Anh
(4, N'Hoàng Thị E', '1992-03-05', N'Nữ', N'Kinh', N'Không', N'Độc thân', '444555666', '777888999', 
N'Biên chế', N'Giáo viên Tiếng Anh', N'Tổ Ngoại ngữ', NULL, 0, N'Đang công tác', N'Sở GD&ĐT Hà Nội', 
'2018-09-01', '2020-08-20', '2018-09-01', N'101 Đường DEF, Hà Nội', N'Nam Định');

-- Thêm dữ liệu vào bảng Classes
INSERT INTO Classes (ClassName, Grade) VALUES ('6A', 6), ('7B', 7);

-- Thêm dữ liệu vào bảng TeacherClasses
INSERT INTO TeacherClasses (TeacherID, ClassID, IsHomeroomTeacher)
VALUES (1, 1, 1);
INSERT INTO Students (FullName, DOB, Gender, ClassID, AdmissionDate, EnrollmentType, Ethnicity, PermanentAddress, BirthPlace, Religion, RepeatingYear, IDCardNumber, Status)
VALUES 
(N'Nguyễn Văn A', '2010-05-15', 'Nam', 1, '2022-09-01', N'Trúng Tuyển', N'Kinh', N'123 Đường ABC, Hà Nội', N'Hà Nội', 'Không', NULL, '123456789', 'Active'),
(N'Trần Thị B', '2011-08-20', N'Nữ', 2, '2022-09-01', N'Trúng Tuyển', N'Kinh', N'456 Đường XYZ, TP.HCM', N'TP.HCM', 'Không', NULL, '987654321', 'Active'),
(N'Lê Văn C', '2010-12-10', 'Nam', 2, '2023-09-01', N'Tuyển Thẳng', N'Tày', N'789 Đường DEF, Đà Nẵng', N'Đà Nẵng', N'Phật giáo', 1, '112233445', 'Active');

-- Thêm dữ liệu vào bảng Parents
INSERT INTO Parents (UserID, FullName, DOB, Occupation, Relationship)
VALUES (4, N'Trần Văn C', '1980-02-20', N'Nhân viên văn phòng', N'Bố');

-- Liên kết học sinh với phụ huynh
INSERT INTO StudentParents (StudentID, ParentID) VALUES (1, 1);

-- Thêm dữ liệu vào bảng Subjects
INSERT INTO Subjects (SubjectName, SubjectCategory) VALUES (N'Toán', 'KHTN'), (N'Văn', 'KHXH');

-- Thêm dữ liệu vào bảng TeacherSubjects
INSERT INTO TeacherSubjects (TeacherID, SubjectID, IsMainSubject) VALUES (1, 1, 1);

-- Thêm dữ liệu vào bảng TeachingAssignments
INSERT INTO TeachingAssignments (TeacherID, SubjectID, ClassID, Semester, AcademicYear)
VALUES (1, 1, 1, 1, '2024-2025');

-- Thêm dữ liệu vào bảng Exams
INSERT INTO Exams (SubjectID, CreatedBy, ExamContent, CreatedDate)
VALUES (1, 1, N'Đề thi toán lớp 6 học kỳ 1', GETDATE());

-- Thêm dữ liệu vào bảng Grades
INSERT INTO Grades (StudentID, SubjectID, Score, ExamID)
VALUES (1, 1, 8.5, 1);

-- Thêm dữ liệu vào bảng Timetables
INSERT INTO Timetables (ClassID, SubjectID, TeacherID, DayOfWeek, Period)
VALUES (1, 1, 1, N'Thứ Hai', 1);

-- Thêm dữ liệu vào bảng Attendances
INSERT INTO Attendances (StudentID, Date, Status)
VALUES (1, '2024-03-06', 'Present');

-- Thêm dữ liệu vào bảng Notifications
INSERT INTO Notifications (UserID, Message, SentDate)
VALUES (2, N'Họp giáo viên vào thứ 6 tuần này', GETDATE());

-- Thêm dữ liệu vào bảng LeaveRequests
INSERT INTO LeaveRequests (TeacherID, RequestDate, LeaveFromDate, LeaveToDate, Reason, Status, ApprovedBy)
VALUES (1, GETDATE(), '2024-03-10', '2024-03-12', N'Nghỉ phép cá nhân', 'Pending', NULL);
