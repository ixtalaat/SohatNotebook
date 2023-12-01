using SohatNotebook.Entities.DbSet;

namespace SohatNotebook.DataService.IRepository
{
	public interface IUsersRepository : IGenericRepository<User>
	{
		Task<bool> UpdateUserProfile(User user);
		Task<User> GetByIdentityId(Guid identityId);
    }
}
