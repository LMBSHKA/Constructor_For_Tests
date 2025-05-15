namespace ConstructorForTests.Models
{
	public class User
	{
		public Guid Id { get; set; }
		public string? FirstName { get; set; }
		public string? SecondName { get; set; }
		public string? Patronymic { get; set; }
		public string? Email { get; set; }

		public User(string firstName, string secondName, string patronymic, string email)
		{
			FirstName = firstName;
			SecondName = secondName;
			Patronymic = patronymic;
			Email = email;
		}
	}
}
