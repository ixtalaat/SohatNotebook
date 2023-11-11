using System.ComponentModel.DataAnnotations;

namespace SohatNotebook.Authentication.Models.DTO.Incoming
{
	public class UserRegistrationRequestDto
	{
        [Required]
        public string FirstName { get; set; } =  null!;
        [Required]
        public string LastName { get; set; } = null!;
		[Required]
		public string Email { get; set; } = null!;
		[Required]
        public string Password { get; set; } = null!;
	}
}
