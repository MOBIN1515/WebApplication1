using WebApplication1.Models;
using WebApplication1.Repositories;
using WebApplication1.DTOs;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace WebApplication1.Services;

public class BookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    public BookService(IBookRepository bookRepository, IUserRepository userRepository, ICacheService cacheService, IMapper mapper, ILogger logger)
    {
        _bookRepository = bookRepository;
        _userRepository = userRepository;
        _cacheService = cacheService;
        _mapper = mapper;
        _logger = logger;
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

    public async Task<BookDto?> GetBookByIdAsync(int id)
    {
        try
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
                return null;

            return _mapper.Map<BookDto>(book);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در گرفتن کتاب با Id: {Id}", id);
            throw;
        }
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

    public async Task<List<BookDto>> GetBooksPagedAsync(int pageNumber, int pageSize, string? titleFilter, string? authorFilter)
    {
        var query = _bookRepository.Query();

        if (!string.IsNullOrEmpty(titleFilter))
            query = query.Where(b => b.Title.Contains(titleFilter));

        if (!string.IsNullOrEmpty(authorFilter))
            query = query.Where(b => b.Author.Contains(authorFilter));

        var result = await query
            .AsNoTracking()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<BookDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return result;
    }



}
