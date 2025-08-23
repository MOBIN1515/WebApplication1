using FluentValidation;
using System.Data;


namespace WebApplication1;
public class BookDtoValdier: AbstractValidator<BookDto>
{

public BookDtoValdier()
    {
        RuleFor(b => b.Title)
          .NotEmpty().WithMessage("عنوان الزامی است")
          .MaximumLength(100);

        RuleFor(b => b.Year)
       .InclusiveBetween(1500, 2100).WithMessage("سال باید بین 1500 تا 2100 باشد");

    }




}
