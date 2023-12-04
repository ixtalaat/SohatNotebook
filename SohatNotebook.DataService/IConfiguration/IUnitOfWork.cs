using SohatNotebook.DataService.IRepository;

namespace SohatNotebook.DataService.IConfiguration;
public interface IUnitOfWork
{
	IUsersRepository Users { get; }
	IRefreshTokenRepository RefreshTokens { get; }
	IHealthDataRepository HealthData { get; }
	Task CompleteAsync();
}
