namespace SohatNotebook.Authentication.Models.DTO.Outcoming
{
	public class AuthResult
	{
        public required string Token { get; set; }
		public bool Success { get; set; }
        public List<string>? Errors { get; set; }
    }
}
