using Lynx.Core.Entities;
using Lynx.Infrastructure.Commands;
using Lynx.Infrastructure.Data;
using Lynx.Infrastructure.Dto;
using Lynx.Infrastructure.Mappers;
using Lynx.Infrastructure.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;
    public UserController(IUnitOfWork unitOfWork, IUserMapper mapper, IPasswordHasher passwordHasher,
        IConfiguration configuration, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _userManager = userManager;
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

    /*[HttpPost]
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
    }*/
    [HttpPost("test")]
    public IActionResult ValidateToken([FromBody] TokenDto tokenDto)
    {
        ClaimsPrincipal? principal;
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["JWT:secret"]!);

        try
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                // Validate the token's signature using the provided key
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),

                // Validate the token's issuer (the entity that issued the token)
                ValidateIssuer = true,
                ValidIssuer = _configuration["JWT:Issuer"],

                // Validate the token's audience (the intended recipient of the token)
                ValidateAudience = true,
                ValidAudience = _configuration["JWT:audience"],

                // Validate the token's lifetime (expiration and not before)
                ValidateLifetime = true,

                // Specify the time span for which the token expiration can be considered valid,
                // accounting for potential clock skew between the server and client
                ClockSkew = TimeSpan.FromMinutes(5),

                // Optional: Validate the token's subject (the identity the token is issued to)
                ValidateActor = false,


                // Optional: Validate the token's audience
                RequireAudience = true,
                RequireExpirationTime = true
            };

            // Validate the token
            principal = tokenHandler.ValidateToken(tokenDto.Token, tokenValidationParameters, out SecurityToken validatedToken);

            // Return true if the token is valid
            if (validatedToken != null)
            {
                return Ok(new { IsValid = true, Claims = principal?.Claims.Select(c => new { c.Type, c.Value }) });
            }
        }
        catch (Exception ex)
        {
            // Log the exception (if needed)
            return BadRequest(new { IsValid = false, Error = ex.Message });
        }

        // If token validation fails
        return BadRequest(new { IsValid = false });
    }



    [HttpPost("Register")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<User>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Register(RegisterDto userDto, CancellationToken cancellationToken)
    {
        ApplicationUser user = new ApplicationUser();
        user.UserName = userDto.UserName;
        user.Email = userDto.Email;
        IdentityResult result = await _userManager.CreateAsync(user, userDto.Password);
        if (result.Succeeded)
        {
            return Ok("Account Add Success");
        }
        return BadRequest(result.Errors.FirstOrDefault());
    }


    [HttpPost("Login")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<User>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Login(LoginDto loginDto, CancellationToken cancellationToken)
    {
        ApplicationUser user = (await _userManager.FindByNameAsync(loginDto.UserName!))!;
        if (user != null) 
        { 
            bool found = await _userManager.CheckPasswordAsync(user,loginDto.Password!);  
            if (found) {
                
                //Claims Token
                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, user.UserName!));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id!));
                claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

                //get role
                var roles = await _userManager.GetRolesAsync(user);
                foreach(var itemRole in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, itemRole));
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:secret"]!));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                //Created token
                var MyToken = new JwtSecurityToken(
                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: creds
                );
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(MyToken),
                    expiration = MyToken.ValidTo
                });
            }
        }
        return BadRequest();

    }
    //private string CreateToken(User user)
    //{
    //    var claims = new List<Claim>();
    //    claims.Add(new Claim(ClaimTypes.Name, user.UserName!));
    //    claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

    //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:secret"]!));
    //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    //    //Created token
    //    var MyToken = new JwtSecurityToken(
    //        issuer: _configuration["JWT:Issuer"],
    //        audience: _configuration["JWT:audience"],
    //        claims: claims,
    //        expires: DateTime.Now.AddHours(1),
    //        signingCredentials: creds
    //    );

    //    return new JwtSecurityTokenHandler().WriteToken(MyToken);
    //}

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