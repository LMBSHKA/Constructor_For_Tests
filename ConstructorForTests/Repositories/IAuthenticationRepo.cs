using ConstructorForTests.Dtos;

namespace ConstructorForTests.Repositories
{
	public interface IAuthenticationRepo
	{
		public Task<bool> LogIn(AuthenticationDto userAccessData, ISession session);
		public Task<bool> Registration(RegistrationDto registrationData);
	}
}
