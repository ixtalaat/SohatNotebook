namespace SohatNotebook.Entities.DbSet
{
	public class User : BaseEntity
	{
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string? Phone { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string? Country { get; set; }
	}
}
