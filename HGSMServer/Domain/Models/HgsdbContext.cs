using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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

    public virtual DbSet<Conduct> Conducts { get; set; }

    public virtual DbSet<ExamProposal> ExamProposals { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<GradeBatch> GradeBatches { get; set; }

    public virtual DbSet<GradeLevel> GradeLevels { get; set; }

    public virtual DbSet<GradeLevelSubject> GradeLevelSubjects { get; set; }

    public virtual DbSet<HomeroomAssignment> HomeroomAssignments { get; set; }

    public virtual DbSet<LeaveRequest> LeaveRequests { get; set; }

    public virtual DbSet<LessonPlan> LessonPlans { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Parent> Parents { get; set; }

    public virtual DbSet<Period> Periods { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Semester> Semesters { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentClass> StudentClasses { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<SubstituteTeaching> SubstituteTeachings { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<TeacherSubject> TeacherSubjects { get; set; }

    public virtual DbSet<TeachingAssignment> TeachingAssignments { get; set; }

    public virtual DbSet<Timetable> Timetables { get; set; }

    public virtual DbSet<TimetableDetail> TimetableDetails { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var builder = new ConfigurationBuilder()
     .SetBasePath(Directory.GetCurrentDirectory())
     .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        IConfigurationRoot configuration = builder.Build();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("MyCnn"));
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AcademicYear>(entity =>
        {
            entity.HasKey(e => e.AcademicYearId).HasName("PK__Academic__C54C7A2161DE46B6");

            entity.HasIndex(e => e.YearName, "UQ__Academic__294C4DA9C87DA0D4").IsUnique();

            entity.Property(e => e.AcademicYearId).HasColumnName("AcademicYearID");
            entity.Property(e => e.YearName)
                .HasMaxLength(9)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => e.AttendanceId).HasName("PK__Attendan__8B69263C43607024");

            entity.HasIndex(e => new { e.StudentClassId, e.Date, e.Session }, "UQ_StudentClass_Date_Session").IsUnique();

            entity.Property(e => e.AttendanceId).HasColumnName("AttendanceID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.Session).HasMaxLength(10);
            entity.Property(e => e.Status).HasMaxLength(2);
            entity.Property(e => e.StudentClassId).HasColumnName("StudentClassID");

            entity.HasOne(d => d.StudentClass).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.StudentClassId)
                .HasConstraintName("FK_Attendances_StudentClasses");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__Classes__CB1927A047F9C640");

            entity.HasIndex(e => e.ClassName, "UQ__Classes__F8BF561B73358199").IsUnique();

            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.ClassName).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Hoạt động");

            entity.HasOne(d => d.GradeLevel).WithMany(p => p.Classes)
                .HasForeignKey(d => d.GradeLevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Classes_GradeLevels");
        });

        modelBuilder.Entity<Conduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Conducts__3214EC0740105179");

            entity.HasIndex(e => new { e.StudentId, e.SemesterId }, "UQ_Conduct_Student_Semester").IsUnique();

            entity.Property(e => e.ConductType).HasMaxLength(50);

            entity.HasOne(d => d.Semester).WithMany(p => p.Conducts)
                .HasForeignKey(d => d.SemesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Conducts_Semesters");

            entity.HasOne(d => d.Student).WithMany(p => p.Conducts)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Conducts_Students");
        });

        modelBuilder.Entity<ExamProposal>(entity =>
        {
            entity.HasKey(e => e.ProposalId).HasName("PK__ExamProp__6F39E100762EBA0E");

            entity.Property(e => e.ProposalId).HasColumnName("ProposalID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FileUrl).HasMaxLength(500);
            entity.Property(e => e.SemesterId).HasColumnName("SemesterID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Ch? duy?t");
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

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.GradeId).HasName("PK__Grades__54F87A37A23312E4");

            entity.Property(e => e.GradeId).HasColumnName("GradeID");
            entity.Property(e => e.AssessmentsTypeName).HasMaxLength(100);
            entity.Property(e => e.AssignmentId).HasColumnName("AssignmentID");
            entity.Property(e => e.BatchId).HasColumnName("BatchID");
            entity.Property(e => e.Score).HasMaxLength(10);
            entity.Property(e => e.StudentClassId).HasColumnName("StudentClassID");

            entity.HasOne(d => d.Assignment).WithMany(p => p.Grades)
                .HasForeignKey(d => d.AssignmentId)
                .HasConstraintName("FK__Grades__Assignme__17F790F9");

            entity.HasOne(d => d.Batch).WithMany(p => p.Grades)
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK__Grades__BatchID__18EBB532");

            entity.HasOne(d => d.StudentClass).WithMany(p => p.Grades)
                .HasForeignKey(d => d.StudentClassId)
                .HasConstraintName("FK__Grades__StudentC__19DFD96B");
        });

        modelBuilder.Entity<GradeBatch>(entity =>
        {
            entity.HasKey(e => e.BatchId).HasName("PK__GradeBat__5D55CE38EA69F817");

            entity.ToTable(tb => tb.HasTrigger("trg_EnsureSingleActiveBatch"));

            entity.Property(e => e.BatchId).HasColumnName("BatchID");
            entity.Property(e => e.BatchName).HasMaxLength(255);
            entity.Property(e => e.SemesterId).HasColumnName("SemesterID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Hoạt động");

            entity.HasOne(d => d.Semester).WithMany(p => p.GradeBatches)
                .HasForeignKey(d => d.SemesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GradeBatches_Semesters");
        });

        modelBuilder.Entity<GradeLevel>(entity =>
        {
            entity.HasKey(e => e.GradeLevelId).HasName("PK__GradeLev__A200CF13CD296B75");

            entity.Property(e => e.GradeName).HasMaxLength(20);
        });

        modelBuilder.Entity<GradeLevelSubject>(entity =>
        {
            entity.HasKey(e => e.GradeLevelSubjectId).HasName("PK__GradeLev__6AF17C405075A90E");

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

        modelBuilder.Entity<HomeroomAssignment>(entity =>
        {
            entity.HasKey(e => e.HomeroomAssignmentId).HasName("PK__Homeroom__5B4EFAE93CEE0B01");

            entity.HasIndex(e => new { e.ClassId, e.SemesterId, e.Status }, "UQ_HomeroomAssignments").IsUnique();

            entity.Property(e => e.HomeroomAssignmentId).HasColumnName("HomeroomAssignmentID");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.SemesterId).HasColumnName("SemesterID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Hoạt động");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.Class).WithMany(p => p.HomeroomAssignments)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK_HomeroomAssignments_Classes");

            entity.HasOne(d => d.Semester).WithMany(p => p.HomeroomAssignments)
                .HasForeignKey(d => d.SemesterId)
                .HasConstraintName("FK_HomeroomAssignments_Semesters");

            entity.HasOne(d => d.Teacher).WithMany(p => p.HomeroomAssignments)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK_HomeroomAssignments_Teachers");
        });

        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__LeaveReq__33A8519AD89E40B2");

            entity.Property(e => e.RequestId).HasColumnName("RequestID");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.Teacher).WithMany(p => p.LeaveRequests)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK_LeaveRequests_Teachers");
        });

        modelBuilder.Entity<LessonPlan>(entity =>
        {
            entity.HasKey(e => e.PlanId).HasName("PK__LessonPl__755C22D76EC9415A");

            entity.Property(e => e.PlanId).HasColumnName("PlanID");
            entity.Property(e => e.AttachmentUrl).HasMaxLength(500);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.ReviewedDate).HasColumnType("datetime");
            entity.Property(e => e.SemesterId).HasColumnName("SemesterID");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
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
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E3212A3D2CA");

            entity.Property(e => e.NotificationId).HasColumnName("NotificationID");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Title).HasMaxLength(1000);
        });

        modelBuilder.Entity<Parent>(entity =>
        {
            entity.HasKey(e => e.ParentId).HasName("PK__Parents__D339510FDAC7BB46");

            entity.HasIndex(e => e.UserId, "UQ__Parents__1788CCAD6E0A5964").IsUnique();

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

        modelBuilder.Entity<Period>(entity =>
        {
            entity.HasKey(e => e.PeriodId).HasName("PK__Periods__E521BB16EF8D045B");

            entity.Property(e => e.PeriodName).HasMaxLength(50);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A56227161");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160E9D3D5C9").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Semester>(entity =>
        {
            entity.HasKey(e => e.SemesterId).HasName("PK__Semester__043301BD9A8988C5");

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
            entity.HasKey(e => e.StudentId).HasName("PK__Students__32C52A7994CAFDB2");

            entity.HasIndex(e => e.IdcardNumber, "UQ__Students__2CEB9836084E490D").IsUnique();

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
            entity.HasKey(e => e.Id).HasName("PK__StudentC__3214EC2715899831");

            entity.HasIndex(e => new { e.StudentId, e.AcademicYearId }, "UQ__StudentC__3E91EDDA9E7B31D6").IsUnique();

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
            entity.HasKey(e => e.SubjectId).HasName("PK__Subjects__AC1BA388461E3841");

            entity.HasIndex(e => e.SubjectName, "UQ__Subjects__4C5A7D55FAF7D060").IsUnique();

            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.SubjectCategory).HasMaxLength(50);
            entity.Property(e => e.SubjectName).HasMaxLength(100);
            entity.Property(e => e.TypeOfGrade).HasMaxLength(50);
        });

        modelBuilder.Entity<SubstituteTeaching>(entity =>
        {
            entity.HasKey(e => e.SubstituteId).HasName("PK__Substitu__A138776EC3186723");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(255);

            entity.HasOne(d => d.OriginalTeacher).WithMany(p => p.SubstituteTeachingOriginalTeachers)
                .HasForeignKey(d => d.OriginalTeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Substitut__Origi__282DF8C2");

            entity.HasOne(d => d.SubstituteTeacher).WithMany(p => p.SubstituteTeachingSubstituteTeachers)
                .HasForeignKey(d => d.SubstituteTeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Substitut__Subst__29221CFB");

            entity.HasOne(d => d.TimetableDetail).WithMany(p => p.SubstituteTeachings)
                .HasForeignKey(d => d.TimetableDetailId)
                .HasConstraintName("FK__Substitut__Timet__2A164134");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.TeacherId).HasName("PK__Teachers__EDF25944A61966C8");

            entity.HasIndex(e => e.UserId, "UQ__Teachers__1788CCADA6D5EE14").IsUnique();

            entity.HasIndex(e => e.IdcardNumber, "UQ__Teachers__2CEB983667EC8DDD").IsUnique();

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

        modelBuilder.Entity<TeacherSubject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TeacherS__3214EC278E1BC237");

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
            entity.HasKey(e => e.AssignmentId).HasName("PK__Teaching__32499E57965D58F4");

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
            entity.HasKey(e => e.TimetableId).HasName("PK__Timetabl__68413F6081FB2664");

            entity.ToTable(tb => tb.HasTrigger("trg_EnsureOnlyOneActiveTimetable"));

            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Hoạt động");

            entity.HasOne(d => d.Semester).WithMany(p => p.Timetables)
                .HasForeignKey(d => d.SemesterId)
                .HasConstraintName("FK__Timetable__Semes__367C1819");
        });

        modelBuilder.Entity<TimetableDetail>(entity =>
        {
            entity.HasKey(e => e.TimetableDetailId).HasName("PK__Timetabl__56B983EA27C80C97");

            entity.Property(e => e.DayOfWeek).HasMaxLength(20);

            entity.HasOne(d => d.Class).WithMany(p => p.TimetableDetails)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__Timetable__Class__31B762FC");

            entity.HasOne(d => d.Period).WithMany(p => p.TimetableDetails)
                .HasForeignKey(d => d.PeriodId)
                .HasConstraintName("FK__Timetable__Perio__32AB8735");

            entity.HasOne(d => d.Subject).WithMany(p => p.TimetableDetails)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("FK__Timetable__Subje__339FAB6E");

            entity.HasOne(d => d.Teacher).WithMany(p => p.TimetableDetails)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK__Timetable__Teach__3493CFA7");

            entity.HasOne(d => d.Timetable).WithMany(p => p.TimetableDetails)
                .HasForeignKey(d => d.TimetableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Timetable__Timet__3587F3E0");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC5E750B5E");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E40699D069").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4F61550DA").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Hoạt động");
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
