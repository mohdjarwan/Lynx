using FakeItEasy;
using Lynx.Controllers;
using Lynx.Core.Entities;
using Lynx.Infrastructure.Data;
using Lynx.Infrastructure.Dto;
using Lynx.Infrastructure.Mappers;
using Lynx.Infrastructure.Repository.Interfaces;
using Lynx.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace Lynx.Tests.Controller;

public class UsersControllerTests
{
    private readonly IUnitOfWork _fakeUnitOfWork;
    private readonly IUserMapper _fakeUserMapper;
    private readonly IAuthService _fakeAuthService;
    private readonly IEmailService _fakeEmailService;
    private readonly IPasswordHasher _fakePasswordHasher;
    private readonly UsersController _controller;
    CancellationToken cancellationToken = new CancellationToken();

    public UsersControllerTests()
    {
        _fakeUnitOfWork = A.Fake<IUnitOfWork>();
        _fakeUserMapper = A.Fake<IUserMapper>();
        _fakeAuthService = A.Fake<IAuthService>();
        _fakeEmailService = A.Fake<IEmailService>();
        _fakePasswordHasher = A.Fake<IPasswordHasher>();

        _controller = new UsersController(_fakeUnitOfWork, _fakeUserMapper, _fakePasswordHasher, _fakeAuthService, _fakeEmailService);
    }

    [Fact]
    public async Task GetValue_ReturnsBadRequest_WhenIdIsInvalid()
    {
        // Arrange
        var id = 0;

        // Act
        var result = await _controller.GetValue(id, cancellationToken);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task GetValue_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var id = 1;


        A.CallTo(() => _fakeUnitOfWork.Users.GetAsync(
            A<Expression<Func<User, bool>>>.That.Matches(u => u.Compile()(new User { Id = id })),
            cancellationToken))
            .Returns(Task.FromResult<User>(null!));


        // Act
        var result = await _controller.GetValue(id, cancellationToken);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetValue_ReturnsOk_WhenUserExists()
    {
        // Arrange
        var id = 1;
        var user = new User { Id = id };
        var userDto = new UserDto { Id = id };


        A.CallTo(() => _fakeUnitOfWork.Users.GetAsync(
            A<Expression<Func<User, bool>>>.That.Matches(u => u.Compile()(new User { Id = id })),
            cancellationToken))
        .Returns(Task.FromResult(user));
        A.CallTo(() => _fakeUserMapper.Map(user));

        // Act
        var result = await _controller.GetValue(id, cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var resultObject = okResult.Value;
        var userProperty = resultObject!.GetType().GetProperty("User");
        var returnedUser = (UserDto)userProperty!.GetValue(resultObject)!;

       Assert.Equal(userDto.Id, returnedUser.Id);
       // Assert.Equal(userDto.UserName, returnedUser.UserName);
       // Assert.Equal(userDto.Email, returnedUser.Email);
    }

}