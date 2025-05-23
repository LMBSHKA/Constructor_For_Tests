using ConstructorForTests.Dtos;
using ConstructorForTests.Filters;
using ConstructorForTests.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ConstructorForTests.Controllers
{
	/// <summary>
	/// Endppoint-ы для управления учетными записями
	/// </summary>
	[ApiController]
	[Route("api/v1/Auth/")]
	public class AuthenticationController : ControllerBase
	{
		private readonly IAuthenticationRepo _authenticationRepo;

		public AuthenticationController(IAuthenticationRepo authenticationRepo)
		{
			_authenticationRepo = authenticationRepo;
		}

		/// <summary>
		/// Регистрация для руководителя
		/// </summary>
		/// <returns></returns>
		/// <response code="200">Успешное выполнение</response>
		/// <response code="400">Пользователь уже существует или ошибка API(какие-то данные были неверными)</response>
		/// <response code="500">Ошибка сервера</response>
		[HttpPost("Registration")]
		public async Task<IActionResult> Registration(RegistrationDto registrationData)
		{
			return await _authenticationRepo
				.Registration(registrationData) ? Ok() : BadRequest("User already exists");
		}

		/// <summary>
		/// Вход для руководителя
		/// </summary>
		/// <returns></returns>
		/// <response code="200">Успешное выполнение</response>
		/// <response code="400">Неверные данные для входа или ошибка API(какие-то данные были неверными)</response>
		/// <response code="500">Ошибка сервера</response>
		[HttpPost("LogIn")]
		public async Task<IActionResult> LogIn([FromBody] AuthenticationDto userAccessData)
		{
			return await _authenticationRepo
				.LogIn(userAccessData, HttpContext.Session) ? Ok() : BadRequest("Invalid LogIn data");
		}

		/// <summary>
		/// Выход для руководителя
		/// </summary>
		/// <returns></returns>
		/// <response code="200">Успешное выполнение</response>
		/// <response code="400">ошибка API, что-то на сервере пошло не так</response>
		/// <response code="500">Ошибка сервера</response>
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
	}
}
