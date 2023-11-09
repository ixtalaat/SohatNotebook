using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SohatNotebook.DataService.Data;
using SohatNotebook.DataService.IRepository;

namespace SohatNotebook.DataService.Repository
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		protected ApplicationDbContext _context;
		internal DbSet<T> dbSet;
		protected readonly ILogger _logger;
		public GenericRepository(ApplicationDbContext context, ILogger logger)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			dbSet = context.Set<T>();
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public virtual async Task<bool> Add(T entity)
		{
			await dbSet.AddAsync(entity);
			return true;
		}

		public virtual async Task<IEnumerable<T>> All()
		{
			return await dbSet.ToListAsync();
		}

		public Task<bool> Delete(Guid id, string userId)
		{
			throw new NotImplementedException();
		}

		public virtual async Task<T> GetById(Guid id)
		{
			var user = await dbSet.FindAsync(id);
			return user;
		}

		public Task<bool> Update(T entity)
		{
			throw new NotImplementedException();
		}
	}
}
