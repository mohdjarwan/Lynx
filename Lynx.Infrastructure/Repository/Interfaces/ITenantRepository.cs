using Lynx.Core.Entities;

namespace Lynx.Infrastructure.Repository.Interfaces;

public interface ITenantRepository: IRepository<Tenant>
{
    void Update(Tenant tenant);

}
