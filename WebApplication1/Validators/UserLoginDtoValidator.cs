using FluentValidation;
using WebApplication1.DTOs;

namespace WebApplication1.Validators
{
    public class UserLoginDtoValidator : AbstractValidator<UserLoginDto>
    {
        public UserLoginDtoValidator()
        {
            RuleFor(u => u.UserName)
                .NotEmpty().WithMessage("نباید اسم خالی باشد")
                .MinimumLength(3).WithMessage(" حداقل 3 کاراکتر")
                .MaximumLength(50).WithMessage("حداکثر 50 کاراکتر");

            RuleFor(p => p.Password)
               .NotEmpty().WithMessage("نباید رمز خالی باشد")
               .MinimumLength(6).WithMessage(" حداقل 6 کاراکتر")
               .MaximumLength(100).WithMessage("حداکثر 100 کاراکتر");
        }




    }
}
