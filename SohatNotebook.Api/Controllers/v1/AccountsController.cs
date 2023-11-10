using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SohatNotebook.Authentication.Configruation;
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
	}
}
