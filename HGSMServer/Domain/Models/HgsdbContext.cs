using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Domain.Models;

public partial class HgsdbContext : DbContext
{
    public HgsdbContext()
    {
    }

    public HgsdbContext(DbContextOptions<HgsdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AcademicYear> AcademicYears { get; set; }

    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<ExamProposal> ExamProposals { get; set; }

    public virtual DbSet<ExamProposalQuestion> ExamProposalQuestions { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<GradeBatch> GradeBatches { get; set; }

    public virtual DbSet<LeaveRequest> LeaveRequests { get; set; }

    public virtual DbSet<LessonPlan> LessonPlans { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Parent> Parents { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Semester> Semesters { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentClass> StudentClasses { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<TeacherClass> TeacherClasses { get; set; }

    public virtual DbSet<TeacherSubject> TeacherSubjects { get; set; }

    public virtual DbSet<TeachingAssignment> TeachingAssignments { get; set; }

    public virtual DbSet<Timetable> Timetables { get; set; }

    public virtual DbSet<TimetableDetail> TimetableDetails { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server =DUYKHANH; database = HGSDB;Trusted_Connection=True;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AcademicYear>(entity =>
        {
<<<<<<< HEAD
            entity.HasKey(e => e.AcademicYearId).HasName("PK__Academic__C54C7A2196D7E364");

            entity.HasIndex(e => e.YearName, "UQ__Academic__294C4DA98F1B64EB").IsUnique();
=======
            entity.HasKey(e => e.AcademicYearId).HasName("PK__Academic__C54C7A2173108EDA");

            entity.HasIndex(e => e.YearName, "UQ__Academic__294C4DA9DEEAE017").IsUnique();
>>>>>>> 6168a45d57591b82132715f37f255d69d80459f9

            entity.Property(e => e.AcademicYearId).HasColumnName("AcademicYearID");
            entity.Property(e => e.YearName)
                .HasMaxLength(9)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Attendance>(entity =>
        {
<<<<<<< HEAD
            entity.HasKey(e => e.AttendanceId).HasName("PK__Attendan__8B69263CD9375B10");
=======
            entity.HasKey(e => e.AttendanceId).HasName("PK__Attendan__8B69263C3FFBC282");
>>>>>>> 6168a45d57591b82132715f37f255d69d80459f9

            entity.HasIndex(e => new { e.StudentId, e.Date, e.Shift, e.SemesterId }, "UQ_Attendance").IsUnique();

            entity.Property(e => e.AttendanceId).HasColumnName("AttendanceID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.SemesterId).HasColumnName("SemesterID");
            entity.Property(e => e.Shift).HasMaxLength(20);
            entity.Property(e => e.Status).HasMaxLength(1);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Attendances_Teachers");

            entity.HasOne(d => d.Semester).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.SemesterId)
                .HasConstraintName("FK_Attendances_Semesters");

            entity.HasOne(d => d.Student).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_Attendances_Students");
        });

        modelBuilder.Entity<Class>(entity =>
        {
<<<<<<< HEAD
            entity.HasKey(e => e.ClassId).HasName("PK__Classes__CB1927A0856A48DD");

            entity.HasIndex(e => e.ClassName, "UQ__Classes__F8BF561BFFFBD54A").IsUnique();
=======
            entity.HasKey(e => e.ClassId).HasName("PK__Classes__CB1927A03348C3A5");

            entity.HasIndex(e => e.ClassName, "UQ__Classes__F8BF561B8F263811").IsUnique();
>>>>>>> 6168a45d57591b82132715f37f255d69d80459f9

            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.ClassName).HasMaxLength(50);
        });

        modelBuilder.Entity<ExamProposal>(entity =>
        {
<<<<<<< HEAD
            entity.HasKey(e => e.ProposalId).HasName("PK__ExamProp__6F39E10048AF24C2");
=======
            entity.HasKey(e => e.ProposalId).HasName("PK__ExamProp__6F39E1009F126E21");
>>>>>>> 6168a45d57591b82132715f37f255d69d80459f9

            entity.Property(e => e.ProposalId).HasColumnName("ProposalID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SemesterId).HasColumnName("SemesterID");
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ExamProposals)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ExamProposals_Teachers");

            entity.HasOne(d => d.Semester).WithMany(p => p.ExamProposals)
                .HasForeignKey(d => d.SemesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ExamProposals_Semesters");

            entity.HasOne(d => d.Subject).WithMany(p => p.ExamProposals)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ExamProposals_Subjects");
        });

        modelBuilder.Entity<ExamProposalQuestion>(entity =>
        {
<<<<<<< HEAD
            entity.HasKey(e => e.Id).HasName("PK__ExamProp__3214EC270C084E42");
=======
            entity.HasKey(e => e.Id).HasName("PK__ExamProp__3214EC2725744010");
>>>>>>> 6168a45d57591b82132715f37f255d69d80459f9

            entity.HasIndex(e => new { e.ProposalId, e.QuestionId }, "UQ_Proposal_Question").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ProposalId).HasColumnName("ProposalID");
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

            entity.HasOne(d => d.Proposal).WithMany(p => p.ExamProposalQuestions)
                .HasForeignKey(d => d.ProposalId)
                .HasConstraintName("FK_ExamProposalQuestions_ExamProposals");

            entity.HasOne(d => d.Question).WithMany(p => p.ExamProposalQuestions)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ExamProposalQuestions_Questions");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
<<<<<<< HEAD
            entity.HasKey(e => e.GradeId).HasName("PK__Grades__54F87A379AA84E07");
=======
            entity.HasKey(e => e.GradeId).HasName("PK__Grades__54F87A377831F58E");
>>>>>>> 6168a45d57591b82132715f37f255d69d80459f9

            entity.Property(e => e.GradeId).HasColumnName("GradeID");
            entity.Property(e => e.AssessmentsTypeName).HasMaxLength(100);
            entity.Property(e => e.AssignmentId).HasColumnName("AssignmentID");
            entity.Property(e => e.BatchId).HasColumnName("BatchID");
            entity.Property(e => e.Score).HasMaxLength(10);
            entity.Property(e => e.StudentClassId).HasColumnName("StudentClassID");

            entity.HasOne(d => d.Assignment).WithMany(p => p.Grades)
                .HasForeignKey(d => d.AssignmentId)
                .HasConstraintName("FK__Grades__Assignme__787EE5A0");

            entity.HasOne(d => d.Batch).WithMany(p => p.Grades)
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK__Grades__BatchID__76969D2E");

            entity.HasOne(d => d.StudentClass).WithMany(p => p.Grades)
                .HasForeignKey(d => d.StudentClassId)
                .HasConstraintName("FK__Grades__StudentC__778AC167");
        });

        modelBuilder.Entity<GradeBatch>(entity =>
        {
<<<<<<< HEAD
            entity.HasKey(e => e.BatchId).HasName("PK__GradeBat__5D55CE38D9B4B281");
=======
            entity.HasKey(e => e.BatchId).HasName("PK__GradeBat__5D55CE38F4383A76");
>>>>>>> 6168a45d57591b82132715f37f255d69d80459f9

            entity.Property(e => e.BatchId).HasColumnName("BatchID");
            entity.Property(e => e.BatchName).HasMaxLength(255);
            entity.Property(e => e.SemesterId).HasColumnName("SemesterID");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.Semester).WithMany(p => p.GradeBatches)
                .HasForeignKey(d => d.SemesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GradeBatches_Semesters");
        });

        modelBuilder.Entity<LeaveRequest>(entity =>
        {
<<<<<<< HEAD
            entity.HasKey(e => e.RequestId).HasName("PK__LeaveReq__33A8519AAB3164A4");
=======
            entity.HasKey(e => e.RequestId).HasName("PK__LeaveReq__33A8519A834D6C9B");
>>>>>>> 6168a45d57591b82132715f37f255d69d80459f9

            entity.Property(e => e.RequestId).HasColumnName("RequestID");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.Teacher).WithMany(p => p.LeaveRequests)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK_LeaveRequests_Teachers");
        });

        modelBuilder.Entity<LessonPlan>(entity =>
        {
<<<<<<< HEAD
            entity.HasKey(e => e.PlanId).HasName("PK__LessonPl__755C22D760A8AA77");
=======
            entity.HasKey(e => e.PlanId).HasName("PK__LessonPl__755C22D767A4C347");
>>>>>>> 6168a45d57591b82132715f37f255d69d80459f9

            entity.Property(e => e.PlanId).HasColumnName("PlanID");
            entity.Property(e => e.AttachmentUrl).HasMaxLength(500);
            entity.Property(e => e.ReviewedDate).HasColumnType("datetime");
            entity.Property(e => e.SemesterId).HasColumnName("SemesterID");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.SubmittedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Reviewer).WithMany(p => p.LessonPlanReviewers)
                .HasForeignKey(d => d.ReviewerId)
                .HasConstraintName("FK_LessonPlans_Reviewer_Teachers");

            entity.HasOne(d => d.Semester).WithMany(p => p.LessonPlans)
                .HasForeignKey(d => d.SemesterId)
                .HasConstraintName("FK_LessonPlans_Semesters");

            entity.HasOne(d => d.Subject).WithMany(p => p.LessonPlans)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("FK_LessonPlans_Subjects");

            entity.HasOne(d => d.Teacher).WithMany(p => p.LessonPlanTeachers)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK_LessonPlans_Teachers");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
<<<<<<< HEAD
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E32FF2FD8CE");
=======
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E32756085F9");
>>>>>>> 6168a45d57591b82132715f37f255d69d80459f9

            entity.Property(e => e.NotificationId).HasColumnName("NotificationID");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Title).HasMaxLength(1000);
        });

        modelBuilder.Entity<Parent>(entity =>
        {
<<<<<<< HEAD
            entity.HasKey(e => e.ParentId).HasName("PK__Parents__D339510F08E5A0DD");

            entity.HasIndex(e => e.UserId, "UQ__Parents__1788CCAD9826AE33").IsUnique();
=======
            entity.HasKey(e => e.ParentId).HasName("PK__Parents__D339510FE195833F");

            entity.HasIndex(e => e.UserId, "UQ__Parents__1788CCAD5A4069FE").IsUnique();
>>>>>>> 6168a45d57591b82132715f37f255d69d80459f9

            entity.Property(e => e.ParentId).HasColumnName("ParentID");
            entity.Property(e => e.EmailFather).HasMaxLength(100);
            entity.Property(e => e.EmailGuardian).HasMaxLength(100);
            entity.Property(e => e.EmailMother).HasMaxLength(100);
            entity.Property(e => e.FullNameFather).HasMaxLength(100);
            entity.Property(e => e.FullNameGuardian).HasMaxLength(100);
            entity.Property(e => e.FullNameMother).HasMaxLength(100);
            entity.Property(e => e.IdcardNumberFather).HasMaxLength(50);
            entity.Property(e => e.IdcardNumberGuardian).HasMaxLength(50);
            entity.Property(e => e.IdcardNumberMother).HasMaxLength(50);
            entity.Property(e => e.OccupationFather).HasMaxLength(100);
            entity.Property(e => e.OccupationGuardian).HasMaxLength(100);
            entity.Property(e => e.OccupationMother).HasMaxLength(100);
            entity.Property(e => e.PhoneNumberFather).HasMaxLength(15);
            entity.Property(e => e.PhoneNumberGuardian).HasMaxLength(15);
            entity.Property(e => e.PhoneNumberMother).HasMaxLength(15);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithOne(p => p.Parent)
                .HasForeignKey<Parent>(d => d.UserId)
                .HasConstraintName("FK_Parents_Users");
        });

        modelBuilder.Entity<Question>(entity =>
        {
<<<<<<< HEAD
            entity.HasKey(e => e.QuestionId).HasName("PK__Question__0DC06F8CCA06A81E");
=======
            entity.HasKey(e => e.QuestionId).HasName("PK__Question__0DC06F8C0E1E7926");
>>>>>>> 6168a45d57591b82132715f37f255d69d80459f9

            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
            entity.Property(e => e.CorrectAnswer).HasMaxLength(255);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Difficulty).HasMaxLength(50);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.QuestionType).HasMaxLength(50);
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Questions)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Questions_Teachers");

            entity.HasOne(d => d.Subject).WithMany(p => p.Questions)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Questions_Subjects");
        });

        modelBuilder.Entity<Role>(entity =>
        {
<<<<<<< HEAD
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A00B3A77B");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160871AEE8A").IsUnique();
=======
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A18C6FA50");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B616079BA58B1").IsUnique();
>>>>>>> 6168a45d57591b82132715f37f255d69d80459f9

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Semester>(entity =>
        {
<<<<<<< HEAD
            entity.HasKey(e => e.SemesterId).HasName("PK__Semester__043301BD04715037");
=======
            entity.HasKey(e => e.SemesterId).HasName("PK__Semester__043301BD97F90FC3");
>>>>>>> 6168a45d57591b82132715f37f255d69d80459f9

            entity.HasIndex(e => new { e.AcademicYearId, e.SemesterName }, "UQ_Semesters").IsUnique();

            entity.Property(e => e.SemesterId).HasColumnName("SemesterID");
            entity.Property(e => e.AcademicYearId).HasColumnName("AcademicYearID");
            entity.Property(e => e.SemesterName).HasMaxLength(20);

            entity.HasOne(d => d.AcademicYear).WithMany(p => p.Semesters)
                .HasForeignKey(d => d.AcademicYearId)
                .HasConstraintName("FK_Semesters_AcademicYears");
        });

        modelBuilder.Entity<Student>(entity =>
        {
<<<<<<< HEAD
            entity.HasKey(e => e.StudentId).HasName("PK__Students__32C52A79482C8ACF");

            entity.HasIndex(e => e.IdcardNumber, "UQ__Students__2CEB98361159120E").IsUnique();
=======
            entity.HasKey(e => e.StudentId).HasName("PK__Students__32C52A79B7718618");

            entity.HasIndex(e => e.IdcardNumber, "UQ__Students__2CEB98367A880F7C").IsUnique();
>>>>>>> 6168a45d57591b82132715f37f255d69d80459f9

            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.BirthPlace).HasMaxLength(255);
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.EnrollmentType).HasMaxLength(50);
            entity.Property(e => e.Ethnicity).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.IdcardNumber)
                .HasMaxLength(20)
                .HasColumnName("IDCardNumber");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
            entity.Property(e => e.PermanentAddress).HasMaxLength(255);
            entity.Property(e => e.Religion).HasMaxLength(50);
            entity.Property(e => e.RepeatingYear).HasDefaultValue(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Đang học");

            entity.HasOne(d => d.Parent).WithMany(p => p.Students)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Students_Parents");
        });

