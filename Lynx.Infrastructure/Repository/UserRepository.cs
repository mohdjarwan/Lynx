using Lynx.Core.Entities;
using Lynx.Infrastructure.Data;
using Lynx.Infrastructure.Repository.Interfaces;

namespace Lynx.Infrastructure.Repository;

public class UserRepository :Repository<User>,IUserRepository
{
    private readonly ApplicationDbContext _db;

    public UserRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }
    public void Update(User user)
    {
        _db.Users.Update(user);
    }
}