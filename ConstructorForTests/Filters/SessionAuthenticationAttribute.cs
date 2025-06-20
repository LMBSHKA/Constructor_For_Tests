using ConstructorForTests.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ConstructorForTests.Filters
{
	/// <summary>
	/// Аутентификация, использует сессии asp.net core
	/// не записывает сессии в базу данных
	/// <a href="https://metanit.com/sharp/aspnet6/8.3.php">узнать подробнее</a>  
	/// </summary>
	public class SessionAuthenticationAttribute : Attribute, IAsyncAuthorizationFilter
	{
		public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
		{
			var curatorId = context.HttpContext.Session.GetString("CuratorId");
			if (curatorId == null)
			{
				context.Result = new UnauthorizedObjectResult("Error authentication, invalid coockie");
				return;
			}

			var user = await context.HttpContext.RequestServices
				.GetRequiredService<AppDbContext>()
				.Curators
				.FindAsync(new Guid(curatorId));

			if (user == null)
			{
				context.Result = new UnauthorizedObjectResult("Error authentication, user not found");
				return;
			}
		}
	}
}
