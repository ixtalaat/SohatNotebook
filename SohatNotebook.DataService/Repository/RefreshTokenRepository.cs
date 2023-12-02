using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SohatNotebook.DataService.Data;
using SohatNotebook.DataService.IRepository;
using SohatNotebook.Entities.DbSet;

namespace SohatNotebook.DataService.Repository;
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
    public async Task<RefreshToken> GetByRefreshToken(string refreshToken)
    {
        try
        {
            return (await dbSet.Where(u => u.Token.ToLower() == refreshToken.ToLower())
                .AsNoTracking()
                .FirstOrDefaultAsync())!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} GetByRefreshToken method has generated an error", typeof(RefreshTokenRepository));
            return null!;
        }
    }

    public async Task<bool> MarkRefreshTokenAsUsed(RefreshToken refreshToken)
    {
        try
        {
            var token = await dbSet.Where(u => u.Token.ToLower() == refreshToken.Token.ToLower())
                .FirstOrDefaultAsync();

            if (token == null) return false;

            token.IsUsed = refreshToken.IsUsed;
            return true;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} MarkRefreshTokenAsUsed method has generated an error", typeof(RefreshTokenRepository));
            return false;
        }
    }

}
