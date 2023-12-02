using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SohatNotebook.DataService.IConfiguration;
using SohatNotebook.Entities.DbSet;
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
        return Ok(users);
    }

    [HttpGet("{id:guid}", Name = "GetUser")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var user = await _unitOfWork.Users.GetById(id);

        if (user is null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody] UserDto userDto)
    {
        if (!ModelState.IsValid)
            return BadRequest();

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

