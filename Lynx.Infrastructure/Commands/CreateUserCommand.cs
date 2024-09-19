using FluentValidation;
namespace Lynx.Infrastructure.Commands
{
    public class CreateUserCommand
    {
        //public int id { get; set; }
        public string? username { get; set; }

        public string? email { get; set; }

        public string? password { get; set; }

        public string? firstname { get; set; }

        public string? lastname { get; set; }

        public int tenantid { get; set; }

        public bool IsDeleted { get; set; }

        public string? CreatedBy { get; set; }

        public string? LastModifiedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime LastModifiedDate { get; set; } = DateTime.Now;

    }
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(u => u.username)
                .NotEmpty().WithMessage("UserName should not be null")
                .Matches("^[a-zA-Z0-9]*$").WithMessage("Username can only contain letters and numbers.");

            RuleFor(u => u.email)
                .NotEmpty().WithMessage("Email should not be null")
                .EmailAddress();

            RuleFor(u => u.password)
                .NotEmpty().WithMessage("Password should not be null")
                .Matches(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$")
                .WithMessage("Password must be at least 8 characters long and contain letters, numbers, and a special character.");

            RuleFor(u => u.firstname)
                .NotEmpty().WithMessage("firstname should not be null")
                .Matches("^[a-zA-Z]*$").WithMessage("firstname can only contain letters and numbers.");

            RuleFor(u => u.lastname)
                .NotEmpty().WithMessage("lastname should not be null")
                .Matches("^[a-zA-Z]*$").WithMessage("lastname can only contain letters and numbers.");

            RuleFor(u => u.tenantid)
                .NotEmpty().WithMessage("tenantid should not be null");

            RuleFor(u => u.IsDeleted)
                .NotEmpty().WithMessage("Email should not be null");

            RuleFor(u => u.CreatedBy)
                .NotEmpty().WithMessage("CreatedBy should not be null")
                .Matches("^[a-zA-Z0-9]*$").WithMessage("CreatedBy can only contain letters and numbers.");

            RuleFor(u => u.LastModifiedBy)
                .NotEmpty().WithMessage("LastModifiedBy should not be null")
                .Matches("^[a-zA-Z0-9]*$").WithMessage("LastModifiedBy can only contain letters and numbers.");

            RuleFor(u => u.CreatedDate)
                .NotEmpty().WithMessage("CreatedDate should not be null")
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage("CreatedDate cannot be in the future.");

            RuleFor(u => u.LastModifiedDate)
                .NotEmpty().WithMessage("LastModifiedDate should not be null")
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage("LastModifiedDate cannot be in the future.");

        }
    }
}