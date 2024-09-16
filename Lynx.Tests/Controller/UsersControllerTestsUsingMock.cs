using FakeItEasy;
using Lynx.Controllers;
using Lynx.Core.Entities;
using Lynx.Infrastructure.Commands;
using Lynx.Infrastructure.Data;
using Lynx.Infrastructure.Dto;
using Lynx.Infrastructure.Mappers;
using Lynx.Infrastructure.Repository.Interfaces;
using Lynx.IServices;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Linq.Expressions;

namespace Lynx.Tests.Controller;

public class UsersControllerTestsUsingMock
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IUserMapper> _mockUserMapper;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly UsersController _controller;
    CancellationToken cancellationToken = new CancellationToken();


    public UsersControllerTestsUsingMock()
    {
        _mockUnitOfWork     = new();
        _mockUserMapper     = new();
        _mockPasswordHasher = new();
        _mockAuthService    = new();
        _mockEmailService   = new();

        _controller = new UsersController(_mockUnitOfWork.Object,
                                          _mockUserMapper.Object,
                                          _mockPasswordHasher.Object,
                                          _mockAuthService.Object,
                                          _mockEmailService.Object);
    }

    [Fact]
    public async Task GetValue_ReturnsBadRequest_WhenIdIsInvalid()
    {
        // Arrange
        var invalidUserId = 0;

        // Act
        var result = await _controller.GetValue(invalidUserId, cancellationToken);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task GetValue_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var invalidUserId = 6;

        _mockUnitOfWork.Setup(uow => uow.Users.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), cancellationToken))!.ReturnsAsync((User)null!);

        // Act
        var result = await _controller.GetValue(invalidUserId, cancellationToken);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetValue_ReturnsOk_WhenUserExists()
    {
        // Arrange
        var userId = 6;
        var cancellationToken = new CancellationToken();

        var user = new User { Id = userId, UserName = "testuser", Email = "test@example.com" };
        var userDto = new UserDto { Id = userId, UserName = "testuser", Email = "test@example.com" };

        // Setup mock behavior for GetAsync and Map
        _mockUnitOfWork.Setup(uow => uow.Users.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), cancellationToken))
                       .ReturnsAsync(user);
        _mockUserMapper.Setup(mapper => mapper.Map(user)).Returns(userDto);

        // Act
        var result = await _controller.GetValue(userId, cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);

        var resultObject = okResult.Value;
        var userProperty = resultObject!.GetType().GetProperty("User"); 
        var returnedUser = (UserDto)userProperty!.GetValue(resultObject)!;

        Assert.Equal(userDto.Id, returnedUser.Id);
        Assert.Equal(userDto.UserName, returnedUser.UserName);
        Assert.Equal(userDto.Email, returnedUser.Email);
    }
    [Fact]
    public async Task Post_ShouldReturnCreatedAtAction_WhenUserIsCreated()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            username = "testuser",
            email = "testuser@example.com",
            password = "password123",
            firstname = "Test",
            lastname = "User",
            tenantid = 1,
            IsDeleted = false,
            CreatedBy = "admin",
            LastModifiedBy = "admin",
            CreatedDate = DateTime.UtcNow,
            LastModifiedDate = DateTime.UtcNow
        };

        var user = new User
        {
            UserName = command.username,
            Email = command.email,
            Password = "hashedpassword",  // The hashed password
            FirstName = command.firstname,
            LastName = command.lastname,
            TenantId = command.tenantid,
            IsDeleted = command.IsDeleted,
            CreatedBy = command.CreatedBy!,
            LastModifiedBy = command.LastModifiedBy!,
            CreatedDate = command.CreatedDate,
            LastModifiedDate = command.LastModifiedDate
        };

        _mockPasswordHasher.Setup(p => p.Hash(command.password!)).Returns("hashedpassword");
        _mockUnitOfWork.Setup(u => u.Users.Add(user)).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.SaveAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

        // Act
        var result = await _controller.Post(command, CancellationToken.None);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetValue), createdAtActionResult.ActionName);
       Assert.Equal(user.Id, ((User)createdAtActionResult.Value!).Id);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenUserIsDeleted()
    {
        // Arrange
        var id = 1;
        var user = new User { Id = id };
        _mockUnitOfWork.Setup(uow => uow.Users.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), cancellationToken))
                       .ReturnsAsync(user);

        _mockUnitOfWork.Setup(uow =>uow.Users.Delete(user)).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.SaveAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

        // Act
        var result = await _controller.Delete(id, CancellationToken.None);
        var actionResult = Assert.IsType<ActionResult<User>>(result);

        // Assert
        Assert.IsType<NoContentResult>(actionResult.Result);
    }
    [Fact]
    public async Task Delete_ReturnsBadRequest_WhenIdIsInvalid()
    {
        // Arrange
        var id = -1;

        // Act
        var result = await _controller.Delete(id, cancellationToken);

        // Assert
        var actionResult = Assert.IsType<ActionResult<User>>(result);
        Assert.IsType<BadRequestResult>(actionResult.Result);
    }


}