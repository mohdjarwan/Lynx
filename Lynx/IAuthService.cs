using Lynx.Core.Entities;

namespace Lynx
{
    public interface IAuthService
    {
        public string GenerateToken(User user);

    }
}