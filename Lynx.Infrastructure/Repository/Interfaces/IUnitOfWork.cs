namespace Lynx.Infrastructure.Repository.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ITenantRepository Tenants { get; }

    Task<int> SaveAsync(CancellationToken cancellationToken);
}