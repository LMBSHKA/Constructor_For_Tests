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
		private readonly IAuthenticationRepo _authenticationRepo;

		public AuthenticationController(IAuthenticationRepo authenticationRepo)
		{
			_authenticationRepo = authenticationRepo;
		}

		[HttpPost("Registration")]
		public IActionResult Registration(RegistrationDto registrationData)
		{
			return _authenticationRepo
				.Registration(registrationData)
				.Result ? Ok() : BadRequest("User already exists");
		}

		[HttpPost("LogIn")]
		public IActionResult LogIn([FromBody] AuthenticationDto userAccessData)
		{
			return _authenticationRepo
				.LogIn(userAccessData, HttpContext.Session)
				.Result ? Ok() : BadRequest("Invalid LogIn data");
		}

		[HttpPost("LogOut")]
		public IActionResult LogOut()
		{
			try
			{
				HttpContext.Response.Cookies.Delete("Session");
				HttpContext.Session.Clear();
				return Ok();
			}
			catch
			{
				return BadRequest("Something went wrong");
			}
		}

		[SessionAuthentication]
		[HttpGet("TestAuth")]
		public IActionResult Test()
		{
			return Ok();
		}
	}
}
