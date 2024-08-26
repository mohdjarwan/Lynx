using Lynx.Core.Entities;
using Lynx.Infrastructure.Data;
using Lynx.Infrastructure.Repository.Interfaces;

namespace Lynx.Infrastructure.Repository;

public class TenantRepository:Repository<Tenant>,ITenantRepository
{
    private readonly ApplicationDbContext _db;

    public TenantRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }
    public void Update(Tenant tenant)
    {
        _db.Tenants.Update(tenant);
    }

}