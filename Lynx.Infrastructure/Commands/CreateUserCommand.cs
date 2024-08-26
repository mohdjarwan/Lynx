using FluentValidation;
using Lynx.Infrastructure.Data;
namespace Lynx.Infrastructure.Commands
{
    public class CreateUserCommand
    {
        public string? name { get; set; }
        public string? email { get; set; }
        public string? password { get; set; }
        public string? firstname { get; set; }
        public string? lastname { get; set; }
        public int tenantid { get; set; }
    }
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(c => c.email).NotNull();

            RuleFor(c => c.name).NotEmpty();
        }
    }
}

