-- Thêm một năm học vào bảng AcademicYears
INSERT INTO [dbo].[AcademicYears] ([YearName], [StartDate], [EndDate])
VALUES ('2024-2025', '2024-08-15', '2025-05-30');

-- Thêm hai học kỳ tương ứng vào bảng Semesters
INSERT INTO [dbo].[Semesters] ([AcademicYearID], [SemesterName], [StartDate], [EndDate])
VALUES 
(1, N'Học kỳ I', '2024-08-15', '2024-12-31'),
(1, N'Học kỳ II', '2025-01-05', '2025-05-30');
INSERT INTO [dbo].[GradeLevels] ([GradeName])
VALUES 
(N'Khối 6'),
(N'Khối 7'),
(N'Khối 8'),
(N'Khối 9');
INSERT INTO [dbo].[Classes] ([ClassName], [GradeLevelId])
VALUES 
(N'6A', 1),
(N'6B', 1),
(N'7A', 2),
(N'7B', 2),
(N'8A', 3),
(N'8B', 3),
(N'9A', 4),
(N'9B', 4);
INSERT INTO [dbo].[Subjects] ([SubjectName], [SubjectCategory], [TypeOfGrade])
VALUES 
(N'Ngữ văn', N'Văn hóa', N'Tính điểm'),
(N'Toán', N'Văn hóa', N'Tính điểm'),
(N'Tiếng Anh', N'Ngoại ngữ', N'Tính điểm'),
(N'Tin học', N'Công nghệ', N'Tính điểm'),
(N'Công nghệ', N'Công nghệ', N'Tính điểm'),
(N'Lịch sử', N'Xã hội', N'Tính điểm'),
(N'Địa lý', N'Xã hội', N'Tính điểm'),
(N'Giáo dục công dân', N'Xã hội', N'Tính điểm'),
(N'Khoa học tự nhiên', N'Tự nhiên', N'Tính điểm'),
(N'Sinh học', N'Tự nhiên', N'Tính điểm'),
(N'Vật lý', N'Tự nhiên', N'Tính điểm'),
(N'Hóa học', N'Tự nhiên', N'Tính điểm'),
(N'Âm nhạc', N'Năng khiếu', N'Nhận xét'),
(N'Mỹ thuật', N'Năng khiếu', N'Nhận xét'),
(N'Thể dục', N'Thể chất', N'Nhận xét'),
(N'Hoạt động trải nghiệm', N'Giáo dục kỹ năng sống', N'Nhận xét');
-- ================= KHỐI 6 =================
INSERT INTO [dbo].[GradeLevelSubjects] (GradeLevelID, SubjectID, PeriodsPerWeek_HKI, PeriodsPerWeek_HKII, ContinuousAssessments_HKI, ContinuousAssessments_HKII, MidtermAssessments, FinalAssessments)
VALUES
(1, 1, 4, 4, 2, 2, 1, 1),  -- Ngữ văn
(1, 2, 4, 4, 2, 2, 1, 1),  -- Toán
(1, 3, 3, 3, 2, 2, 1, 1),  -- Tiếng Anh
(1, 4, 1, 1, 1, 1, 0, 1),  -- Tin học
(1, 5, 1, 1, 1, 1, 0, 1),  -- Công nghệ
(1, 6, 1, 1, 1, 1, 0, 1),  -- Lịch sử
(1, 7, 1, 1, 1, 1, 0, 1),  -- Địa lý
(1, 8, 1, 1, 1, 1, 0, 1),  -- GDCD
(1, 9, 3, 3, 2, 2, 1, 1),  -- KHTN
(1,13, 1, 1, 0, 0, 0, 0),  -- Âm nhạc
(1,14, 1, 1, 0, 0, 0, 0),  -- Mỹ thuật
(1,15, 2, 2, 0, 0, 0, 0),  -- Thể dục
(1,16, 1, 1, 0, 0, 0, 0);  -- HĐ Trải nghiệm

