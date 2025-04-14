using ConstructorForTests.Database;
using Microsoft.AspNetCore.Mvc;
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

		public async Task<bool> LogIn(string email, string password, ISession session)
		{
			var user = await _context.Curators
				.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
			if (user == null)
			{
				return false;
			}

			session.SetString("UserId", user.Id.ToString());
			session.SetString("Email", email);

			return true;
		}
	}
}
