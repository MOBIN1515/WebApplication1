using System.ComponentModel.DataAnnotations;

public class Book
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Title { get; set; }

    [Required, MaxLength(50)]
    public string Author { get; set; }

    [Range(1, 3000)]
    public int Year { get; set; }

    // فیلد جدید برای حذف نرم
    public bool IsDelete { get; set; } = false;
    public int? AddedByUserId { get; set; }
}
