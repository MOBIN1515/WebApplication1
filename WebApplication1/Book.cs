using System.ComponentModel.DataAnnotations;

public class Book
{
    public int Id { get; set; }

    [Required(ErrorMessage = "عنوان کتاب الزامی است")]
    [MaxLength(100, ErrorMessage = "عنوان نباید بیش از ۱۰۰ کاراکتر باشد")]
    public string Title { get; set; }

    [Required(ErrorMessage = "نام نویسنده الزامی است")]
    [MaxLength(50, ErrorMessage = "نام نویسنده نباید بیش از ۵۰ کاراکتر باشد")]
    public string Author { get; set; }

    [Range(1, 3000, ErrorMessage = "سال باید بین ۱ تا ۳۰۰۰ باشد")]
    public int Year { get; set; }
}


