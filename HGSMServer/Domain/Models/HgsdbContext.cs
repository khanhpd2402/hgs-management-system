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

    public virtual DbSet<TeacherClass> TeacherClasses { get; set; }

    public virtual DbSet<TeacherSubject> TeacherSubjects { get; set; }

    public virtual DbSet<TeachingAssignment> TeachingAssignments { get; set; }

    public virtual DbSet<Timetable> Timetables { get; set; }

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
        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => e.AttendanceId).HasName("PK__Attendan__8B69263C921F5ACC");

            entity.Property(e => e.AttendanceId).HasColumnName("AttendanceID");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Student).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Attendanc__Stude__02084FDA");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__Classes__CB1927A086245EF4");

            entity.HasIndex(e => e.ClassName, "UQ__Classes__F8BF561B4EFF51D9").IsUnique();

            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.ClassName).HasMaxLength(50);
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.ExamId).HasName("PK__Exams__297521A781D1AEC9");

            entity.Property(e => e.ExamId).HasColumnName("ExamID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Exams)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Exams__CreatedBy__6A30C649");

            entity.HasOne(d => d.Subject).WithMany(p => p.Exams)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Exams__SubjectID__693CA210");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.GradeId).HasName("PK__Grades__54F87A37A70EDA2C");

            entity.Property(e => e.GradeId).HasColumnName("GradeID");
            entity.Property(e => e.ExamId).HasColumnName("ExamID");
            entity.Property(e => e.Score).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");

            entity.HasOne(d => d.Exam).WithMany(p => p.Grades)
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Grades__ExamID__6EF57B66");

            entity.HasOne(d => d.Student).WithMany(p => p.Grades)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Grades__StudentI__6D0D32F4");

            entity.HasOne(d => d.Subject).WithMany(p => p.Grades)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Grades__SubjectI__6E01572D");
        });

        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__LeaveReq__33A8519A079A8078");

            entity.Property(e => e.RequestId).HasColumnName("RequestID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.LeaveRequests)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK__LeaveRequ__Appro__0A9D95DB");

            entity.HasOne(d => d.Teacher).WithMany(p => p.LeaveRequests)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LeaveRequ__Teach__09A971A2");
        });

        modelBuilder.Entity<LessonPlan>(entity =>
        {
            entity.HasKey(e => e.PlanId).HasName("PK__LessonPl__755C22D7B7EABD03");

            entity.Property(e => e.PlanId).HasColumnName("PlanID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.Subject).WithMany(p => p.LessonPlans)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LessonPla__Subje__7A672E12");

            entity.HasOne(d => d.Teacher).WithMany(p => p.LessonPlans)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LessonPla__Teach__797309D9");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E32DF0E0E03");

            entity.Property(e => e.NotificationId).HasColumnName("NotificationID");
            entity.Property(e => e.SentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__05D8E0BE");
        });

        modelBuilder.Entity<Parent>(entity =>
        {
            entity.HasKey(e => e.ParentId).HasName("PK__Parents__D339510F0A006751");

            entity.HasIndex(e => e.UserId, "UQ__Parents__1788CCAD4DC4F414").IsUnique();

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
            entity.HasKey(e => e.RewardId).HasName("PK__Rewards__82501599EF0E77A4");

            entity.Property(e => e.RewardId).HasColumnName("RewardID");
            entity.Property(e => e.RewardType).HasMaxLength(50);
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Rewards)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Rewards__Teacher__75A278F5");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3AD50358A1");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B616052F175BF").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Students__32C52A79E2010D97");

            entity.HasIndex(e => e.IdcardNumber, "UQ__Students__2CEB983625750E4B").IsUnique();

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
                .HasDefaultValue("Đang học");

            entity.HasOne(d => d.Class).WithMany(p => p.Students)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Students__ClassI__5629CD9C");

            entity.HasMany(d => d.Parents).WithMany(p => p.Students)
                .UsingEntity<Dictionary<string, object>>(
                    "StudentParent",
                    r => r.HasOne<Parent>().WithMany()
                        .HasForeignKey("ParentId")
                        .HasConstraintName("FK__StudentPa__Paren__72C60C4A"),
                    l => l.HasOne<Student>().WithMany()
                        .HasForeignKey("StudentId")
                        .HasConstraintName("FK__StudentPa__Stude__71D1E811"),
                    j =>
                    {
                        j.HasKey("StudentId", "ParentId").HasName("PK__StudentP__DFF6BF6924160500");
                        j.ToTable("StudentParents");
                        j.IndexerProperty<int>("StudentId").HasColumnName("StudentID");
                        j.IndexerProperty<int>("ParentId").HasColumnName("ParentID");
                    });
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK__Subjects__AC1BA3881505DB40");

            entity.HasIndex(e => e.SubjectName, "UQ__Subjects__4C5A7D5575784F79").IsUnique();

            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.SubjectCategory)
                .HasMaxLength(50)
                .HasDefaultValue("KHTN");
            entity.Property(e => e.SubjectName).HasMaxLength(100);
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.TeacherId).HasName("PK__Teachers__EDF25944FD10E518");

            entity.HasIndex(e => e.InsuranceNumber, "UQ__Teachers__01A4DDAC660F4D4D").IsUnique();

            entity.HasIndex(e => e.UserId, "UQ__Teachers__1788CCAD1E95AC73").IsUnique();

            entity.HasIndex(e => e.IdcardNumber, "UQ__Teachers__2CEB98366DD1F5FE").IsUnique();

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
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Teachers_Users");
        });

        modelBuilder.Entity<TeacherClass>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TeacherC__3214EC2737A6F4EB");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.IsHomeroomTeacher).HasDefaultValue(false);
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.Class).WithMany(p => p.TeacherClasses)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__TeacherCl__Class__4BAC3F29");

            entity.HasOne(d => d.Teacher).WithMany(p => p.TeacherClasses)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK__TeacherCl__Teach__4AB81AF0");
        });

        modelBuilder.Entity<TeacherSubject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TeacherS__3214EC27B3BE30BF");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IsMainSubject).HasDefaultValue(false);
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.Subject).WithMany(p => p.TeacherSubjects)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TeacherSu__Subje__5FB337D6");

            entity.HasOne(d => d.Teacher).WithMany(p => p.TeacherSubjects)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TeacherSu__Teach__5EBF139D");
        });

        modelBuilder.Entity<TeachingAssignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId).HasName("PK__Teaching__32499E578F440163");

            entity.Property(e => e.AssignmentId).HasColumnName("AssignmentID");
            entity.Property(e => e.AcademicYear)
                .HasMaxLength(9)
                .IsUnicode(false);
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.Class).WithMany(p => p.TeachingAssignments)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TeachingA__Class__656C112C");

            entity.HasOne(d => d.Subject).WithMany(p => p.TeachingAssignments)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TeachingA__Subje__6477ECF3");

            entity.HasOne(d => d.Teacher).WithMany(p => p.TeachingAssignments)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TeachingA__Teach__6383C8BA");
        });

        modelBuilder.Entity<Timetable>(entity =>
        {
            entity.HasKey(e => e.TimetableId).HasName("PK__Timetabl__68413F4038C6CBEF");

            entity.Property(e => e.TimetableId).HasColumnName("TimetableID");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.DayOfWeek).HasMaxLength(20);
            entity.Property(e => e.SchoolYear).HasMaxLength(10);
            entity.Property(e => e.Shift).HasMaxLength(20);
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.Class).WithMany(p => p.Timetables)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Timetable__Class__7D439ABD");

            entity.HasOne(d => d.Subject).WithMany(p => p.Timetables)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Timetable__Subje__7E37BEF6");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Timetables)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Timetable__Teach__7F2BE32F");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC32DD80FF");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E421EF3EFC").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534570E91A8").IsUnique();

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
