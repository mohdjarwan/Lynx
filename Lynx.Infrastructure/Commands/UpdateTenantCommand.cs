using FluentValidation;
namespace Lynx.Infrastructure.Commands;

public class UpdateTenantCommand
{
    public string? name { get; set; }
}
public class UpdateTenantCommandValidator : AbstractValidator<UpdateTenantCommand>
{
    public UpdateTenantCommandValidator()
    {
        RuleFor(c => c.name).NotNull();

    }
}