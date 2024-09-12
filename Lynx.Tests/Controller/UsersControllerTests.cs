using FakeItEasy;
using Lynx.Controllers;
using Lynx.Core.Entities;
using Lynx.Infrastructure.Data;
using Lynx.Infrastructure.Dto;
using Lynx.Infrastructure.Mappers;
using Lynx.Infrastructure.Repository.Interfaces;
using Lynx.IServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Lynx.Tests.Controller;

public class UsersControllerTests
{
    private IAuthService _authService;
    private IEmailService _email;
    private IUnitOfWork _unitOfWork;
    private IUserMapper _mapper;
    private IPasswordHasher _passwordHasher;
    public UsersControllerTests()
    {
        _authService = A.Fake<IAuthService>();
        _email = A.Fake<IEmailService>();
        _unitOfWork = A.Fake<IUnitOfWork>();
        _mapper = A.Fake<IUserMapper>();
        _passwordHasher = A.Fake<IPasswordHasher>();
    }

    [Fact]
    public void UsersController_GetUsers_ReturnOK()
    {
        var users = A.Fake<ICollection<User>>(); // Fake collection of Users
        var userList = A.Fake<List<User>>(); // Fake collection of Users
        var userListDto = new List<UserDto>
        {
            new UserDto { Id = 1, UserName = "TestUser1", Email = "test1@example.com" },
            new UserDto { Id = 2, UserName = "TestUser2", Email = "test2@example.com" }
        };
        A.CallTo(() => _mapper.Map<List<UserDto>>(users)).Returns(userList);
        var controller = new UsersController(_unitOfWork, _mapper, _passwordHasher, _authService, _email);

        //Act
        var result = controller.Get(); // Assuming it returns some form of IActionResult

        //Assert
        //Assert.IsType<OkObjectResult>(result); // Check if the result is OkObjectResult
        //var okResult = result as OkObjectResult;
        //var returnedUsers = okResult?.Value as IEnumerable<UserDto>;

        //Assert.NotNull(returnedUsers);
        //Assert.Equal(2, returnedUsers.Count()); // Check if two users are returned

    }
}