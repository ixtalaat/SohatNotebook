using System.ComponentModel.DataAnnotations;

namespace SohatNotebook.Entities.Dtos.Outgoing.Profile
{
    public class ProfileDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string DateOfBirth { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}
