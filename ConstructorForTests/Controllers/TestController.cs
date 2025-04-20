using ConstructorForTests.Dtos;
using ConstructorForTests.Models;
using ConstructorForTests.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ConstructorForTests.Controllers
{
	[ApiController]
	[Route("api/v1/operationsOnTest")]
	public class TestController : ControllerBase
	{
		private readonly ITestRepo _testRepo;

		public TestController(ITestRepo testRepo)
		{
			_testRepo = testRepo;
		}

		[HttpGet("GetAllTests")]
		public IActionResult GetAllTests()
		{
			return Ok(_testRepo.GetAllTests().Result);
		}

		[HttpGet("GetTest/{id}")]
		public IActionResult GetTestById(Guid id)
		{
			var test = _testRepo.GetTestById(id).Result;

			if (test == null)
				return NotFound();

			return Ok(test);
		}

		[HttpPost("CreateTest")]
		public IActionResult CreateTest([FromBody] GetOrCreateTestDto createTestData)
		{
			if (_testRepo.CreateTest(createTestData).Result)
				return Ok();

			return BadRequest("Something went wrong");
		}

		[HttpPut("Update/{id}")]
		public IActionResult UpdateTest(Guid id, [FromBody] Test updateTestData)
		{
			if (_testRepo.UpdateTest(id, updateTestData).Result)
				return Ok();

			return BadRequest("Something went wrong");
		}
	}

}