using FluentValidation;
using WebApplication1.DTOs;

namespace WebApplication1.Validators
{
    public class UserNameLoginDtoValidator : AbstractValidator<UserLoginDto>
    {
        public UserNameLoginDtoValidator()
        {
            RuleFor(u => u.UserName)
                .NotEmpty().WithMessage("نام کاربری اجباری است")
                .MinimumLength(3).WithMessage("نام کاربری باید حداقل 3 کاراکتر باشد")
                .MaximumLength(50).WithMessage("نام کاربری نمی‌تواند بیشتر از 50 کاراکتر باشد");
        }
    }
}
