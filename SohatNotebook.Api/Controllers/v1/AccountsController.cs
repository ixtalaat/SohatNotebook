using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SohatNotebook.Authentication.Configruation;
using SohatNotebook.Authentication.Models.DTO.Generic;
using SohatNotebook.Authentication.Models.DTO.Incoming;
using SohatNotebook.Authentication.Models.DTO.Outcoming;
using SohatNotebook.DataService.IConfiguration;
using SohatNotebook.Entities.DbSet;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SohatNotebook.Api.Controllers.v1;
public class AccountsController : BaseController
{
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly JwtConfig _jwtConfig;
    public AccountsController(IUnitOfWork unitOfWork,
        UserManager<IdentityUser> userManager,
        IOptionsMonitor<JwtConfig> OptionMonitor,
        TokenValidationParameters tokenValidationParameters) : base(unitOfWork, userManager)
    {
        _jwtConfig = OptionMonitor.CurrentValue;
        _tokenValidationParameters = tokenValidationParameters;
    }

    // Register Action
    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto registrationDto)
    {
        if (!ModelState.IsValid)
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
            UserName = registrationDto.Email,
            EmailConfirmed = true // Todo build email functionality to send to the user to comfirm email
        };

        var isCreated = await _userManager.CreateAsync(newUser, registrationDto.Password);
        if (!isCreated.Succeeded)
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

        var jwtToken = await GenerateJwtToken(newUser);

        return Ok(new UserRegistrationResponseDto
        {
            Success = true,
            Token = jwtToken.JwtToken,
            RefreshToken = jwtToken.RefreshToken
        });
    }

    // Logic Action
    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequestDto loginDto)
    {
        if (!ModelState.IsValid)
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

        var jwtToken = await GenerateJwtToken(userExists);

        return Ok(new UserLoginResponseDto
        {
            Success = true,
            Token = jwtToken.JwtToken,
            RefreshToken = jwtToken.RefreshToken
        });
    }

    [HttpPost]
    [Route("RefreshToken")]
    public async Task<ActionResult> RefreshToken([FromBody] TokenRequestDto tokenRequestDto)
    {
        if (!ModelState.IsValid)
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

        // Check if the token is valid
        var result = await VerifyToken(tokenRequestDto);

        if (result == null)
        {
            return BadRequest(new UserRegistrationResponseDto
            {
                Success = false,
                Errors = new List<string>()
                    {
                        "Token validation failed."
                    }
            });
        }

        return Ok(result);

    }

    private async Task<AuthResult> VerifyToken(TokenRequestDto tokenRequestDto)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            // We need to check the validity of the token
            var principal = tokenHandler.ValidateToken(tokenRequestDto.Token, _tokenValidationParameters, out var validatedToken);

            // We need to validate the results that has been generated for us
            // Validate if the string is an actual JWT token not a random string
            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                // check if the jwt token is created with the same algorithm as our jwt token
                var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                if (!result)
                    return null!;
            }

            // We need to check the expiry date of the token
            var utcExpiryDate = long.Parse(principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)!.Value);

            // convert to date to check
            var expDate = UnixTimeStampToDateTime(utcExpiryDate);

            // Checking if the jwt token has expired
            if (expDate > DateTime.UtcNow)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string>()
                        {
                            "Jwt token has not expired"
                        }
                };
            }

            // Check if the refresh token exist
            var refreshTokenExist = await _unitOfWork.RefreshTokens.GetByRefreshToken(tokenRequestDto.RefreshToken);
            if (refreshTokenExist == null)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string>()
                        {
                            "Invalid refresh token"
                        }
                };
            }

            // Check the expiry date of a refresh token
            if (refreshTokenExist.ExpiryDate < DateTime.UtcNow)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string>()
                        {
                            "Refresh token has expired, please login again."
                        }
                };
            }

            // Check if refresh token has been used or not
            if (refreshTokenExist.IsUsed)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string>()
                        {
                            "Refresh token has been used, it cannot be reused."
                        }
                };
            }

            // Check if refresh token if it has been revoked
            if (refreshTokenExist.IsRevoked)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string>()
                        {
                            "Refresh token has been revoked, it cannot be used."
                        }
                };
            }

            var jti = principal.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)!.Value;
            if (refreshTokenExist.JwtId != jti)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string>()
                        {
                            "Refresh token reference does not match the jwt token."
                        }
                };
            }

            // Start processing and get a new token
            refreshTokenExist.IsUsed = true;

            var updateResult = await _unitOfWork.RefreshTokens.MarkRefreshTokenAsUsed(refreshTokenExist);

            if (!updateResult)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string>()
                        {
                            "Error processing request"
                        }
                };
            }

            await _unitOfWork.CompleteAsync();

            // Get the user to generate a new jwt token
            var dbUser = await _userManager.FindByIdAsync(refreshTokenExist.UserId);
            if (dbUser == null)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string>()
                        {
                            "Error processing request"
                        }
                };
            }

            // Generate a jwt token
            var tokens = await GenerateJwtToken(dbUser);

            return new AuthResult
            {
                Success = true,
                Token = tokens.JwtToken,
                RefreshToken = tokens.RefreshToken
            };
        }
        catch (Exception)
        {
            // TODO: Add better error handling, and add a logger
            return null!;
        }
    }

    private DateTime UnixTimeStampToDateTime(long unixTime)
    {
        // Sets the time to 1, jan, 1970
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        // Add the number of seconds from 1 Jan 1970
        dateTime = dateTime.AddSeconds(unixTime).ToUniversalTime();
        return dateTime;
    }

    private async Task<TokenData> GenerateJwtToken(IdentityUser user)
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
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email!), // unique id
					new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // used by the refresh token
				}),
            Expires = DateTime.UtcNow.Add(_jwtConfig.ExpiryTimeFrame), // Todo update the expiration time to minutes
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256
            )
        };

        // Generate the security obj token
        var token = jwtHandler.CreateToken(tokenDescriptor);

        // Convert the security obj token into a string
        var jwtToken = jwtHandler.WriteToken(token);

        // Generate refresh token
        var refreshToken = new RefreshToken
        {
            AddedDate = DateTime.UtcNow,
            Token = $"{RandomStringGenerator(25)}_{Guid.NewGuid()}",
            UserId = user.Id,
            IsRevoked = false,
            IsUsed = false,
            Status = 1,
            JwtId = token.Id,
            ExpiryDate = DateTime.UtcNow.AddMonths(6),
        };

        await _unitOfWork.RefreshTokens.Add(refreshToken);
        await _unitOfWork.CompleteAsync();

        var tokenData = new TokenData
        {
            JwtToken = jwtToken,
            RefreshToken = refreshToken.Token,
        };

        return tokenData;
    }

    private string RandomStringGenerator(int length)
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

}

