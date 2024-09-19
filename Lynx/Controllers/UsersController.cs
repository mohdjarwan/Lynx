using Lynx.Core.Entities;
using Lynx.Infrastructure.Commands;
using Lynx.Infrastructure.Response;
using Lynx.Infrastructure.Data;
using Lynx.Infrastructure.Dto;
using Lynx.Infrastructure.Mappers;
using Lynx.Infrastructure.Repository.Interfaces;
using Lynx.IServices;
using Lynx.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Confluent.Kafka;
using System.Net;
using ServiceStack.Web;
using FluentValidation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Lynx.Controllers;
//[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IEmailService _email;
    private readonly IValidator<CreateUserCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    // private readonly IConfiguration _configuration;
    public UsersController(IUnitOfWork unitOfWork, IUserMapper mapper, IPasswordHasher passwordHasher,
    /* IConfiguration configuration,*/ IAuthService auth, IEmailService email, IValidator<CreateUserCommand> validator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        //_configuration = configuration;
        _authService = auth;
        _email = email;
        _validator = validator;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmail(CreateEmail create)
    {
        var email = create.email;
        var subject = create.subject;
        var message = create.message;

        await _email.SendEmailAsync(email!, subject!, message!);
        return Ok("Email sent successfully!");
    }

    // [Authorize]
    [HttpGet]
    [ProducesResponseType<User>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<User>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<IEnumerable<User>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var response = new APIResponse<string>();

        var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
        if (users is null)
        {
            return NotFound();
        }
        response.SetResponseInfo(HttpStatusCode.OK, null!, users, false);

        return Ok(new
        {
            Users = users.Select(_mapper.Map)
        });
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<User>(StatusCodes.Status200OK)]
    [ProducesResponseType<User>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<User>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetValue(int id, CancellationToken cancellationToken)
    {

        if (id <= 0)
        {
            return BadRequest();
        }
        var user = await _unitOfWork.Users.GetAsync(u => u.Id == id, cancellationToken);
        if (user is null)
        {
            var response = new APIResponse<string>();

            // response.SetResponseInfo(HttpStatusCode.BadRequest, new List<string> { "Something went wrong" }, null, false);
            return NotFound(response);
        }

        return Ok(new
        {
            User = _mapper.Map(user)
        });
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<User>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Post([FromBody] CreateUserCommand command, CancellationToken cancellationToken)
    {
        var response = new APIResponse<ValidateDto>();
        var result = _validator.Validate(command);

        if (!result.IsValid)
        {
            var errors = (result.Errors).Select(u => new ValidateDto
            {
                PropertyName = u.PropertyName,
                ErrorMessage = u.ErrorMessage
            }).ToList();
            response.SetResponseInfo(HttpStatusCode.BadRequest, errors, null!, false);

            return BadRequest(response);
        }

        var user = new User
        {
            UserName = command.username,
            Email = command.email,
            Password = _passwordHasher.Hash(command.password!),
            FirstName = command.firstname,
            LastName = command.lastname,
            TenantId = command.tenantid,
            IsDeleted = command.IsDeleted,
            CreatedBy = command.CreatedBy!,
            LastModifiedBy = command.LastModifiedBy!,
            CreatedDate = command.CreatedDate,
            LastModifiedDate = command.LastModifiedDate

        };

        await _unitOfWork.Users.Add(user);
        await _unitOfWork.SaveAsync(cancellationToken);
        response.SetResponseInfo(HttpStatusCode.OK,null!, user, true);

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<User>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Login(LoginDto loginDto, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetAsync(u => u.UserName == loginDto.UserName, cancellationToken);
        var MyToken = _authService.GenerateToken(user);
        return Ok(MyToken);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<User>> Delete(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return BadRequest();
        }
        var user = await _unitOfWork.Users.GetAsync(u => u.Id == id, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }
        await _unitOfWork.Users.Delete(user);
        await _unitOfWork.SaveAsync(cancellationToken);

        return NoContent();
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<User>> Update(int id, [FromBody] UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetAsync(u => u.Id == id, cancellationToken);
        user.Email = command.email;
        user.CreatedBy = command.createdby;
        user.CreatedDate = command.createdDate;
        await _unitOfWork.SaveAsync(cancellationToken);

        return NoContent();
    }

}