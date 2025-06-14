using ConstructorForTests.Models;
using ConstructorForTests.Repositories;
using ConstructorForTests.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using static System.Net.Mime.MediaTypeNames;

namespace ConstructorForTests.Filters
{
	public class SetTimerAttribute : Attribute, IAsyncActionFilter
	{
		private readonly ITestRepo _testRepo;
		private readonly ITestService _testService;

		public SetTimerAttribute(ITestRepo testRepo, ITestService testService)
		{
			_testRepo = testRepo;
			_testService = testService;
		}

		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var testId = (Guid)context.ActionArguments["id"]!;
			var testInfo = await _testRepo.GetTestInfoById(testId);
			var httpContext = context.HttpContext;

			if (httpContext.Session.GetString("StartTime") == null)
			{
				httpContext.Response.Headers
					.Add(new KeyValuePair<string, StringValues>("Remaining-Time", testInfo!.TimerInSeconds!.ToString()));
				httpContext.Session.SetString("StartTime", DateTime.Now.ToLongTimeString());
			}

			else
			{
				var startTimer = httpContext.Session.GetString("StartTime");
				var remainingTime = _testService.CalculateTimer(int.Parse(testInfo!.TimerInSeconds!), startTimer!);
				httpContext.Response.Headers.Add(new KeyValuePair<string, StringValues>("Remaining-Time", remainingTime));
			}

			await next();
		}
	}
}
