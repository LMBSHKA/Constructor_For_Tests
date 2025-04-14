namespace ConstructorForTests.Repositories
{
	public interface IAuthenticationRepo
	{
		public Task<bool> LogIn(string email, string password, ISession session);
	}
}
