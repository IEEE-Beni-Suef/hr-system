using IEEE.Data;
using IEEE.Entities;
using IEEE.JsonConverters;
using IEEE.Middleware;
using IEEE.Services.Auth;
using IEEE.Services.Email;
using IEEE.Services.Emails;
using IEEE.Services.EmailSettings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. الأساسيات (Services)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. قاعدة البيانات والـ Identity
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, ApplicationRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// 3. إضافة خدماتك الخاصة (Email & Auth Services)
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailConfiguration"));

// 4. الـ Controllers مع الـ Converters الجديدة (شغل زمايلك)
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new FlexibleDateTimeConverter());
        options.JsonSerializerOptions.Converters.Add(new FlexibleNullableDateTimeConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// 5. الـ Authentication والـ JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:IssuerIP"],
        ValidAudience = builder.Configuration["Jwt:AudienceIP"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecritKey"]))
    };
});

// 6. الـ Authorization والـ Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("HighBoardOnly", policy => policy.RequireClaim("RoleId", "1"));
    options.AddPolicy("HeadOnly", policy => policy.RequireClaim("RoleId", "2"));
    options.AddPolicy("MemberOnly", policy => policy.RequireClaim("RoleId", "3"));
    options.AddPolicy("HROnly", policy => policy.RequireClaim("RoleId", "4"));
    options.AddPolicy("ViceOnly", policy => policy.RequireClaim("RoleId", "5"));
    options.AddPolicy("ActiveUserOnly", policy => policy.RequireClaim("IsActive", "True"));
});

// 7. الـ CORS (تعديلات زمايلك المهمة)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                  "https://ieee-mangment.vercel.app",
                  "http://localhost:3000",
                  "http://localhost:5173",
                  "http://localhost:4173",
                  "http://192.168.1.96:5173",
                  "https://localhost:7171"
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// 8. إعدادات الـ Identity والـ Form Limits
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 1;
    options.Password.RequiredUniqueChars = 0;
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 104857600; // 100 MB
});

// --- Middleware Pipeline ---

var app = builder.Build();

app.UseStaticFiles();
app.UseCors("AllowFrontend"); // لازم قبل الـ Routing
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();