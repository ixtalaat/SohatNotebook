﻿namespace SohatNotebook.Authentication.Configruation
{
	public class JwtConfig
	{
        public string Secret { get; set; } = string.Empty;
        public TimeSpan ExpiryTimeFrame { get; set; }

	}
}
