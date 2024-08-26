using Lynx.Core.Entities;
using Lynx.Infrastructure.Commands;
using Lynx.Infrastructure.Mappers;
using Lynx.Infrastructure.Repository;
using Lynx.Infrastructure.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lynx.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TenantController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantMapper _mapper;

    public TenantController(IUnitOfWork unitOfWork, ITenantMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType<IEnumerable<User>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var Tenants = await _unitOfWork.Tenants.GetAllAsync(cancellationToken);

        if (Tenants is null)
        {
            return NotFound();
        }

            return Ok(new
            {
                Tenants = Tenants.Select(_mapper.Map)
            });
        }

        [HttpGet]
        [ProducesResponseType<User>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetValue(int tenantId, CancellationToken cancellationToken)
        {
            if (tenantId <= 0)
            {
                return BadRequest();
            }

            var tenant = await _unitOfWork.Tenants.GetAsync(u => u.Id == tenantId, cancellationToken);

            if (tenant is null)
            {
                return NotFound();
            }
            return Ok(new
            {
                Tenant = _mapper.Map(tenant)
            });
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<User>(StatusCodes.Status201Created)]
        public async Task<IActionResult> Post([FromBody] CreateTenantCommand command, CancellationToken cancellationToken)
        {
            var tenant = new Tenant
            {
                UserName = command.name,
                Email = command.email,
                Password = command.password,
                FirstName = command.firstname,
                LastName = command.lastname,
                TenantId = command.tenantid
            };

            await _unitOfWork.Tenants.Add(tenant);

            await _unitOfWork.SaveAsync(cancellationToken);

            return CreatedAtAction(nameof(GetValue), new
            {
                id = user.Id
            }, user);
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
            var user = await _unitOfWork.Tenants.GetAsync(u => u.Id == id, cancellationToken);
            if (user is null)
            {
                return NotFound();
            }
            await _unitOfWork.Tenants.Delete(user);
            await _unitOfWork.SaveAsync(cancellationToken);
            return NoContent();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<User>> Update(int id, [FromBody] UpdateUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Tenants.GetAsync(u => u.Id == id, cancellationToken);
            user.Email = command.email;
            await _unitOfWork.SaveAsync(cancellationToken);
            return NoContent();
        }
    }