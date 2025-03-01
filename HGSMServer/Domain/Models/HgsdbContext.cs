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

    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<Exam> Exams { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<LeaveRequest> LeaveRequests { get; set; }

    public virtual DbSet<LessonPlan> LessonPlans { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Parent> Parents { get; set; }

    public virtual DbSet<Reward> Rewards { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<TeacherSubject> TeacherSubjects { get; set; }

    public virtual DbSet<Timetable> Timetables { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DUYKHANH;Database=HGSDB;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => e.AttendanceId).HasName("PK__Attendan__8B69263CEE163123");

            entity.Property(e => e.AttendanceId).HasColumnName("AttendanceID");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Student).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Attendanc__Stude__76969D2E");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__Classes__CB1927A0D952000D");

            entity.HasIndex(e => e.ClassName, "UQ__Classes__F8BF561B43EBCCCD").IsUnique();

            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.ClassName).HasMaxLength(50);
            entity.Property(e => e.HomeroomTeacherId).HasColumnName("HomeroomTeacherID");

            entity.HasOne(d => d.HomeroomTeacher).WithMany(p => p.Classes)
                .HasForeignKey(d => d.HomeroomTeacherId)
                .HasConstraintName("FK__Classes__Homeroo__48CFD27E");
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.ExamId).HasName("PK__Exams__297521A793EE312A");

            entity.Property(e => e.ExamId).HasColumnName("ExamID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Exams)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Exams__CreatedBy__5EBF139D");

            entity.HasOne(d => d.Subject).WithMany(p => p.Exams)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Exams__SubjectID__5DCAEF64");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.GradeId).HasName("PK__Grades__54F87A375D4A5D19");

            entity.Property(e => e.GradeId).HasColumnName("GradeID");
            entity.Property(e => e.ExamId).HasColumnName("ExamID");
            entity.Property(e => e.Score).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");

            entity.HasOne(d => d.Exam).WithMany(p => p.Grades)
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Grades__ExamID__6383C8BA");

            entity.HasOne(d => d.Student).WithMany(p => p.Grades)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Grades__StudentI__619B8048");

            entity.HasOne(d => d.Subject).WithMany(p => p.Grades)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Grades__SubjectI__628FA481");
        });

        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__LeaveReq__33A8519AE07041BB");

            entity.Property(e => e.RequestId).HasColumnName("RequestID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.LeaveRequests)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK__LeaveRequ__Appro__7F2BE32F");

            entity.HasOne(d => d.Teacher).WithMany(p => p.LeaveRequests)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LeaveRequ__Teach__7E37BEF6");
        });

        modelBuilder.Entity<LessonPlan>(entity =>
        {
            entity.HasKey(e => e.PlanId).HasName("PK__LessonPl__755C22D73EE2232C");

            entity.Property(e => e.PlanId).HasColumnName("PlanID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.Subject).WithMany(p => p.LessonPlans)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LessonPla__Subje__6EF57B66");

            entity.HasOne(d => d.Teacher).WithMany(p => p.LessonPlans)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LessonPla__Teach__6E01572D");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E32BCC50784");

            entity.Property(e => e.NotificationId).HasColumnName("NotificationID");
            entity.Property(e => e.SentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__7A672E12");
        });

        modelBuilder.Entity<Parent>(entity =>
        {
            entity.HasKey(e => e.ParentId).HasName("PK__Parents__D339510FE2B76ADE");

            entity.HasIndex(e => e.UserId, "UQ__Parents__1788CCAD48E520D6").IsUnique();

            entity.Property(e => e.ParentId).HasColumnName("ParentID");
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Occupation).HasMaxLength(100);
            entity.Property(e => e.Relationship).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithOne(p => p.Parent)
                .HasForeignKey<Parent>(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Parents_Users");
        });

        modelBuilder.Entity<Reward>(entity =>
        {
            entity.HasKey(e => e.RewardId).HasName("PK__Rewards__82501599534EFD8A");

            entity.Property(e => e.RewardId).HasColumnName("RewardID");
            entity.Property(e => e.RewardType).HasMaxLength(50);
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Rewards)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Rewards__Teacher__6A30C649");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A2BC8E6A4");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160AE4BFA50").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Students__32C52A796D2DE5ED");

            entity.HasIndex(e => e.IdcardNumber, "UQ__Students__2CEB9836F4374EA1").IsUnique();

            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.BirthPlace).HasMaxLength(255);
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.EnrollmentType).HasMaxLength(50);
            entity.Property(e => e.Ethnicity).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.IdcardNumber)
                .HasMaxLength(20)
                .HasColumnName("IDCardNumber");
            entity.Property(e => e.PermanentAddress).HasMaxLength(255);
            entity.Property(e => e.Religion).HasMaxLength(50);
            entity.Property(e => e.RepeatingYear).HasDefaultValue(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Ðang h?c");

            entity.HasOne(d => d.Class).WithMany(p => p.Students)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Students__ClassI__534D60F1");

            entity.HasMany(d => d.Parents).WithMany(p => p.Students)
                .UsingEntity<Dictionary<string, object>>(
                    "StudentParent",
                    r => r.HasOne<Parent>().WithMany()
                        .HasForeignKey("ParentId")
                        .HasConstraintName("FK__StudentPa__Paren__6754599E"),
                    l => l.HasOne<Student>().WithMany()
                        .HasForeignKey("StudentId")
                        .HasConstraintName("FK__StudentPa__Stude__66603565"),
                    j =>
                    {
                        j.HasKey("StudentId", "ParentId").HasName("PK__StudentP__DFF6BF69602A3AA0");
                        j.ToTable("StudentParents");
                        j.IndexerProperty<int>("StudentId").HasColumnName("StudentID");
                        j.IndexerProperty<int>("ParentId").HasColumnName("ParentID");
                    });
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK__Subjects__AC1BA3883EF982D4");

            entity.HasIndex(e => e.SubjectName, "UQ__Subjects__4C5A7D55FBCD0D3C").IsUnique();

            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.SubjectName).HasMaxLength(100);
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.TeacherId).HasName("PK__Teachers__EDF25944B2A8E664");

            entity.HasIndex(e => e.InsuranceNumber, "UQ__Teachers__01A4DDAC07024087").IsUnique();

            entity.HasIndex(e => e.UserId, "UQ__Teachers__1788CCADE8C44828").IsUnique();

            entity.HasIndex(e => e.IdcardNumber, "UQ__Teachers__2CEB9836E7603409").IsUnique();

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
            entity.Property(e => e.MainSubject).HasMaxLength(100);
            entity.Property(e => e.MaritalStatus).HasMaxLength(50);
            entity.Property(e => e.PermanentAddress).HasMaxLength(255);
            entity.Property(e => e.Position).HasMaxLength(100);
            entity.Property(e => e.RecruitmentAgency).HasMaxLength(255);
            entity.Property(e => e.Religion).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithOne(p => p.Teacher)
                .HasForeignKey<Teacher>(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Teachers_Users");
        });

        modelBuilder.Entity<TeacherSubject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TeacherS__3214EC27CCED2D20");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.Subject).WithMany(p => p.TeacherSubjects)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TeacherSu__Subje__59FA5E80");

            entity.HasOne(d => d.Teacher).WithMany(p => p.TeacherSubjects)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TeacherSu__Teach__59063A47");
        });

        modelBuilder.Entity<Timetable>(entity =>
        {
            entity.HasKey(e => e.TimetableId).HasName("PK__Timetabl__68413F4030D2AA74");

            entity.Property(e => e.TimetableId).HasColumnName("TimetableID");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.DayOfWeek).HasMaxLength(20);
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.Class).WithMany(p => p.Timetables)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Timetable__Class__71D1E811");

            entity.HasOne(d => d.Subject).WithMany(p => p.Timetables)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Timetable__Subje__72C60C4A");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Timetables)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Timetable__Teach__73BA3083");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC04635E04");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4ABF8CFC6").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534F7F3566D").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleID__3D5E1FD2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
