using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOs;

public class BookDto
{
    [Required, MaxLength(100)]
    public string Title { get; set; } = "";

    [Required, MaxLength(50)]
    public string Author { get; set; } = "";

    [Range(1, 3000)]
    public int Year { get; set; }
}

public class BookResultDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int Year { get; set; }

}