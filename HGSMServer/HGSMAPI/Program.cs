using Domain.Models;
using HGSMAPI.AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.OData;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using NLog;
using Infrastructure.Repositories.Implementations;
using Infrastructure.Repositories.Interfaces;
using Common.Constants;
using Application.Features.Students.Interfaces;
using Application.Features.Students.Services;
using Application.Features.Users.Interfaces;
using Application.Features.Users.Services;
using Application.Features.Teachers.Interfaces;
using Application.Features.Role.Interfaces;
using Application.Features.Role.Services;
using Common.Utils;
using Application.Features.Timetables.Services;
using Application.Features.Timetables.Interfaces;
using Application.Features.Classes.Interfaces;
using Application.Features.Classes.Services;
using Application.Features.HomeRooms.Services;
using Application.Features.HomeRooms.Interfaces;
using Microsoft.EntityFrameworkCore;
using Application.Features.Attendances.Interfaces;
using Application.Features.Attendances.Services;
using Application.Features.GradeBatchs.Interfaces;
using Application.Services;
using Application.Features.Grades.Interfaces;
using Application.Features.Grades.Services;
using Application.Features.Subjects.Interfaces;
using Application.Features.Subjects.Services;
using Application.Features.Semesters.Interfaces;
using Application.Features.Semesters.Services;
using Application.Features.AcademicYears.Interfaces;
using Application.Features.AcademicYears.Services;
using Application.Features.LeaveRequests.Interfaces;
using Application.Features.LeaveRequests.Services;
using Application.Features.LessonPlans.Interfaces;
using Application.Features.LessonPlans.Services;
using Application.Features.TeachingAssignments.Interfaces;
using Application.Features.Attendances.DTOs;
using Infrastructure.Repositories.Implementtations;
using Infrastructure.Repositories;
using Application.Features.Exams.Interfaces;
using Application.Features.Exams.Services;
using HGSMAPI;
using Infrastructure.Repositories.UnitOfWork;
using Application.Features.GradeLevelSubjects.Interfaces;
using Application.Features.GradeLevelSubjects.Services;
using Application.Features.GradeLevels.Interfaces;
using Application.Features.GradeLevels.Services;
using Application.Features.Periods.Interfaces;
using Application.Features.Periods.Services;
using Application.Features.StudentClass.Interfaces;
using Application.Features.StudentClass.Services;
using Application.Features.TeacherSubjects.Services;
using Application.Features.TeacherSubjects.Interfaces;
using Application.Features.SubstituteTeachings.Interfaces;
using Application.Features.SubstituteTeachings.Services;
using Application.Features.Conducts.Interfaces;
using Application.Features.Conducts.Services;
using Application.Features.Statistics.Interfaces;
using Application.Features.Statistics.Services;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình NLog
LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

// Đọc khóa từ appsettings.json
var encryptionKey = builder.Configuration["SecuritySettings:EncryptionKey"];
if (string.IsNullOrEmpty(encryptionKey))
{
    throw new InvalidOperationException("EncryptionKey is not configured in appsettings.json.");
}

// Đăng ký SecurityHelper vào DI container
builder.Services.AddSingleton(new SecurityHelper(encryptionKey));
//config email sending
var emailSettings = builder.Configuration.GetSection("EmailSettings");
builder.Services.AddSingleton(new EmailService(
    smtpHost: emailSettings["SmtpHost"],
    smtpPort: int.Parse(emailSettings["SmtpPort"]),
    smtpUsername: emailSettings["SmtpUsername"],
    smtpPassword: emailSettings["SmtpPassword"],
    fromEmail: emailSettings["FromEmail"],
    fromName: emailSettings["FromName"]
));

// Thêm CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
// Thiết lập culture mặc định cho ứng dụng
var cultureInfo = new System.Globalization.CultureInfo("vi-VN");
System.Globalization.CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Thêm controllers và cấu hình JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    })
    .AddOData(op => op.Select().Expand().Filter().Count().OrderBy().SetMaxTop(AppConstants.MAX_TOP_ODATA));

// Thêm AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Thêm DbContext
builder.Services.AddDbContext<HgsdbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));

// Thêm Session
builder.Services.AddDistributedMemoryCache(); // Sử dụng bộ nhớ trong để lưu session (cho dev/test)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".HGSM.Session";
});

