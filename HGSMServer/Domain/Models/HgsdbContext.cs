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

    public virtual DbSet<GradeLevel> GradeLevels { get; set; }

    public virtual DbSet<GradeLevelSubject> GradeLevelSubjects { get; set; }

    public virtual DbSet<LeaveRequest> LeaveRequests { get; set; }

    public virtual DbSet<LessonPlan> LessonPlans { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Parent> Parents { get; set; }

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
            entity.HasKey(e => e.AcademicYearId).HasName("PK__Academic__C54C7A21CD7E9F76");

            entity.HasIndex(e => e.YearName, "UQ__Academic__294C4DA959D5A3F4").IsUnique();

            entity.Property(e => e.AcademicYearId).HasColumnName("AcademicYearID");
            entity.Property(e => e.YearName)
                .HasMaxLength(9)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => e.AttendanceId).HasName("PK__Attendan__8B69263C1DB8E0BF");

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
            entity.HasKey(e => e.ClassId).HasName("PK__Classes__CB1927A0999586A9");

            entity.HasIndex(e => e.ClassName, "UQ__Classes__F8BF561B99E40D3B").IsUnique();

            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.ClassName).HasMaxLength(50);

            entity.HasOne(d => d.GradeLevel).WithMany(p => p.Classes)
                .HasForeignKey(d => d.GradeLevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Classes_GradeLevels");
        });

        modelBuilder.Entity<ExamProposal>(entity =>
        {
            entity.HasKey(e => e.ProposalId).HasName("PK__ExamProp__6F39E10022483E0E");

            entity.Property(e => e.ProposalId).HasColumnName("ProposalID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.SemesterId).HasColumnName("SemesterID");
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.Title).HasMaxLength(255);
        });

        modelBuilder.Entity<ExamProposalQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ExamProp__3214EC27BE4D0F45");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ProposalId).HasColumnName("ProposalID");
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.GradeId).HasName("PK__Grades__54F87A37149A7C67");

            entity.Property(e => e.GradeId).HasColumnName("GradeID");
            entity.Property(e => e.AssessmentsTypeName).HasMaxLength(100);
            entity.Property(e => e.AssignmentId).HasColumnName("AssignmentID");
            entity.Property(e => e.BatchId).HasColumnName("BatchID");
            entity.Property(e => e.Score).HasMaxLength(10);
            entity.Property(e => e.StudentClassId).HasColumnName("StudentClassID");

            entity.HasOne(d => d.Assignment).WithMany(p => p.Grades)
                .HasForeignKey(d => d.AssignmentId)
                .HasConstraintName("FK__Grades__Assignme__03F0984C");

            entity.HasOne(d => d.Batch).WithMany(p => p.Grades)
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK__Grades__BatchID__02084FDA");

            entity.HasOne(d => d.StudentClass).WithMany(p => p.Grades)
                .HasForeignKey(d => d.StudentClassId)
                .HasConstraintName("FK__Grades__StudentC__02FC7413");
        });

        modelBuilder.Entity<GradeBatch>(entity =>
        {
            entity.HasKey(e => e.BatchId).HasName("PK__GradeBat__5D55CE382C4CDAED");

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

        modelBuilder.Entity<GradeLevel>(entity =>
        {
            entity.HasKey(e => e.GradeLevelId).HasName("PK__GradeLev__A200CF133A32280F");

            entity.Property(e => e.GradeName).HasMaxLength(20);
        });

        modelBuilder.Entity<GradeLevelSubject>(entity =>
        {
            entity.HasKey(e => e.GradeLevelSubjectId).HasName("PK__GradeLev__6AF17C40C637277E");

            entity.HasIndex(e => new { e.GradeLevelId, e.SubjectId }, "UQ_GradeLevel_Subject").IsUnique();

            entity.Property(e => e.GradeLevelSubjectId).HasColumnName("GradeLevelSubjectID");
            entity.Property(e => e.ContinuousAssessmentsHki).HasColumnName("ContinuousAssessments_HKI");
            entity.Property(e => e.ContinuousAssessmentsHkii).HasColumnName("ContinuousAssessments_HKII");
            entity.Property(e => e.GradeLevelId).HasColumnName("GradeLevelID");
            entity.Property(e => e.PeriodsPerWeekHki).HasColumnName("PeriodsPerWeek_HKI");
            entity.Property(e => e.PeriodsPerWeekHkii).HasColumnName("PeriodsPerWeek_HKII");
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");

            entity.HasOne(d => d.GradeLevel).WithMany(p => p.GradeLevelSubjects)
                .HasForeignKey(d => d.GradeLevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GLS_GradeLevel");

            entity.HasOne(d => d.Subject).WithMany(p => p.GradeLevelSubjects)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GLS_Subject");
        });

        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__LeaveReq__33A8519A81A418F3");

            entity.Property(e => e.RequestId).HasColumnName("RequestID");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.Teacher).WithMany(p => p.LeaveRequests)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK_LeaveRequests_Teachers");
        });

        modelBuilder.Entity<LessonPlan>(entity =>
        {
            entity.HasKey(e => e.PlanId).HasName("PK__LessonPl__755C22D755193A16");

            entity.Property(e => e.PlanId).HasColumnName("PlanID");
            entity.Property(e => e.AttachmentUrl).HasMaxLength(500);
            entity.Property(e => e.ReviewedDate).HasColumnType("datetime");
            entity.Property(e => e.SemesterId).HasColumnName("SemesterID");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.SubmittedDate).HasColumnType("datetime");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
            entity.Property(e => e.Title).HasMaxLength(255);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E3200A5B3F5");

            entity.Property(e => e.NotificationId).HasColumnName("NotificationID");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Title).HasMaxLength(1000);
        });

        modelBuilder.Entity<Parent>(entity =>
        {
            entity.HasKey(e => e.ParentId).HasName("PK__Parents__D339510FEB00F02A");

            entity.HasIndex(e => e.EmailFather, "UQ_Parents_EmailFather")
                .IsUnique()
                .HasFilter("([EmailFather] IS NOT NULL)");

            entity.HasIndex(e => e.EmailGuardian, "UQ_Parents_EmailGuardian")
                .IsUnique()
                .HasFilter("([EmailGuardian] IS NOT NULL)");

            entity.HasIndex(e => e.EmailMother, "UQ_Parents_EmailMother")
                .IsUnique()
                .HasFilter("([EmailMother] IS NOT NULL)");

            entity.HasIndex(e => e.IdcardNumberFather, "UQ_Parents_IdcardNumberFather")
                .IsUnique()
                .HasFilter("([IdcardNumberFather] IS NOT NULL)");

            entity.HasIndex(e => e.IdcardNumberGuardian, "UQ_Parents_IdcardNumberGuardian")
                .IsUnique()
                .HasFilter("([IdcardNumberGuardian] IS NOT NULL)");

            entity.HasIndex(e => e.IdcardNumberMother, "UQ_Parents_IdcardNumberMother")
                .IsUnique()
                .HasFilter("([IdcardNumberMother] IS NOT NULL)");

            entity.HasIndex(e => e.UserId, "UQ__Parents__1788CCAD9FB28779").IsUnique();

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

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3AD45372A2");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160BAE2887B").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Semester>(entity =>
        {
            entity.HasKey(e => e.SemesterId).HasName("PK__Semester__043301BDB4A1E74A");

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
            entity.HasKey(e => e.StudentId).HasName("PK__Students__32C52A79CDEB9687");

            entity.HasIndex(e => e.IdcardNumber, "UQ__Students__2CEB983697979160").IsUnique();

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
            entity.HasKey(e => e.Id).HasName("PK__StudentC__3214EC2740A22C35");

            entity.HasIndex(e => new { e.StudentId, e.AcademicYearId }, "UQ__StudentC__3E91EDDAA35790A2").IsUnique();

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
            entity.HasKey(e => e.SubjectId).HasName("PK__Subjects__AC1BA388C5EFA8E3");

            entity.HasIndex(e => e.SubjectName, "UQ__Subjects__4C5A7D55A2116707").IsUnique();

            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.SubjectCategory).HasMaxLength(50);
            entity.Property(e => e.SubjectName).HasMaxLength(100);
            entity.Property(e => e.TypeOfGrade).HasMaxLength(50);
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.TeacherId).HasName("PK__Teachers__EDF259449B136DE4");

            entity.HasIndex(e => e.UserId, "UQ__Teachers__1788CCAD213920B0").IsUnique();

            entity.HasIndex(e => e.IdcardNumber, "UQ__Teachers__2CEB9836F1FBE674").IsUnique();

            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
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
            entity.Property(e => e.MainSubject).HasMaxLength(100);
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
            entity.HasKey(e => e.Id).HasName("PK__TeacherC__3214EC27698049D4");

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
            entity.HasKey(e => e.Id).HasName("PK__TeacherS__3214EC276D8EB3A1");

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
            entity.HasKey(e => e.AssignmentId).HasName("PK__Teaching__32499E57FDFA8079");

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
            entity.HasKey(e => e.TimetableId).HasName("PK__Timetabl__68413F6071F11890");

            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.Semester).WithMany(p => p.Timetables)
                .HasForeignKey(d => d.SemesterId)
                .HasConstraintName("FK__Timetable__Statu__14270015");
        });

        modelBuilder.Entity<TimetableDetail>(entity =>
        {
            entity.HasKey(e => e.TimetableDetailId).HasName("PK__Timetabl__56B983EA7059020B");

            entity.HasOne(d => d.Class).WithMany(p => p.TimetableDetails)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__Timetable__Class__1AD3FDA4");

            entity.HasOne(d => d.Subject).WithMany(p => p.TimetableDetails)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("FK__Timetable__Subje__1BC821DD");

            entity.HasOne(d => d.Teacher).WithMany(p => p.TimetableDetails)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK__Timetable__Teach__1CBC4616");

            entity.HasOne(d => d.Timetable).WithMany(p => p.TimetableDetails)
                .HasForeignKey(d => d.TimetableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Timetable__Timet__19DFD96B");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC95B3A6ED");

            entity.HasIndex(e => e.Email, "IX_Users_Email");

            entity.HasIndex(e => e.PhoneNumber, "IX_Users_PhoneNumber");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E491455577").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4CB981462").IsUnique();

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
