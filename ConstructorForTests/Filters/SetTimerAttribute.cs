﻿using ConstructorForTests.Models;
using ConstructorForTests.Repositories;
using ConstructorForTests.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using static System.Net.Mime.MediaTypeNames;

namespace ConstructorForTests.Filters
{
	/// <summary>
	/// Атрибут утсанавливает таймер на тест
	/// </summary>
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
			var curatorId = httpContext.Session.GetString("CuratorId");

			if (string.IsNullOrEmpty(curatorId))
			{
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
			}

			await next();
		}
	}
}
