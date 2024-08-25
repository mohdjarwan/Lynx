using Lynx.Infrastructure.Data;
using Lynx.Infrastructure.Repository.Interfaces;

namespace Lynx.Infrastructure.Repository;

public class UnitOfWork(ApplicationDbContext db, ITenantRepository tenants, IUserRepository users) : IUnitOfWork
{
    private readonly ApplicationDbContext _db = db;
    public IUserRepository Users { get; set; } = users;
    public ITenantRepository Tenants { get; set; } = tenants;

    public async Task<int> SaveAsync(CancellationToken cancellationToken)
    {
        return await _db.SaveChangesAsync(cancellationToken);
    }
}
