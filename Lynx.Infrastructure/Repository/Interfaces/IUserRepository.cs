using Lynx.Core.Entities;

namespace Lynx.Infrastructure.Repository.Interfaces;

public interface IUserRepository:IRepository<User>
{
    void Update(User user);

}