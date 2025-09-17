using Moq;
using WebApplication1.Models;
using WebApplication1.Repositories;
using WebApplication1.Services;
using AutoMapper;
using Xunit;

public class BookServiceUnitTests
{
    private readonly IMapper _mapper;

    public BookServiceUnitTests()
    {
        // AutoMapper Configuration
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Book, WebApplication1.DTOs.BookResultDto>();
            cfg.CreateMap<WebApplication1.DTOs.BookDto, Book>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task GetBookByIdAsync_ReturnsBook_WhenExists()
    {
        // Arrange
        var mockBookRepo = new Mock<IBookRepository>();
        var mockUserRepo = new Mock<IUserRepository>();
        var mockCache = new Mock<ICacheService>();

        mockBookRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Book { Id = 1, Title = "Test", Author = "Author", Year = 2025 });

        var service = new BookService(mockBookRepo.Object, mockUserRepo.Object, mockCache.Object, _mapper);

        // Act
        var book = await service.GetBookByIdAsync(1);

        // Assert
        Assert.NotNull(book);
        Assert.Equal("Test", book!.Title);
    }

    [Fact]
    public async Task GetBookByIdAsync_ReturnsNull_WhenNotExists()
    {
        // Arrange
        var mockBookRepo = new Mock<IBookRepository>();
        var mockUserRepo = new Mock<IUserRepository>();
        var mockCache = new Mock<ICacheService>();

        mockBookRepo.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Book?)null);

        var service = new BookService(mockBookRepo.Object, mockUserRepo.Object, mockCache.Object, _mapper);

        // Act
        var book = await service.GetBookByIdAsync(999);

        // Assert
        Assert.Null(book);
    }

    [Fact]
    public async Task AddBookWithRulesAsync_Should_AddBook()
    {
        // Arrange
        var mockBookRepo = new Mock<IBookRepository>();
        var mockUserRepo = new Mock<IUserRepository>();
        var mockCache = new Mock<ICacheService>();

        var newBook = new Book { Title = "New Book", Author = "Author", Year = 2025 };
        mockBookRepo.Setup(r => r.AddAsync(newBook)).ReturnsAsync(newBook);

        var service = new BookService(mockBookRepo.Object, mockUserRepo.Object, mockCache.Object, _mapper);

        var user = new User { Id = 1, UserName = "testuser", Role = "User" };

        // Act
        var addedBook = await service.AddBookWithRulesAsync(newBook, user);

        // Assert
        Assert.Equal("New Book", addedBook.Title);
        mockBookRepo.Verify(r => r.AddAsync(newBook), Times.Once);
    }
}
