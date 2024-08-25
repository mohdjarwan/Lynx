using ServiceStack.DataAnnotations;

namespace Lynx.Core.Entities;

public class User
{
    public int Id { get; set; }

    [Unique]
    [Required]
    public string? UserName { get; set; }

    [Unique]
    [Required]
    public string? Email { get; set; }

    [Unique]
    [Required]
    public string? Password { get; set; }

    [Unique]
    [Required]
    public string? FirstName { get; set; }

    [Unique]
    [Required]
    public string? LastName { get; set; }

    [Unique]
    [Required]
    public int TenantId { get; set; }
}
