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
    Gender NVARCHAR(10) CHECK (Gender IN ('Nam', 'Nữ', 'Khác')) NOT NULL, -- Giới tính
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
    Relationship NVARCHAR(50) CHECK (Relationship IN ('Bố', 'Mẹ', 'Người bảo hộ')) NOT NULL,  -- Quan hệ với học sinh
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
    Status NVARCHAR(50) DEFAULT 'Đang học',  -- Trạng thái (Đang học, Bảo lưu, Nghỉ học, etc.)
    FOREIGN KEY (ClassID) REFERENCES Classes(ClassID),
);





-- Bảng 6: Subjects (Môn học)
CREATE TABLE Subjects (
    SubjectID INT IDENTITY(1,1) PRIMARY KEY,
    SubjectName NVARCHAR(100) UNIQUE NOT NULL
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

-- Bảng 14: Timetables (Thời khóa biểu)
CREATE TABLE Timetables (
    TimetableID INT IDENTITY(1,1) PRIMARY KEY,
    ClassID INT NOT NULL,
    SubjectID INT NOT NULL,
    TeacherID INT NOT NULL,
    DayOfWeek NVARCHAR(20) NOT NULL,
    Period INT NOT NULL,
    FOREIGN KEY (ClassID) REFERENCES Classes(ClassID),
    FOREIGN KEY (SubjectID) REFERENCES Subjects(SubjectID),
    FOREIGN KEY (TeacherID) REFERENCES Teachers(TeacherID)
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
