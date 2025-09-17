using FluentValidation;
using WebApplication1.DTOs;

public class BookDtoValidator : AbstractValidator<BookDto>
{
    public BookDtoValidator()
    {
        RuleFor(b => b.Title)
            .NotEmpty().WithMessage("عنوان کتاب اجباری است")
            .MaximumLength(100).WithMessage("عنوان نمی‌تواند بیشتر از 100 کاراکتر باشد");

        RuleFor(b => b.Author)
            .NotEmpty().WithMessage("نام نویسنده اجباری است")
            .MaximumLength(50).WithMessage("نام نویسنده نمی‌تواند بیشتر از 50 کاراکتر باشد");

        RuleFor(b => b.Year)
            .InclusiveBetween(1000, 2025)
            .WithMessage("سال انتشار باید بین 1000 و 2025 باشد");
    }
}