        modelBuilder.Entity<StudentClass>(entity =>
        {
<<<<<<< HEAD
            entity.HasKey(e => e.Id).HasName("PK__StudentC__3214EC274F6500B1");

            entity.HasIndex(e => new { e.StudentId, e.AcademicYearId }, "UQ__StudentC__3E91EDDAADCB4D3A").IsUnique();
=======
            entity.HasKey(e => e.Id).HasName("PK__StudentC__3214EC277C4EDEF6");

            entity.HasIndex(e => new { e.StudentId, e.AcademicYearId }, "UQ__StudentC__3E91EDDAEA578191").IsUnique();
>>>>>>> 6168a45d57591b82132715f37f255d69d80459f9

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AcademicYearId).HasColumnName("AcademicYearID");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.AcademicYear).WithMany(p => p.StudentClasses)
                .HasForeignKey(d => d.AcademicYearId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentClasses_AcademicYears");

            entity.HasOne(d => d.Class).WithMany(p => p.StudentClasses)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK_StudentClasses_Classes");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentClasses)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_StudentClasses_Students");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
<<<<<<< HEAD
            entity.HasKey(e => e.SubjectId).HasName("PK__Subjects__AC1BA3886D8B24FF");

            entity.HasIndex(e => e.SubjectName, "UQ__Subjects__4C5A7D55517490A5").IsUnique();
=======
            entity.HasKey(e => e.SubjectId).HasName("PK__Subjects__AC1BA38899757A56");

