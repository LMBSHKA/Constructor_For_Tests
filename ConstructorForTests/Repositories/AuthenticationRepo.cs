using ConstructorForTests.Database;
using ConstructorForTests.Dtos;
using ConstructorForTests.Models;
using Microsoft.EntityFrameworkCore;

namespace ConstructorForTests.Repositories
{
	public class AuthenticationRepo : IAuthenticationRepo
	{
		private readonly AppDbContext _context;

		public AuthenticationRepo(AppDbContext context)
		{
			_context = context;
		}

		public async Task<bool> LogIn(AuthenticationDto userAccessData, ISession session)
		{
			var email = userAccessData.Email;
			var password = userAccessData.Password;
			var user = await _context.Curators
				.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);

			if (user == null)
			{
				return false;
			}

			session.SetString("CuratorId", user.Id.ToString());
			session.SetString("Email", email);

			return true;
		}

		public async Task<bool> Registration(RegistrationDto registrationData)
		{
			var email = registrationData.Email;
			var password = registrationData.Password;
			var user = await _context.Curators.FirstOrDefaultAsync(x => x.Email == email);

			if (user != null)
				return false;

			await _context.Curators.AddAsync(new Curator(email, password));
			await _context.SaveChangesAsync();

			return true;
		}
	}
}
