using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SohatNotebook.Configuration.Messages;
using SohatNotebook.DataService.IConfiguration;
using SohatNotebook.Entities.DbSet;
using SohatNotebook.Entities.Dtos.Generic;
using SohatNotebook.Entities.Dtos.Incoming;
using SohatNotebook.Entities.Dtos.Outgoing.Profile;
using static SohatNotebook.Configuration.Messages.ErrorMessages;

namespace SohatNotebook.Api.Controllers.v1;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UsersController : BaseController
{
    public UsersController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, IMapper mapper)
        : base(unitOfWork, userManager, mapper)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _unitOfWork.Users.All();

        var result = new PagedResult<User>();
        result.Content = users.ToList();
        result.ResultCount = users.Count();
        return Ok(result);
    }

    [HttpGet("{id:guid}", Name = "GetUser")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await _unitOfWork.Users.GetById(id);

        var result = new Result<ProfileDto>();
        if (user is null)
        {
            result.Error = PopulateError(404,
                 ErrorMessages.Users.UserNotFound,
                 ErrorMessages.Generic.TypeNotFound);

            return NotFound(result);
        }
        var mappedProfile = _mapper.Map<ProfileDto>(user);
        result.Content = mappedProfile;
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody] UserDto userDto)
    {
        var result = new Result<UserDto>();
        if (!ModelState.IsValid)
        {
            result.Error = PopulateError(400,
                ErrorMessages.Generic.InvalidPayload,
                ErrorMessages.Generic.TypeBadRequest);
            return BadRequest(result);
        }

        var mappedUser = _mapper.Map<User>(userDto);

        await _unitOfWork.Users.Add(mappedUser);
        await _unitOfWork.CompleteAsync();

        result.Content = userDto;

        return CreatedAtRoute(nameof(GetUser), new { id = mappedUser.Id }, result); // return a 201
    }
}