            entity.HasIndex(e => e.SubjectName, "UQ__Subjects__4C5A7D55F5674572").IsUnique();
>>>>>>> 6168a45d57591b82132715f37f255d69d80459f9

            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.SubjectCategory).HasMaxLength(50);
            entity.Property(e => e.SubjectName).HasMaxLength(100);
            entity.Property(e => e.TypeOfGrade).HasMaxLength(50);
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
<<<<<<< HEAD
            entity.HasKey(e => e.TeacherId).HasName("PK__Teachers__EDF259441D21959A");

            entity.HasIndex(e => e.UserId, "UQ__Teachers__1788CCADEA30EBEE").IsUnique();

            entity.HasIndex(e => e.IdcardNumber, "UQ__Teachers__2CEB9836B4BBCD72").IsUnique();
=======
            entity.HasKey(e => e.TeacherId).HasName("PK__Teachers__EDF25944A0BBD7CE");

            entity.HasIndex(e => e.UserId, "UQ__Teachers__1788CCADEC50461C").IsUnique();

            entity.HasIndex(e => e.IdcardNumber, "UQ__Teachers__2CEB9836FA826D87").IsUnique();
>>>>>>> 6168a45d57591b82132715f37f255d69d80459f9

            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
            entity.Property(e => e.AdditionalDuties).HasMaxLength(255);
            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.EmploymentStatus).HasMaxLength(50);
            entity.Property(e => e.EmploymentType).HasMaxLength(100);
            entity.Property(e => e.Ethnicity).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.Hometown).HasMaxLength(255);
            entity.Property(e => e.IdcardNumber)
                .HasMaxLength(20)
                .HasColumnName("IDCardNumber");
            entity.Property(e => e.InsuranceNumber).HasMaxLength(20);
            entity.Property(e => e.IsHeadOfDepartment).HasDefaultValue(false);
            entity.Property(e => e.MaritalStatus).HasMaxLength(50);
            entity.Property(e => e.PermanentAddress).HasMaxLength(255);
            entity.Property(e => e.Position).HasMaxLength(100);
            entity.Property(e => e.RecruitmentAgency).HasMaxLength(255);
            entity.Property(e => e.Religion).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithOne(p => p.Teacher)
                .HasForeignKey<Teacher>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Teachers_Users");
        });

        modelBuilder.Entity<TeacherClass>(entity =>
        {
<<<<<<< HEAD
            entity.HasKey(e => e.Id).HasName("PK__TeacherC__3214EC277059CC67");
=======
            entity.HasKey(e => e.Id).HasName("PK__TeacherC__3214EC27809C2591");
>>>>>>> 6168a45d57591b82132715f37f255d69d80459f9

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AcademicYearId).HasColumnName("AcademicYearID");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.IsHomeroomTeacher).HasDefaultValue(false);
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.AcademicYear).WithMany(p => p.TeacherClasses)
                .HasForeignKey(d => d.AcademicYearId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TeacherClasses_AcademicYears");

            entity.HasOne(d => d.Class).WithMany(p => p.TeacherClasses)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK_TeacherClasses_Classes");

            entity.HasOne(d => d.Teacher).WithMany(p => p.TeacherClasses)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK_TeacherClasses_Teachers");
        });

        modelBuilder.Entity<TeacherSubject>(entity =>
        {
<<<<<<< HEAD
            entity.HasKey(e => e.Id).HasName("PK__TeacherS__3214EC27E0F63448");
=======
            entity.HasKey(e => e.Id).HasName("PK__TeacherS__3214EC2705983496");
>>>>>>> 6168a45d57591b82132715f37f255d69d80459f9

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IsMainSubject).HasDefaultValue(false);
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.Subject).WithMany(p => p.TeacherSubjects)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TeacherSubjects_Subjects");

            entity.HasOne(d => d.Teacher).WithMany(p => p.TeacherSubjects)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TeacherSubjects_Teachers");
        });

        modelBuilder.Entity<TeachingAssignment>(entity =>
        {
<<<<<<< HEAD
            entity.HasKey(e => e.AssignmentId).HasName("PK__Teaching__32499E57EEA43D32");
=======
            entity.HasKey(e => e.AssignmentId).HasName("PK__Teaching__32499E5718536EA3");
>>>>>>> 6168a45d57591b82132715f37f255d69d80459f9

            entity.Property(e => e.AssignmentId).HasColumnName("AssignmentID");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.SemesterId).HasColumnName("SemesterID");
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.Class).WithMany(p => p.TeachingAssignments)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK_TeachingAssignments_Classes");

            entity.HasOne(d => d.Semester).WithMany(p => p.TeachingAssignments)
                .HasForeignKey(d => d.SemesterId)
                .HasConstraintName("FK_TeachingAssignments_Semesters");

            entity.HasOne(d => d.Subject).WithMany(p => p.TeachingAssignments)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("FK_TeachingAssignments_Subjects");

            entity.HasOne(d => d.Teacher).WithMany(p => p.TeachingAssignments)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK_TeachingAssignments_Teachers");
        });

        modelBuilder.Entity<Timetable>(entity =>
        {
<<<<<<< HEAD
            entity.HasKey(e => e.TimetableId).HasName("PK__Timetabl__68413F60CFA4DFF9");
=======
            entity.HasKey(e => e.TimetableId).HasName("PK__Timetabl__68413F60CD584ABE");
>>>>>>> 6168a45d57591b82132715f37f255d69d80459f9

            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.Semester).WithMany(p => p.Timetables)
                .HasForeignKey(d => d.SemesterId)
                .HasConstraintName("FK__Timetable__Semes__1BC821DD");
        });

        modelBuilder.Entity<TimetableDetail>(entity =>
        {
<<<<<<< HEAD
            entity.HasKey(e => e.TimetableDetailId).HasName("PK__Timetabl__56B983EAA3CCD69A");
=======
            entity.HasKey(e => e.TimetableDetailId).HasName("PK__Timetabl__56B983EA73B64AA6");
>>>>>>> 6168a45d57591b82132715f37f255d69d80459f9

            entity.HasOne(d => d.Class).WithMany(p => p.TimetableDetails)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__Timetable__Class__22751F6C");

            entity.HasOne(d => d.Subject).WithMany(p => p.TimetableDetails)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("FK__Timetable__Subje__236943A5");

            entity.HasOne(d => d.Teacher).WithMany(p => p.TimetableDetails)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK__Timetable__Teach__245D67DE");

            entity.HasOne(d => d.Timetable).WithMany(p => p.TimetableDetails)
                .HasForeignKey(d => d.TimetableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Timetable__Timet__2180FB33");
        });

        modelBuilder.Entity<User>(entity =>
        {
<<<<<<< HEAD
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC87B26C5E");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E46C32D65A").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4E8388C21").IsUnique();
=======
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC8715ED57");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E455E35990").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4BFEB9957").IsUnique();
>>>>>>> 6168a45d57591b82132715f37f255d69d80459f9

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Hoạt Động");
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
