using Lynx.Core.Entities;

namespace Lynx.IServices;

public interface IAuthService
{
    public string GenerateToken(User user);

}