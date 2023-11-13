using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SohatNotebook.DataService.Data;
using SohatNotebook.DataService.IRepository;
using SohatNotebook.Entities.DbSet;

namespace SohatNotebook.DataService.Repository
{
	public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
	{
		public RefreshTokenRepository(ApplicationDbContext context, ILogger logger)
			: base(context, logger)
		{
		}

		public override async Task<IEnumerable<RefreshToken>> All()
		{
			try
			{
				return await dbSet.Where(u => u.Status == 1)
					.AsNoTracking()
					.ToListAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "{Repo} All method has generated an error", typeof(RefreshTokenRepository));
				return new List<RefreshToken>();
			}
		}
	}
}
