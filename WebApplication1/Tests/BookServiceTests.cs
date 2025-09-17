using Microsoft.EntityFrameworkCore;
using Moq;
using WebApplication1.Models;
using WebApplication1.Repositories;
using WebApplication1.Services;
using WebApplication1.AppDbContextEF;
using Xunit;

public class BookServiceTests
{
    [Fact]
    public async Task AddBook_Should_AddBookToDatabase()
    {
        // ساخت InMemory DbContext
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        await using var context = new AppDbContext(options);

        // ساخت Repository واقعی
        var bookRepository = new BookRepository(context);

        // Mock UserRepository (نیازی به رفتار خاص نیست)
        var userRepository = new Mock<IUserRepository>().Object;

        // ساخت BookService با Repositoryها
        var service = new BookService(bookRepository, userRepository);

        // کاربر نمونه
        var user = new User { Id = 1, UserName = "testuser", Role = "User" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // کتاب نمونه
        var book = new Book { Title = "Test Book", Author = "Author", Year = 2025 };

        // استفاده از متد AddBookWithRulesAsync
        await service.AddBookWithRulesAsync(book, user);

        // Assert
        Assert.Equal(1, context.Books.Count());
        Assert.Equal("Test Book", context.Books.First().Title);
    }
}
