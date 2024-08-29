using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Lynx.Infrastructure.Data;

public class ApplicationUser : IdentityUser
{
    [Required, MaxLength(128)]
    public string? firstname { get; set; }
    [Required, MaxLength(128)]
    public string? lastname { get; set; }

}
