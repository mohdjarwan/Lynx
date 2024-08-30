using ServiceStack.DataAnnotations;

namespace Lynx.Infrastructure.Dto;

public class RegisterDto
{


    [Unique]
    [Required]
    public string? UserName { get; set; }

    public string? Email { get; set; }

    [Unique]
    [Required]
    public string? Password { get; set; }

}