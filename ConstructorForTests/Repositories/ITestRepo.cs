using ConstructorForTests.Dtos;
using ConstructorForTests.Models;

namespace ConstructorForTests.Repositories
{
	//Нужен рефакторинг слишком большой интерфейс
	public interface ITestRepo
	{
		IQueryable<Test> GetAllTests();
		Task CreateTestInfo(Test test);
		IQueryable<TestResult> GetTestResult();
		Task AddQuestion(Question newQuestion);
		Task AddAnswer(Answer newAnswer);
		Task AddSingleSolutionForMultipleAnswer(MultipleChoice newSingleAnswer);
		Task<Answer> GetQuestionsCorrectAnswers(Guid questionId);
		IQueryable<MultipleChoice> GetMultipleChoice(Guid multipleAnswerId);
		IQueryable<MatchingPair> GetMatchingPair(Guid pairId);
		Task AddPairAnswer(MatchingPair pairAnswer);
		Task<bool> UpdateTest(Guid id, Test updateTestData);
		Task<Test?> GetTestInfoById(Guid id);
		IOrderedQueryable<Question> GetTestQuestion(Guid testId);
		Task<int> DeleteTest(Guid testId);
		Task<int> UpdateResult(Guid testId, List<ManualCheckDto> manualCheckData);
		Task<List<SendTestToCheckDto>> CreateTestDtoToCheck(Guid testId);
		Task SaveChecnhesAsync();
	}
}
