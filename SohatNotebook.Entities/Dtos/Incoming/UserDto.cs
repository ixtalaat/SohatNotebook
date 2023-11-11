namespace SohatNotebook.Entities.Dtos.Incoming
{
	public class UserDto
	{
		public string FirstName { get; set; } = null!;
		public string LastName { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string Phone { get; set; } = null!;
		public string DateOfBirth { get; set; } = null!;
		public string Country { get; set; } = null!;
	}
}
