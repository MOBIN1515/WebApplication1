using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Controller;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("2.0")]  // نسخه ۲
public class BooksV2Controller : ControllerBase
{
    private readonly BookService _service;

    public BooksV2Controller(BookService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetBooks()
    {
        var books = await _service.GetAllBooksAsync();

        // فقط برای نمونه، خروجی تغییر می‌کنیم
        var result = books.Select(b => new
        {
            b.Id,
            b.Title,
            PublishedYear = b.Year
        });

        return Ok(result);
    }
}
