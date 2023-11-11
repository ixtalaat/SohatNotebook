namespace SohatNotebook.Entities.DbSet
{
	public class User : BaseEntity
	{
        public Guid IdentityId { get; set; }
        public string FirstName { get; set; } = null!;
		public string LastName { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string Phone { get; set; } = null!;
		public DateTime DateOfBirth { get; set; }
		public string Country { get; set; } = null!;
	}
}
