using Lynx.Core.Entities;

namespace Lynx.IServices;

public interface IEmailService
{
     Task<User> GetUser(int id);
}