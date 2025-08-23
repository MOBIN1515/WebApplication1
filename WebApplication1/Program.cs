// 🧩 1) Using ها: می‌گن از کدوم کتابخانه‌ها استفاده می‌کنیم
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;         // برای تنظیم Swagger با JWT
using AutoMapper;
using System.Text;
using WebApplication1;
using MiniValidation;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using FluentValidation;
using Microsoft.OpenApi.Models; // 👈 اضافه شد
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;


internal class Program
{
    private static void Main(string[] args)
    {
        // 🏁 Builder
        var builder = WebApplication.CreateBuilder(args);
    https://localhost:5001/swagger

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddValidatorsFromAssemblyContaining<BookDtoValdier>();

        // Swagger
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Books API", Version = "v1" });

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "توکن را به صورت: Bearer {token} وارد کنید",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            };
            c.AddSecurityDefinition("Bearer", securityScheme);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { securityScheme, new string[] { } }
            });
        });

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddAutoMapper(typeof(Program));

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
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
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
            };
        });

        // 🏗️ Build
        var app = builder.Build();
        // بعد از ساخت اپ (قبل از app.Run)
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // اگر دیتابیس ساخته نشده باشه
            db.Database.EnsureCreated();

            // اگر هیچ کاربری وجود نداره، یکی اضافه کن
            if (!db.Users.Any())
            {
                db.Users.Add(new User
                {
                    UserName = "admin",
                    Password = "1234", // ❗ در دنیای واقعی باید هش کنی
                    Role = "Admin"
                });

                db.Users.Add(new User
                {
                    UserName = "user1",
                    Password = "1234",
                    Role = "User"
                });

                db.SaveChanges();
            }
        }

        // 🔌 Middleware ها
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        // Root endpoint
        app.MapGet("/", () => Results.Ok(new
        {
            Message = "Books API is running 🚀",
            Version = "v1"
        }));

        // Middleware خطای عمومی
        app.Use(async (context, next) =>
        {
            try
            {
                await next();
            }
            catch (Exception)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                var errorResponse = new
                {
                    Message = "خطای غیرمنتظره‌ای رخ داد.",
                    ErrorId = Guid.NewGuid()
                };
                await context.Response.WriteAsJsonAsync(errorResponse);
            }
        });

        // 🚏 Endpoints

        // گرفتن همه کتاب‌ها
        app.MapGet("/books", async (AppDbContext db, IMapper mapper) =>
        {
            var books = await db.Books.ToListAsync();
            var result = mapper.Map<List<BookResultDto>>(books);
            return Results.Ok(result);
        });

        // گرفتن یک کتاب با id
        app.MapGet("/books/{id}", async (int id, AppDbContext db, IMapper mapper) =>
        {
            var book = await db.Books.FindAsync(id);
            if (book is null) return Results.NotFound(new { message = "کتاب پیدا نشد" });
            return Results.Ok(mapper.Map<BookResultDto>(book));
        });

        // صفحه‌بندی
        app.MapGet("/books/page", async (int pageNumber, int pageSize, AppDbContext db, IMapper mapper) =>
        {
            var books = await db.Books
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Results.Ok(mapper.Map<List<BookResultDto>>(books));
        });

        // جستجو
        app.MapGet("/books/search", async (string? title, int? year, AppDbContext db, IMapper mapper) =>
        {
            var query = db.Books.AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
                query = query.Where(b => b.Title.Contains(title));

            if (year.HasValue)
                query = query.Where(b => b.Year == year.Value);

            var list = await query.ToListAsync();
            if (list.Count == 0)
                return Results.NotFound(new { message = "کتابی یافت نشد" });

            return Results.Ok(mapper.Map<List<BookResultDto>>(list));
        });

        // login
        app.MapPost("/login", async (UserLoginDto login, AppDbContext db) =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.UserName == login.UserName);

            if (user is null || user.Password != login.Password) // ❗ رمز عبور واقعی باید هش باشه
                return Results.Unauthorized();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role) // 🔑 نقش مهمه برای DELETE
        }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = builder.Configuration["Jwt:Issuer"],
                Audience = builder.Configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            return Results.Ok(new { token = jwt });
        });

        // اضافه کردن کتاب (یوزر معمولی)
        app.MapPost("/books", async (BookDto dto, AppDbContext db, IMapper mapper) =>
        {
            if (!MiniValidator.TryValidate(dto, out var errors))
                return Results.ValidationProblem(errors);

            var book = mapper.Map<Book>(dto);
            db.Books.Add(book);
            await db.SaveChangesAsync();

            return Results.Created($"/books/{book.Id}", new { message = "کتاب با موفقیت اضافه شد", book.Id });
        }).RequireAuthorization();

        // اضافه کردن کتاب (فقط Admin)
        app.MapPost("/admin/books", async (BookDto dto, AppDbContext db, IMapper mapper) =>
        {
            var book = mapper.Map<Book>(dto);
            db.Books.Add(book);
            await db.SaveChangesAsync();
            return Results.Created($"/books/{book.Id}", mapper.Map<BookResultDto>(book));
        }).RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });

        // ویرایش کتاب
        app.MapPut("/books/{id}", async (int id, BookDto dto, AppDbContext db, IMapper mapper) =>
        {
            var book = await db.Books.FindAsync(id);
            if (book is null) return Results.NotFound(new { message = "کتاب پیدا نشد" });

            mapper.Map(dto, book);
            await db.SaveChangesAsync();

            return Results.Ok(new { message = "کتاب ویرایش شد" });
        }).RequireAuthorization();

        // حذف کتاب (فقط Admin)
        app.MapDelete("/books/{id}", async (int id, AppDbContext db) =>
        {
            var book = await db.Books.FindAsync(id);
            if (book is null) return Results.NotFound(new { message = "کتاب پیدا نشد" });

            db.Books.Remove(book);
            await db.SaveChangesAsync();
            return Results.Ok(new { message = "کتاب حذف شد" });
        }).RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });

        // کنترلر سنتی
        app.MapControllers();

        // 🏁 Run
        app.Run();
    }
}
