using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SohatNotebook.Entities.DbSet
{
	public class RefreshToken : BaseEntity
	{
		public string UserId { get; set; } = string.Empty; // User Id when logged in
		public string Token { get; set; } = string.Empty;
		public string JwtId { get; set; } = string.Empty; // the id generated when a jwt id has been requested
        public bool IsUsed { get; set; } // To make sure that the token is only used once
        public bool IsRevoked { get; set; } // make sure they are valid
        public DateTime ExpiryDate { get; set; }

        [ForeignKey(nameof(UserId))]
        public IdentityUser? User { get; set; }
    }
}