// Đăng ký các dịch vụ và repository
//Exam, Question Management
builder.Services.AddScoped<IExamProposalService, ExamProposalService>();
builder.Services.AddScoped<IExamProposalRepository, ExamProposalRepository>();
builder.Services.AddScoped<GoogleDriveService>();
//Parent Management
builder.Services.AddScoped<IParentRepository, ParentRepository>();
// Student Management
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IStudentClassService, StudentClassService>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IGradeService, GradeService>();
builder.Services.AddScoped<IGradeRepository, GradeRepository>();
builder.Services.AddScoped<IGradeBatchService, GradeBatchService>();
builder.Services.AddScoped<IGradeBatchRepository, GradeBatchRepository>();
builder.Services.AddScoped<ITeachingAssignmentRepository, TeachingAssignmentRepository>();
builder.Services.AddScoped<IStudentClassRepository, StudentClassRepository>();
builder.Services.AddScoped<IGradeLevelSubjectRepository, GradeLevelSubjectRepository>();
builder.Services.AddScoped<IGradeLevelSubjectService, GradeLevelSubjectService>();
builder.Services.AddScoped<IGradeUnitOfWork, GradeUnitOfWork>();
builder.Services.AddScoped<IAttendanceUnitOfWork, AttendanceUnitOfWork>();

// Teacher Management
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<ITeacherSubjectRepository, TeacherSubjectRepository>();
builder.Services.AddScoped<IHomeroomAssignmentRepository, HomeroomAssignmentRepository>();
builder.Services.AddScoped<ITeacherSubjectService, TeacherSubjectService>();
builder.Services.AddScoped<ITeachingAssignmentService, TeachingAssignmentService>();
builder.Services.AddScoped<ILessonPlanService, LessonPlanService>();
builder.Services.AddScoped<ILessonPlanRepository, LessonPlanRepository>();
builder.Services.AddScoped<IAssignHomeRoomService, AssignHomeRoomService>();
builder.Services.AddScoped<IStatisticsRepository, StatisticsRepository>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
// Class & Timetable Management
builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddScoped<IClassRepository, ClassRepository>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<ITimetableRepository, TimetableRepository>();
builder.Services.AddScoped<ITimetableDetailRepository, TimetableDetailRepository>();
builder.Services.AddScoped<ITimetableService, TimetableService>();
builder.Services.AddScoped<IGradeLevelService, GradeLevelService>();
builder.Services.AddScoped<IGradeLevelRepository, GradeLevelRepository>();
builder.Services.AddScoped<IPeriodService, PeriodService>();
builder.Services.AddScoped<IPeriodRepository, PeriodRepository>();
builder.Services.AddScoped<ISubstituteTeachingRepository, SubstituteTeachingRepository>();
builder.Services.AddScoped<ISubstituteTeachingService, SubstituteTeachingService>();
builder.Services.AddScoped<ITimetableUnitOfWork, TimetableUnitOfWork>();
// Academic Year & Semester Management
builder.Services.AddScoped<IAcademicYearService, AcademicYearService>();
builder.Services.AddScoped<IAcademicYearRepository, AcademicYearRepository>();
builder.Services.AddScoped<ISemesterService, SemesterService>();
builder.Services.AddScoped<ISemesterRepository, SemesterRepository>();

// Attendance & Leave Management
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
builder.Services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
builder.Services.AddScoped<IConductRepository, ConductRepository>();
builder.Services.AddScoped<IConductService, ConductService>();

// User & Role Management
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

// System & Utility Services
builder.Services.AddScoped<ITokenService, TokenService>();

// Đăng ký Logger
builder.Services.AddSingleton<ILoggerManager, LoggerManager>();

// Thêm HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Configure Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Sửa thành JwtBearer
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/api/Auth/login";
    options.LogoutPath = "/api/Auth/logout";
})
.AddJwtBearer(options =>
{
    var actualKey = builder.Configuration["JWT:SecretKey"];
    Console.WriteLine("JWT SecretKey being used: " + actualKey);

    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(actualKey))
    };

    // Log nếu xác thực token bị fail
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("JWT FAILED: " + context.Exception.Message);
            return Task.CompletedTask;
        }
    };
})

.AddGoogle("Google", options =>
{
    options.ClientId = builder.Configuration["Google:ClientId"];
    options.ClientSecret = builder.Configuration["Google:ClientSecret"];
    options.CallbackPath = "/GoogleLogin/callback";
    options.SaveTokens = true;
    options.Scope.Add("profile");
    options.Scope.Add("email");
});

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Project API", Version = "v1" });

    // Cấu hình Bearer token
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });

    // Cấu hình OAuth2 cho Google
    option.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://accounts.google.com/o/oauth2/auth"),
                TokenUrl = new Uri("https://oauth2.googleapis.com/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "profile", "Access to your profile" },
                    { "email", "Access to your email" }
                }
            }
        }
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new string[] { "profile", "email" }
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
//if (app.Environment.IsDevelopment())
//{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Project API v1");
        options.OAuthClientId(builder.Configuration["Google:ClientId"]);
        options.OAuthScopes("profile", "email");
        options.OAuthUsePkce();
    });
//}

app.UseCors("AllowAll");

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();