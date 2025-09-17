using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using WebApplication1.Services;
using WebApplication1.Validators;
using WebApplication1.Middlewares;
using WebApplication1.Setting;
using WebApplication1.AppDbContextEF;
using WebApplication1.Repositories;





Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() 
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day) 
    .Enrich.FromLogContext()
    .MinimumLevel.Debug() 
    .CreateLogger();
var builder = WebApplication.CreateBuilder(args);


// ===========================
// Services
// ===========================

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Business Services
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<AuthService>();

// Cache
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICacheService, CacheService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Controllers + FluentValidation
builder.Services.AddControllers()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<UserLoginDtoValidator>();
        fv.RegisterValidatorsFromAssemblyContaining<UserNameLoginDtoValidator>();
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Redis Cache (اختیاری)
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:Configuration"] ?? "localhost:6379";
    options.InstanceName = builder.Configuration["Redis:InstanceName"] ?? "BookApp_";
});

// JWT
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOrAdmin", policy => policy.RequireRole("Admin", "User"));
});

builder.Host.UseSerilog();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>(); // اگر User Repository داری
builder.Services.AddScoped<BookService>();



// ===========================
// Middleware
// ===========================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();
