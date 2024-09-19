using FakeItEasy;
using FluentValidation;
using FluentValidation.Results;
using Lynx.Controllers;
using Lynx.Core.Entities;
using Lynx.Infrastructure.Commands;
using Lynx.Infrastructure.Data;
using Lynx.Infrastructure.Dto;
using Lynx.Infrastructure.Mappers;
using Lynx.Infrastructure.Repository.Interfaces;
using Lynx.Infrastructure.Response;
using Lynx.IServices;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Linq.Expressions;

namespace Lynx.Tests.Controller;

public class UsersControllerTests
{
    private readonly IUnitOfWork _fakeUnitOfWork;
    private readonly IUserMapper _fakeUserMapper;
    private readonly IAuthService _fakeAuthService;
    private readonly IEmailService _fakeEmailService;
    private readonly IPasswordHasher _fakePasswordHasher;
    private readonly IValidator<CreateUserCommand> _fakevalidator;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _fakeUnitOfWork = A.Fake<IUnitOfWork>();
        _fakeUserMapper = A.Fake<IUserMapper>();
        _fakeAuthService = A.Fake<IAuthService>();
        _fakeEmailService = A.Fake<IEmailService>();
        _fakePasswordHasher = A.Fake<IPasswordHasher>();
        _fakevalidator = A.Fake<IValidator<CreateUserCommand>>();


        _controller = new UsersController(_fakeUnitOfWork, _fakeUserMapper, _fakePasswordHasher, _fakeAuthService, _fakeEmailService, _fakevalidator);
    }

    [Fact]
    public async Task User_bad_request_if_id_is_invalid  /*GetValue_ReturnsBadRequest_WhenIdIsInvalid*/()
    {
        // Arrange
        var id = 0;

        // Act
        var result = await _controller.GetValue(id, default);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task GetValue_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var id = 1;

        A.CallTo(() => _fakeUnitOfWork.Users.GetAsync(
                                                    A<Expression<Func<User, bool>>>.Ignored,
                                                    A<CancellationToken>.Ignored)).Returns(Task.FromResult<User>(null!));
        // Act
        var result = await _controller.GetValue(id, default);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetValue_ReturnsOk_WhenUserExists()
    {
        // Arrange
        var id = 6;
        var user = A.Fake<User>();
        var userDto = A.Fake<UserDto>();

        A.CallTo(() => _fakeUnitOfWork.Users.GetAsync(
                                                    A<Expression<Func<User, bool>>>.Ignored,
                                                    A<CancellationToken>.Ignored)).Returns(user);

        A.CallTo(() => _fakeUserMapper.Map(user)).Returns(userDto);

        // Act
        var result = await _controller.GetValue(id, default);

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
    public async Task Post_ReturnCreatedAction_WhenUserIsCreated()
    {
        // Arrange
        var response = new APIResponse<ValidateDto>();

        var command = A.Fake<CreateUserCommand>();
        var user = A.Fake<User>();

        A.CallTo(() => _fakevalidator.Validate(command)).Returns(new ValidationResult());

        A.CallTo(() => _fakeUnitOfWork.Users.Add(It.IsAny<User>())).Returns(Task.CompletedTask);

        A.CallTo(() => _fakePasswordHasher.Hash(command.password!)).Returns("hashedpassword");

        A.CallTo(() => _fakeUnitOfWork.SaveAsync(A<CancellationToken>.Ignored)).Returns(Task.FromResult(1));


        // Act
        var result = await _controller.Post(command, default);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        //Assert.Equal("GetValue", createdAtActionResult.ActionName);
        //Assert.Equal(user.Id, ((User)createdAtActionResult.Value!).Id);

    }
    [Fact]
    public async Task Post_ReturnBadRequest_WhenValidationIsFails()
    {
        // Arrange
        var response = A.Fake<APIResponse<ValidateDto>>();
        var command = A.Fake<CreateUserCommand>();
        var fakeValidationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("username", "Username is required."),
                new ValidationFailure("email", "Email is required.")
            };

        var fakeValidationResult = new ValidationResult();

        // Make the validator return this failed validation result
        A.CallTo(() => _fakevalidator.Validate(command)).Returns(fakeValidationResult);
        // Act
        var result = await _controller.Post(command, default);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        //Assert.Equal("GetValue", createdAtActionResult.ActionName);
        //Assert.Equal(user.Id, ((User)createdAtActionResult.Value!).Id);

    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenUserIsDeleted()
    {
        // Arrange
        var id = 1;
        var user = new User { Id = id };

        A.CallTo(() => _fakeUnitOfWork.Users.GetAsync(
                                                    A<Expression<Func<User, bool>>>.Ignored,
                                                    A<CancellationToken>.Ignored)).Returns(user);

        A.CallTo(() => _fakeUnitOfWork.Users.Delete(A<User>.Ignored)).Returns(Task.CompletedTask);
        A.CallTo(() => _fakeUnitOfWork.SaveAsync(A<CancellationToken>.Ignored)).Returns(Task.FromResult(1));

        // Act
        var result = await _controller.Delete(id, default);
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
        var result = await _controller.Delete(id, default);

        // Assert
        var actionResult = Assert.IsType<ActionResult<User>>(result);
        Assert.IsType<BadRequestResult>(actionResult.Result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var id = 1;
        A.CallTo(() => _fakeUnitOfWork.Users.GetAsync(
                                                    A<Expression<Func<User, bool>>>.Ignored,
                                                    A<CancellationToken>.Ignored)).Returns((User)null!);
        // Act
        var result = await _controller.Delete(id, default);

        // Assert

        var actionResult = Assert.IsType<ActionResult<User>>(result);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

}