using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Lynx.Infrastructure.Commands;

public class CreateEmail
{
    public string? email { get; set; }
    public string? subject { get; set; }
    public string? message { get; set; }
}
public class CreateEmailValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateEmailValidator()
    {
        RuleFor(c => c.name).NotNull();

    }
}