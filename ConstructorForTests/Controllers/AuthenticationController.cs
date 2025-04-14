using ConstructorForTests.Database;
using ConstructorForTests.Dtos;
using ConstructorForTests.Filters;
using ConstructorForTests.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ConstructorForTests.Controllers
{
	[ApiController]
	[Route("api/v1/Auth/")]
	public class AuthenticationController : ControllerBase
	{
		private readonly AppDbContext _context;
		private readonly IAuthenticationRepo _authenticationRepo;

		public AuthenticationController(IAuthenticationRepo authenticationRepo, AppDbContext context)
		{
			_context = context;
			_authenticationRepo = authenticationRepo;
		}

		[HttpPost("LogIn")]
		public IActionResult LogIn([FromBody] AuthenticationDto userAccessData)
		{
			return _authenticationRepo
				.LogIn(userAccessData.Email, userAccessData.Password, HttpContext.Session)
				.Result ? Ok() : Unauthorized();
		}

		[HttpPost("LogOut")]
		public IActionResult LogOut()
		{
			HttpContext.Response.Cookies.Delete("Session");
			HttpContext.Session.Clear();
			return Ok();
		}

		[SessionAuthentication]
		[HttpGet("TestAuth")]
		public IActionResult Test()
		{
			return Ok();
		}
	}
}
