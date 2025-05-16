using ConstructorForTests.Dtos;
using ConstructorForTests.Filters;
using ConstructorForTests.Models;
using ConstructorForTests.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ConstructorForTests.Controllers
{
	[ApiController]
	[Route("api/v1/operationsOnTest")]
	public class CreateTestController : ControllerBase
	{
		private readonly ITestRepo _testRepo;

		public CreateTestController(ITestRepo testRepo)
		{
			_testRepo = testRepo;
		}

		[HttpGet("GetAllTests")]
		public async Task<IActionResult> GetAllTests()
		{
			return Ok(await _testRepo.GetAllTests());
		}

		[HttpGet("GetTest/{id}")]
		public async Task<IActionResult> GetTestById(Guid id)
		{
			var test = await _testRepo.GetTestById(id, HttpContext.Session);

			if (test == null)
				return NotFound();

			return Ok(test);
		}

		[HttpPost("CreateTest")]
		public async Task<IActionResult> CreateTest([FromBody] CreateTestDto createTestData)
		{
			
			if (await _testRepo.CreateTest(createTestData))
				return Ok();

			return BadRequest("Something went wrong");
		}

		[HttpPut("Update/{id}")]
		public async Task<IActionResult> UpdateTest(Guid id, [FromBody] Test updateTestData)
		{
			if (await _testRepo.UpdateTest(id, updateTestData))
				return Ok();

			return BadRequest("Something went wrong");
		}
	}

}