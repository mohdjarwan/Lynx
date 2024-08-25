using Lynx.Core.Entities;
using Lynx.Infrastructure.Dto;

namespace Lynx.Infrastructure.Mappers;

public interface ITenantMapper
{
    TenantDto Map(Tenant tenant);
}
public class TenantMapper : ITenantMapper
{
    public TenantDto Map(Tenant tenant)
    {
        return new()
        {
            Id = tenant.Id,
            Name = tenant.Name,
        };
        
    }
}
