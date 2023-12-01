namespace SohatNotebook.Entities.DbSet
{
	public class User : BaseEntity
	{
        public Guid IdentityId { get; set; }
        public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Phone { get; set; } = string.Empty;
		public DateTime DateOfBirth { get; set; }
		public string Country { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public string MobileNumber { get; set; } = string.Empty;
		public string Gender { get; set; } = string.Empty;
	}
}
