using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SohatNotebook.DataService.Data;
using SohatNotebook.DataService.IRepository;
using SohatNotebook.Entities.DbSet;

namespace SohatNotebook.DataService.Repository;

public class HealthDataRepository : GenericRepository<HealthData>, IHealthDataRepository
{
    public HealthDataRepository(ApplicationDbContext context, ILogger logger) : base(context, logger)
    {
    }
    public override async Task<IEnumerable<HealthData>> All()
    {
        try
        {
            return await dbSet.Where(u => u.Status == 1)
                .AsNoTracking()
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} All method has generated an error", typeof(HealthDataRepository));
            return new List<HealthData>();
        }
    }

    public async Task<bool> UpdateHealthData(HealthData healthData)
    {
        try
        {
            var existingHealthData = await dbSet.Where(u => u.Status == 1 && u.Id == healthData.Id).FirstOrDefaultAsync();
            if (existingHealthData == null) return false;

            existingHealthData.Height = healthData.Height;
            existingHealthData.Weight = healthData.Weight;
            existingHealthData.Race = healthData.Race;
            existingHealthData.BloodType = healthData.BloodType;
            existingHealthData.UseGlasses = healthData.UseGlasses;
            existingHealthData.UpdatedDate = DateTime.UtcNow;

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Repo} UpdateHealthData method has generated an error", typeof(HealthDataRepository));
            return false;
        }
    }
}


