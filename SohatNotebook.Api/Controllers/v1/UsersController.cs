using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SohatNotebook.Configuration.Messages;
using SohatNotebook.DataService.IConfiguration;
using SohatNotebook.Entities.DbSet;
using SohatNotebook.Entities.Dtos.Generic;
using SohatNotebook.Entities.Dtos.Incoming;

namespace SohatNotebook.Api.Controllers.v1;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UsersController : BaseController
{
    public UsersController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        : base(unitOfWork, userManager)
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

        var result = new Result<User>();
        if (user is null)
        {
            result.Error = PopulateError(404,
                 ErrorMessages.Users.UserNotFound,
                 ErrorMessages.Generic.TypeNotFound);

            return NotFound(result);
        }

        result.Content = user;
        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody] UserDto userDto)
    {
        var result = new Result<User>();
        if (!ModelState.IsValid)
        {
            result.Error = PopulateError(400,
                ErrorMessages.Generic.InvalidPayload,
                ErrorMessages.Generic.TypeBadRequest);
            return BadRequest(result);
        }

        var user = new User()
        {
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            Email = userDto.Email,
            Phone = userDto.Phone,
            DateOfBirth = Convert.ToDateTime(userDto.DateOfBirth),
            Country = userDto.Country,
            Status = 1
        };

        await _unitOfWork.Users.Add(user);
        await _unitOfWork.CompleteAsync();

        return CreatedAtRoute(nameof(GetUser), new { id = user.Id }, user); // return a 201
    }
}

