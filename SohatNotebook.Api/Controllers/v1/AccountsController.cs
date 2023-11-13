using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SohatNotebook.Authentication.Configruation;
using SohatNotebook.Authentication.Models.DTO.Incoming;
using SohatNotebook.Authentication.Models.DTO.Outcoming;
using SohatNotebook.DataService.IConfiguration;
using SohatNotebook.Entities.DbSet;
using SohatNotebook.Entities.Dtos.Incoming;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SohatNotebook.Api.Controllers.v1
{
	public class AccountsController : BaseController
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly JwtConfig _jwtConfig;
		public AccountsController(IUnitOfWork unitOfWork,
			UserManager<IdentityUser> userManager,
			IOptionsMonitor<JwtConfig> OptionMonitor) : base(unitOfWork)
		{
			_jwtConfig = OptionMonitor.CurrentValue;
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
            
			var userExists = await _userManager.FindByEmailAsync(registrationDto.Email);
			if (userExists != null)
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

			// Adding user to the database
			var user = new User()
			{
				IdentityId = new Guid(newUser.Id),
				FirstName = registrationDto.FirstName,
				LastName = registrationDto.LastName,
				Email = registrationDto.Email,
				DateOfBirth = DateTime.UtcNow, //Convert.ToDateTime(userDto.DateOfBirth),
				Phone = "",
				Country = "",
				Status = 1
			};

			await _unitOfWork.Users.Add(user);
			await _unitOfWork.CompleteAsync();

			var jwtToken = GenerateJwtToken(newUser);

			return Ok(new UserRegistrationResponseDto
			{
				Success = true,
				Token = jwtToken
			});
		}

		// Logic Action
		[HttpPost]
		[Route("Login")]
		public async Task<IActionResult> Login([FromBody] UserLoginRequestDto loginDto)
		{
			if(!ModelState.IsValid)
			{

				return BadRequest(new UserLoginResponseDto
				{
					Success = false,
					Errors = new List<string>()
					{
						"Invalid payload"
					}
				});
			}

			var userExists = await _userManager.FindByEmailAsync(loginDto.Email);
			if (userExists == null || !await _userManager.CheckPasswordAsync(userExists, loginDto.Password))
			{
				return BadRequest(new UserLoginResponseDto
				{
					Success = false,
					Errors = new List<string>()
					{
						"Invalid login attempt"
					}
				});
			}

			var jwtToken = GenerateJwtToken(userExists);

			return Ok(new UserLoginResponseDto
			{
				Success = true,
				Token = jwtToken
			});
		}

		private string GenerateJwtToken(IdentityUser user)
		{
			// the handler is going to be responsible for creating the token
			var jwtHandler = new JwtSecurityTokenHandler();

			// Get the security key
			var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

			var tokenDescriptor = new SecurityTokenDescriptor 
			{
				Subject = new ClaimsIdentity(new[]
				{
					new Claim("Id", user.Id),
					new Claim(JwtRegisteredClaimNames.Sub, user.Email!), // unique id
					new Claim(JwtRegisteredClaimNames.Email, user.Email!),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // used by the refresh token
				}),
				Expires = DateTime.UtcNow.AddHours(3), // Todo update the expiration time to minutes
				SigningCredentials = new SigningCredentials(
					new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256
				)
			};

			// Generate the security obj token
			var token = jwtHandler.CreateToken(tokenDescriptor);

			// Convert the security obj token into a string
			var jwtToken = jwtHandler.WriteToken(token);

			return jwtToken;
		}

	}
}
