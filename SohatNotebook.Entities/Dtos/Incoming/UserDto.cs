using System.ComponentModel.DataAnnotations;

namespace SohatNotebook.Entities.Dtos.Incoming;
public class UserDto
{
	[Required]
	public string FirstName { get; set; } = string.Empty;
	[Required]
	public string LastName { get; set; } = string.Empty;
	[Required]
	[EmailAddress]
	public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
	public string Country { get; set; } = string.Empty;
}
