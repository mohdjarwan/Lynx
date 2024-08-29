using Lynx.Core.Entities;
using Lynx.Infrastructure.Commands;
using Lynx.Infrastructure.Data;
using Lynx.Infrastructure.Mappers;
using Lynx.Infrastructure.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Lynx.Controllers;


[ApiController]
//[Authorize]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;
    public UserController(IUnitOfWork unitOfWork, IUserMapper mapper, IPasswordHasher passwordHasher,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType<User>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<User>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<IEnumerable<User>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
        if (users is null)
        {
            return NotFound();
        }

        return Ok(new
        {
            Users = users.Select(_mapper.Map)
        });
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<User>(StatusCodes.Status200OK)]
    [ProducesResponseType<User>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<User>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetValue(int userId, CancellationToken cancellationToken)
    {
        if (userId <= 0)
        {
            return BadRequest();
        }
        var user = await _unitOfWork.Users.GetAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            return NotFound();
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
        var user = new User
        {
            UserName = command.name,
            Email = command.email,
            Password = _passwordHasher.Hash(command.password!),
            FirstName = command.firstname,
            LastName = command.lastname,
            TenantId = command.tenantid
        };

        await _unitOfWork.Users.Add(user);
        await _unitOfWork.SaveAsync(cancellationToken);

        return CreatedAtAction(nameof(GetValue), new
        {
            id = user.Id
        }, user);
    }

    [HttpPost("Login")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<User>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetAsync(u => u.Email == command.email, cancellationToken);
        if (user is null)
        {
            return Unauthorized("Invalid Email");
        }

        bool verified = _passwordHasher.Verify(command.password!, user.Password!);
        if (!verified)
        {
            return Unauthorized("Invalid Password");
        }

        string token = CreateToken(user);
        return Ok(new { Token = token });
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("userid", user.Id.ToString())
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(int.Parse(_configuration["Jwt:ExpiryInDays"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
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
        await _unitOfWork.SaveAsync(cancellationToken);

        return NoContent();
    }
}