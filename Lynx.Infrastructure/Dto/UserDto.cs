using FluentValidation;
using Lynx.Infrastructure.Commands;

namespace Lynx.Infrastructure.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public string? UserName { get; set; }

        public string? Email { get; set; }

    }
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        public UserDtoValidator()
        {
            RuleFor(c => c.Email).NotNull();

            RuleFor(c => c.UserName).NotEmpty();
        }
    }

}