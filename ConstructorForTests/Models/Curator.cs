using System.ComponentModel.DataAnnotations;

namespace ConstructorForTests.Models
{
	public class Curator
	{
		[Key]
		public Guid Id { get; set; }
		[Required]
		public string Email { get; set; }
		[Required]
		public string Password { get; set; }

		public Curator() { }

		public Curator(string email, string password)
		{
			Email = email;
			Password = password;
		}
	}
}
