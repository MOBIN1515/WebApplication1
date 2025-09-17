using WebApplication1.Models;
using WebApplication1.Repositories;
using WebApplication1.DTOs;
using AutoMapper;

namespace WebApplication1.Services;

public class BookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;

    public BookService(IBookRepository bookRepository, IUserRepository userRepository, ICacheService cacheService, IMapper mapper)
    {
        _bookRepository = bookRepository;
        _userRepository = userRepository;
        _cacheService = cacheService;
        _mapper = mapper;
    }

    public async Task<User?> GetUserByUserNameAsync(string userName)
    {
        return await _userRepository.GetByUserNameAsync(userName);
    }

    public async Task<BookResultDto> AddBookWithRulesAsync(Book book, User user)
    {
        if (user == null)
            throw new UnauthorizedAccessException("کاربر معتبر نیست");

        var addedBook = await _bookRepository.AddAsync(book);
        return _mapper.Map<BookResultDto>(addedBook);
    }

    public async Task<BookResultDto?> GetBookByIdAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        return book == null ? null : _mapper.Map<BookResultDto>(book);
    }

    public async Task UpdateBookAsync(Book book)
    {
        await _bookRepository.UpdateAsync(book);
    }

    public async Task DeleteBookAsync(int id)
    {
        await _bookRepository.DeleteAsync(id);
    }

    public async Task<bool> SoftDeleteBookAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null || book.IsDelete)
            return false;

        book.IsDelete = true;
        await _bookRepository.UpdateAsync(book);
        return true;
    }

    public async Task<List<BookResultDto>> GetAllBooksAsync(bool includeDeleted = false)
    {
        string cacheKey = includeDeleted ? "AllBooksIncludingDeleted" : "AllBooks";
        var cached = _cacheService.Get<List<BookResultDto>>(cacheKey);
        if (cached != null)
            return cached;

        var books = await _bookRepository.GetAllAsync();
        var result = _mapper.Map<List<BookResultDto>>(books);

        _cacheService.Set(cacheKey, result, TimeSpan.FromMinutes(5));
        return result;
    }

    public async Task<List<BookResultDto>> GetBooksPagedAsync(int pageNumber, int pageSize, string? title = null, string? author = null)
    {
        var books = await _bookRepository.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(title))
            books = books.Where(b => b.Title.Contains(title)).ToList();

        if (!string.IsNullOrWhiteSpace(author))
            books = books.Where(b => b.Author.Contains(author)).ToList();

        var paged = books.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return _mapper.Map<List<BookResultDto>>(paged);
    }
}