-- ================= KHỐI 7 =================
INSERT INTO [dbo].[GradeLevelSubjects]
VALUES
(2, 1, 4, 4, 2, 2, 1, 1),  
(2, 2, 4, 4, 2, 2, 1, 1),  
(2, 3, 3, 3, 2, 2, 1, 1),  
(2, 4, 1, 1, 1, 1, 0, 1),  
(2, 5, 1, 1, 1, 1, 0, 1),  
(2, 6, 1, 1, 1, 1, 0, 1),  
(2, 7, 1, 1, 1, 1, 0, 1),  
(2, 8, 1, 1, 1, 1, 0, 1),  
(2, 9, 3, 3, 2, 2, 1, 1),  
(2,13, 1, 1, 0, 0, 0, 0),  
(2,14, 1, 1, 0, 0, 0, 0),  
(2,15, 2, 2, 0, 0, 0, 0),  
(2,16, 1, 1, 0, 0, 0, 0);  

-- ================= KHỐI 8 =================
-- KHTN được tách thành Lý, Hóa, Sinh
INSERT INTO [dbo].[GradeLevelSubjects]
VALUES
(3, 1, 4, 4, 2, 2, 1, 1),  
(3, 2, 4, 4, 2, 2, 1, 1),  
(3, 3, 3, 3, 2, 2, 1, 1),  
(3, 4, 1, 1, 1, 1, 0, 1),  
(3, 5, 1, 1, 1, 1, 0, 1),  
(3, 6, 1, 1, 1, 1, 0, 1),  
(3, 7, 1, 1, 1, 1, 0, 1),  
(3, 8, 1, 1, 1, 1, 0, 1),  
(3,10, 1, 1, 1, 1, 0, 1),  -- Sinh học
(3,11, 1, 1, 1, 1, 0, 1),  -- Vật lý
(3,12, 1, 1, 1, 1, 0, 1),  -- Hóa học
(3,13, 1, 1, 0, 0, 0, 0),  
(3,14, 1, 1, 0, 0, 0, 0),  
(3,15, 2, 2, 0, 0, 0, 0),  
(3,16, 1, 1, 0, 0, 0, 0);  

-- ================= KHỐI 9 =================
INSERT INTO [dbo].[GradeLevelSubjects]
VALUES
(4, 1, 4, 4, 2, 2, 1, 1),  
(4, 2, 4, 4, 2, 2, 1, 1),  
(4, 3, 3, 3, 2, 2, 1, 1),  
(4, 4, 1, 1, 1, 1, 0, 1),  
(4, 5, 1, 1, 1, 1, 0, 1),  
(4, 6, 1, 1, 1, 1, 0, 1),  
(4, 7, 1, 1, 1, 1, 0, 1),  
(4, 8, 1, 1, 1, 1, 0, 1),  
(4,10, 1, 1, 1, 1, 0, 1),  
(4,11, 1, 1, 1, 1, 0, 1),  
(4,12, 1, 1, 1, 1, 0, 1),  
(4,13, 1, 1, 0, 0, 0, 0),  
(4,14, 1, 1, 0, 0, 0, 0),  
(4,15, 2, 2, 0, 0, 0, 0),  
(4,16, 1, 1, 0, 0, 0, 0);  
INSERT INTO [dbo].[Roles] ([RoleName])
VALUES
(N'Hiệu Trưởng'),
(N'Hiệu Phó'),
(N'Cán Bộ Văn Thư'),
(N'Tổ Trưởng Bộ Môn'),
(N'Giáo Viên'),
(N'Phụ Huynh');
-- Insert data into Periods table
INSERT INTO [dbo].[Periods] ([PeriodName], [StartTime], [EndTime], [Shift])
VALUES
(N'Tiết 1', '07:00:00', '07:45:00', 1),  -- Period 1, Morning
(N'Tiết 2', '07:50:00', '08:35:00', 1),  -- Period 2, Morning
(N'Tiết 3', '08:40:00', '09:25:00', 1),  -- Period 3, Morning
(N'Tiết 4', '09:30:00', '10:15:00', 1),  -- Period 4, Morning
(N'Tiết 5', '10:20:00', '11:05:00', 1),  -- Period 5, Morning
(N'Tiết 6', '11:10:00', '11:55:00', 1),  -- Period 6, Morning
(N'Tiết 7', '13:00:00', '13:45:00', 2),  -- Period 7, Afternoon
(N'Tiết 8', '13:50:00', '14:35:00', 2),  -- Period 8, Afternoon
(N'Tiết 9', '14:40:00', '15:25:00', 2),  -- Period 9, Afternoon
(N'Tiết 10', '15:30:00', '16:15:00', 2); -- Period 10, Afternoon

