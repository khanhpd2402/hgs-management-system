using Domain.Models;
using HGSMAPI.AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies; // Thêm namespace này
using Microsoft.AspNetCore.OData;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using NLog;
using Infrastructure.Repositories.Implementtations;
using Infrastructure.Repositories.Interfaces;
using Common.Constants;
using Application.Features.Students.Interfaces;
using Application.Features.Students.Services;
using Application.Features.Users.Interfaces;
using Application.Features.Users.Services;
using Application.Features.Teachers.Interfaces;
using Application.Features.Teachers.Services;
using Application.Features.Role.Interfaces;
using Application.Features.Role.Services;
using Common.Utils;
using Application.Features.Timetables.Interfaces;
using Application.Features.Timetables.Services;
using Application.Features.Classes.Interfaces;
using Application.Features.Classes.Services;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Application.Features.Attendances.Interfaces;
using Application.Features.Attendances.Services;
using Application.Features.Attendances.DTOs;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);
LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
// Đọc khóa từ appsettings.json
var encryptionKey = builder.Configuration["SecuritySettings:EncryptionKey"];
// Đăng ký SecurityHelper vào DI container
builder.Services.AddSingleton(new SecurityHelper(encryptionKey));
// Add services to the container.
builder.Services.AddCors();
builder.Services.AddControllers();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddDbContext<HgsdbContext>();
// Service 
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITimetableRepository, TimetableRepository>();
builder.Services.AddScoped<IClassRepository, ClassRepository>();
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
// Repository
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
//builder.Services.AddScoped<ITimetableService, TimetableService>();
builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<ISmsService, TwilioSmsService>();
builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("Twilio")); // Sử dụng builder.Configuration
builder.Services.AddSingleton<ILoggerManager, LoggerManager>();
builder.Services.AddControllers().AddOData(op => op.Select().Expand().Filter().Count().OrderBy().SetMaxTop(AppConstants.MAX_TOP_ODATA));
builder.Services.AddDbContext<HgsdbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<TimetableService>();
// Configure Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "Google"; // Sử dụng Google khi challenge
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Dùng Cookies để lưu phiên
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme) // Thêm Cookie Authentication
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"])),
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
builder.Services.AddHttpContextAccessor();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Project API v1");
        options.OAuthClientId(builder.Configuration["Google:ClientId"]);
        options.OAuthScopes("profile", "email");
        options.OAuthUsePkce();
    });
}
app.UseCors(builder =>
{
    builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
});
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();