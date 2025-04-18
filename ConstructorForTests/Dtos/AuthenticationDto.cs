﻿using System.ComponentModel.DataAnnotations;

namespace ConstructorForTests.Dtos
{
	public class AuthenticationDto
	{
		[Required]
		public string Email { get; set; }
		[Required]
		public string Password { get; set; }
	}
}
