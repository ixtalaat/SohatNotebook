using SohatNotebook.Entities.DbSet;

namespace SohatNotebook.DataService.IRepository;

public interface IHealthDataRepository : IGenericRepository<HealthData>
{
    Task<bool> UpdateHealthData(HealthData healthData);
}
