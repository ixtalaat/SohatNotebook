﻿using System.ComponentModel.DataAnnotations;

namespace SohatNotebook.Authentication.Models.DTO.Incoming
{
	public class TokenRequest
	{
		[Required]
		public string Token { get; set; } = string.Empty;
		[Required]
		public string RefreshToken { get; set; } = string.Empty;
	}
}
