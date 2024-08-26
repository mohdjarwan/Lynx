using FluentValidation;
namespace Lynx.Infrastructure.Commands;

public class UpdateUserCommand
{
    public string? email {  get; set; }
}
public class UpdateTaskCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(c => c.email).NotNull();

    }
}