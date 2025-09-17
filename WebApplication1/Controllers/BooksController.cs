using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOs;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/books")]
[ApiVersion("1.0")]
public class BooksController : ControllerBase
{
    private readonly BookService _service;
    private readonly ILogger<BooksController> _logger;

    public BooksController(BookService service, ILogger<BooksController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var books = await _service.GetAllBooksAsync();
            return Ok(books);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطای دریافت همه کتاب‌ها");
            return StatusCode(500, new { message = "یک خطای داخلی رخ داد" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var book = await _service.GetBookByIdAsync(id);
            if (book == null)
                return NotFound(new { message = $"کتاب با شناسه {id} پیدا نشد" });

            return Ok(book);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطای دریافت کتاب");
            return StatusCode(500, new { message = "یک خطای داخلی رخ داد" });
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] BookDto dto)
    {
        try
        {
            var user = await _service.GetUserByUserNameAsync(User.Identity?.Name!);
            if (user == null)
                return Unauthorized(new { message = "کاربر معتبر نیست" });

            var book = await _service.AddBookWithRulesAsync(new Book
            {
                Title = dto.Title,
                Author = dto.Author,
                Year = dto.Year
            }, user);

            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطای ایجاد کتاب");
            return StatusCode(500, new { message = "یک خطای داخلی رخ داد" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] BookDto dto)
    {
        try
        {
            var book = await _service.GetBookByIdAsync(id);
            if (book == null)
                return NotFound(new { message = $"کتاب با شناسه {id} پیدا نشد" });

            book.Title = dto.Title;
            book.Author = dto.Author;
            book.Year = dto.Year;

            await _service.UpdateBookAsync(book);
            return Ok(book);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطای بروزرسانی کتاب");
            return StatusCode(500, new { message = "یک خطای داخلی رخ داد" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteBookAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطای حذف کتاب");
            return StatusCode(500, new { message = "یک خطای داخلی رخ داد" });
        }
    }

    [HttpGet("paged")]
    [Authorize(Policy = "UserOrAdmin")]
    public async Task<IActionResult> GetPagedBooks(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? title = null,
        [FromQuery] string? author = null)
    {
        try
        {
            var books = await _service.GetBooksPagedAsync(pageNumber, pageSize, title, author);
            return Ok(new { PageNumber = pageNumber, PageSize = pageSize, Books = books });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطای دریافت کتاب‌ها با صفحه‌بندی");
            return StatusCode(500, new { message = "یک خطای داخلی رخ داد" });
        }
    }

    [HttpDelete("{id}/soft")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> SoftDelete(int id)
    {
        try
        {
            var result = await _service.SoftDeleteBookAsync(id);
            if (!result)
                return NotFound(new { message = "کتاب پیدا نشد یا قبلا حذف شده" });

            return Ok(new { message = "کتاب با موفقیت حذف شد (Soft Delete)" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطای Soft Delete");
            return StatusCode(500, new { message = "یک خطای داخلی رخ داد" });
        }
    }
}
