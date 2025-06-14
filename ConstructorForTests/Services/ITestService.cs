﻿using ConstructorForTests.Dtos;
using ConstructorForTests.Models;

namespace ConstructorForTests.Services
{
	public interface ITestService
	{
		Task<SendTestDTO?> GetTest(Guid testId, bool isCurator);
		string CalculateTimer(int timerInSeconds, string startTimer);
		Task<List<Test>> GetAllTests(string curatorId);
		Task<bool> CreateTest(CreateTestDto createTestData, string curatorId);
	}
}
