namespace SohatNotebook.Authentication.Models.DTO.Generic
{
    public class TokenData
    {
        public string JwtToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
