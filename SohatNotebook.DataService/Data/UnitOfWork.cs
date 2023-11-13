using Microsoft.Extensions.Logging;
using SohatNotebook.DataService.IConfiguration;
using SohatNotebook.DataService.IRepository;
using SohatNotebook.DataService.Repository;

namespace SohatNotebook.DataService.Data
{
	public class UnitOfWork : IUnitOfWork, IDisposable
	{
		private readonly ApplicationDbContext _context;
		private readonly ILogger _logger;
		public IUsersRepository Users { get; private set; }
		public IRefreshTokenRepository RefreshTokens { get; private set; }

		public UnitOfWork(ApplicationDbContext context, ILoggerFactory loggerFactory)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_logger = loggerFactory.CreateLogger("db_logs");
			Users = new UsersRepository(context, _logger);
			RefreshTokens = new RefreshTokenRepository(context, _logger);
		}

		public async Task CompleteAsync()
		{
			await _context.SaveChangesAsync();
		}

		public void Dispose()
		{
			_context.Dispose();
		}
	}
}
