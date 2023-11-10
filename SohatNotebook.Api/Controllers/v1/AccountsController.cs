using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SohatNotebook.Authentication.Configruation;
using SohatNotebook.Authentication.Models.DTO.Incoming;
using SohatNotebook.Authentication.Models.DTO.Outcoming;
using SohatNotebook.DataService.IConfiguration;

namespace SohatNotebook.Api.Controllers.v1
{
	public class AccountsController : BaseController
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly JwtConfig _jwt;
		public AccountsController(IUnitOfWork unitOfWork,
			UserManager<IdentityUser> userManager,
			IOptionsMonitor<JwtConfig> jwt) : base(unitOfWork)
		{
			_jwt = jwt.CurrentValue;
			_userManager = userManager;
		}

		// Register Action
		[HttpPost]
		[Route("Register")]
		public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto registrationDto)
		{
			if(!ModelState.IsValid)
			{
				return BadRequest(new UserRegistrationResponseDto
				{
					Success = false,
					Errors = new List<string>()
					{
						"Invalid payload"
					}
				});
			}
            
			var userExist = _userManager.FindByEmailAsync(registrationDto.Email);
			if (userExist != null)
			{
				return BadRequest(new UserRegistrationResponseDto
				{
					Success = false,
					Errors = new List<string>()
					{
						"Email already in use"
					}
				});
			}

			var newUser = new IdentityUser()
			{
				Email = registrationDto.Email,
				UserName =registrationDto.Email,
				EmailConfirmed = true // Todo build email functionality to send to the user to comfirm email
			};

			var isCreated = await _userManager.CreateAsync(newUser, registrationDto.Password);
			if(!isCreated.Succeeded)
			{
				return BadRequest(new UserRegistrationResponseDto
				{
					Success = isCreated.Succeeded,
					Errors = isCreated.Errors.Select(x => x.Description).ToList()
				});
			}
		}


	}
}
