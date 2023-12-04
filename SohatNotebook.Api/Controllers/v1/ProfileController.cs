using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SohatNotebook.Api.Profiles;
using SohatNotebook.Configuration.Messages;
using SohatNotebook.DataService.IConfiguration;
using SohatNotebook.Entities.DbSet;
using SohatNotebook.Entities.Dtos.Generic;
using SohatNotebook.Entities.Dtos.Incoming.Profile;
using SohatNotebook.Entities.Dtos.Outgoing.Profile;

namespace SohatNotebook.Api.Controllers.v1;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ProfileController : BaseController
{
    public ProfileController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, IMapper mapper)
        : base(unitOfWork, userManager, mapper)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);

        var result = new Result<ProfileDto>();

        if (loggedInUser == null)
        {
            result.Error = PopulateError(400,
                ErrorMessages.Profile.UserNotFound,
                ErrorMessages.Generic.TypeBadRequest);

            return BadRequest(result);
        }

        var identityId = new Guid(loggedInUser.Id);

        var profile = await _unitOfWork.Users.GetByIdentityId(identityId);
        if (profile == null)
        {
            result.Error = PopulateError(400,
                 ErrorMessages.Profile.UserNotFound,
                 ErrorMessages.Generic.TypeBadRequest);

            return BadRequest(result);
        }

        var mappedProfile = _mapper.Map<ProfileDto>(profile);


        result.Content = mappedProfile;
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto profileDto)
    {
        var result = new Result<ProfileDto>();

        if (!ModelState.IsValid)
        {
            result.Error = PopulateError(400,
                ErrorMessages.Generic.InvalidPayload,
                ErrorMessages.Generic.TypeBadRequest);
            return BadRequest(result);
        }

        var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
        if (loggedInUser == null)
        {
            result.Error = PopulateError(400,
                ErrorMessages.Profile.UserNotFound,
                ErrorMessages.Generic.TypeBadRequest);

            return BadRequest(result);
        }

        var identityId = new Guid(loggedInUser.Id);

        var userProfile = await _unitOfWork.Users.GetByIdentityId(identityId);
        if (userProfile == null)
        {
            result.Error = PopulateError(400,
                ErrorMessages.Profile.UserNotFound,
                ErrorMessages.Generic.TypeBadRequest);

            return BadRequest(result);
        }

        userProfile.Country = profileDto.Country;
        userProfile.Address = profileDto.Address;
        userProfile.MobileNumber = profileDto.MobileNumber;
        userProfile.Gender = profileDto.Gender;


        var isUpdated = await _unitOfWork.Users.UpdateUserProfile(userProfile);

        if (!isUpdated)
        {
            result.Error = PopulateError(500,
                ErrorMessages.Generic.SomethingWentWrong,
                ErrorMessages.Generic.UnableToProcess);

            return BadRequest(result);
        }

        await _unitOfWork.CompleteAsync();

        var mappedProfile = _mapper.Map<ProfileDto>(userProfile);

        result.Content = mappedProfile;
        return Ok(result);
    }

}

