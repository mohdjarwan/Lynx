﻿using Lynx.Infrastructure.Data;
using Lynx.Infrastructure.Repository.Interfaces;

namespace Lynx.Infrastructure.Repository;

public class UnitOfWork(ApplicationDbContext db) : IUnitOfWork,IDisposable
{
    private readonly ApplicationDbContext _db = db;
    public IUserRepository Users { get; set; } = new UserRepository(db);
    public ITenantRepository Tenants { get; set; } = new TenantRepository(db);

    public void Dispose()
    {
        _db.Dispose();
    }


    public async Task<int> SaveAsync(CancellationToken cancellationToken)
    {
        return await _db.SaveChangesAsync(cancellationToken);
    }
}