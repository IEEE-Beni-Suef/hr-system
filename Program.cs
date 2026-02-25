using IEEE.Data;
using IEEE.Entities;
using IEEE.JsonConverters;
using IEEE.Middleware;
using IEEE.Services.Email;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using IEEE.Services.EmailSettings;
using IEEE.Services.Auth;
using IEEE.Services.Emails;
using IEEE.Services.Email;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Bind EmailSettings from configuration
builder.Services.Configure<IEEE.Services.EmailSettings.EmailSettings>(builder.Configuration.GetSection("EmailConfiguration"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, ApplicationRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        // Existing enum converter
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

        // Flexible DateTime converters (non-nullable and nullable)
        options.JsonSerializerOptions.Converters.Add(new FlexibleDateTimeConverter());
        options.JsonSerializerOptions.Converters.Add(new FlexibleNullableDateTimeConverter());

        // Existing reference handling
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

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


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("HighBoardOnly", policy => policy.RequireClaim("RoleId", "1"));
    options.AddPolicy("HeadOnly", policy => policy.RequireClaim("RoleId", "2"));
    options.AddPolicy("MemberOnly", policy => policy.RequireClaim("RoleId", "3"));
    options.AddPolicy("HROnly", policy => policy.RequireClaim("RoleId", "4"));
    options.AddPolicy("ViceOnly", policy => policy.RequireClaim("RoleId", "5"));

    options.AddPolicy("ActiveUserOnly", policy =>
        policy.RequireClaim("IsActive", "True"));
});

// FIXED CORS CONFIGURATION
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
              .WithOrigins(
                  "https://ieee-mangment.vercel.app",  // Your production frontend
                  "http://localhost:3000",             // Local development
                  "http://localhost:5173",             // Vite default port
                  "http://localhost:4173",
                  "http://192.168.1.96:5173", // Vite preview port و 
                  "https://localhost:7171"
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});


builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 1;   // Minimum length
    options.Password.RequiredUniqueChars = 0;
// Password settings.
options.Password.RequireDigit = false;
options.Password.RequireLowercase = false;
options.Password.RequireNonAlphanumeric = false;
options.Password.RequireUppercase = false;
options.Password.RequiredLength = 1;   // Minimum length
options.Password.RequiredUniqueChars = 0;
});


builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB
});

// لو محتاج تزود كمان حجم البودي كله (لما ترفع ملفات ضخمة مع داتا)
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 104857600; // 100 MB
});


builder.Services.AddAuthorization();

var app = builder.Build();
app.UseStaticFiles();


// CORS must be used BEFORE routing and authentication
app.UseCors("AllowFrontend");

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
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

//using (var scope = app.Services.CreateScope())
//{
//    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//    await DefaultRolesSeeder.SeedAsync(roleManager);
//}

app.Run();