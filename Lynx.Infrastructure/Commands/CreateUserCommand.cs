using FluentValidation;
namespace Lynx.Infrastructure.Commands
{
    public class CreateUserCommand
    {
        public string? username { get; set; }

        public string? email { get; set; }

        public string? password { get; set; }

        public string? firstname { get; set; }

        public string? lastname { get; set; }

        public int tenantid { get; set; }

        public bool IsDeleted{ get; set; }

        public string? CreatedBy { get; set; }

        public string? LastModifiedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime LastModifiedDate { get; set; } = DateTime.Now;

    }
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(c => c.email).NotNull();

            RuleFor(c => c.username).NotEmpty();
        }
    }
}
