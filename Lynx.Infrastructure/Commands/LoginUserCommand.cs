using FluentValidation;
using Lynx.Infrastructure.Data;
namespace Lynx.Infrastructure.Commands
{
    public class LoginUserCommand
    {
        public string? email { get; set; }
        public string? password { get; set; }
   
    }
    public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(c => c.email).NotNull();
        }
    }
}

