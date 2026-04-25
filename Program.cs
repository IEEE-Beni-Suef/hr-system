using IEEE.Data;
using IEEE.Entities;
using IEEE.JsonConverters;
using IEEE.Middleware;
using IEEE.Services.Email;
using IEEE.Services.Emails;
using IEEE.Services.OptionsPatterns;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

namespace IEEE
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ---------------- Swagger & Logging ----------------
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Information);

            // ---------------- Database ----------------
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ---------------- Identity ----------------
            builder.Services.AddIdentity<User, ApplicationRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // ---------------- Services ----------------
          //  builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.Configure<EmailSettings>(
                builder.Configuration.GetSection("EmailConfiguration"));

            // ---------------- Controllers + JSON ----------------
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.Converters.Add(new FlexibleDateTimeConverter());
                    options.JsonSerializerOptions.Converters.Add(new FlexibleNullableDateTimeConverter());
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                });

            // ---------------- JWT Authentication ----------------
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
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
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
                };
            });

            // ---------------- Authorization Policies ----------------
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("HighBoardOnly", policy => policy.RequireClaim("RoleId", "1"));
                options.AddPolicy("HeadOnly", policy => policy.RequireClaim("RoleId", "2"));
                options.AddPolicy("MemberOnly", policy => policy.RequireClaim("RoleId", "3"));
                options.AddPolicy("HROnly", policy => policy.RequireClaim("RoleId", "4"));
                options.AddPolicy("ViceOnly", policy => policy.RequireClaim("RoleId", "5"));
                options.AddPolicy("ActiveUserOnly", policy => policy.RequireClaim("IsActive", "True"));
            });

            // ---------------- Rate Limiting (FIXED - single instance) ----------------
            builder.Services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("EmailSendingPolicy", opt =>
                {
                    opt.PermitLimit = 5;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueLimit = 2;
                    opt.QueueProcessingOrder =
                        System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                });
            });

            // ---------------- CORS ----------------
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

            // ---------------- Identity Password Settings ----------------
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 1;
                options.Password.RequiredUniqueChars = 0;
            });

            // ---------------- Upload Limits ----------------
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 104857600; // 100 MB
            });

            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.Limits.MaxRequestBodySize = 104857600; // 100 MB
            });

            // ---------------- Build App ----------------
            var app = builder.Build();

            app.UseStaticFiles();
            app.UseCors("AllowFrontend");

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // ---------------- Seed Identity ----------------
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var userManager = services.GetRequiredService<UserManager<User>>();
                var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

                await IdentitySeeder.SeedAsync(userManager, roleManager);
            }

            app.Run();
        }
    }
}