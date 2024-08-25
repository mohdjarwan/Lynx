namespace Lynx.Infrastructure.Repository.Interfaces;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    ITenantRepository Tenants { get; }
    Task<int> SaveAsync(CancellationToken cancellationToken);

}
