using Lynx.Core.Entities;
using Lynx.Infrastructure.Dto;

namespace Lynx.Infrastructure.Mappers;

public interface IUserMapper
{
    UserDto Map(User user);
}
public class UserMapper:IUserMapper
{
    public UserDto Map(User user)
    {
        return new()
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            //Password = user.Password
        };
        
    }
}