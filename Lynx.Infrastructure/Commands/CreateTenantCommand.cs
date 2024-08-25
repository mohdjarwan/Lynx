using FluentValidation;

namespace Lynx.Infrastructure.Commands;

public class CreateTenantCommand
{
    public string? name { get; set; }
}
public class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantCommandValidator()
    {
        RuleFor(c => c.name).NotNull();

    }
}