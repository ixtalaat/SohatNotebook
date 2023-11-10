using System.ComponentModel.DataAnnotations;

namespace SohatNotebook.Authentication.Models.DTO.Incoming
{
	public class UserRegistrationRequestDto
	{
        [Required]
        public required string FirstName { get; set; }
        [Required]
        public required string LastName { get; set; }
		[Required]
		public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
