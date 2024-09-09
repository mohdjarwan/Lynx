using FluentValidation;
namespace Lynx.Infrastructure.Commands;

public class UpdateUserCommand
{
    public string? email {  get; set; }
    public string? createdby {  get; set; }
    public DateTime createdDate {  get; set; }
}
public class UpdateTaskCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(c => c.email).NotNull();
        RuleFor(c => c.createdby).NotNull();
        RuleFor(c => c.createdDate).NotNull();

    }
}