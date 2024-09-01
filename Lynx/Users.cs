namespace Lynx;

public record Users(Guid Id, string Name, string Email, string Password, string[] Roles);
