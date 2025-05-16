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
		public async Task<IActionResult> Registration(RegistrationDto registrationData)
		{
			return await _authenticationRepo
				.Registration(registrationData) ? Ok() : BadRequest("User already exists");
		}

		[HttpPost("LogIn")]
		public async Task<IActionResult> LogIn([FromBody] AuthenticationDto userAccessData)
		{
			return await _authenticationRepo
				.LogIn(userAccessData, HttpContext.Session) ? Ok() : BadRequest("Invalid LogIn data");
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
