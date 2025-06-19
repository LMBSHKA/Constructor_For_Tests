using ConstructorForTests.Dtos;
using ConstructorForTests.Handlers;
using ConstructorForTests.Models;
using ConstructorForTests.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ConstructorForTests.Services
{
	public class TestService : ITestService
	{
		private readonly ITestRepo _testRepo;

		public TestService(ITestRepo testRepo)
		{
			_testRepo = testRepo;
		}

		public async Task<List<Test>> GetAllTests(string curatorId)
		{
			var allTests = _testRepo.GetAllTests();
			var testsByUserId = await allTests
				.Where(x => x.UserId == curatorId)
				.ToListAsync();

			return testsByUserId;
		}

		public async Task<SendTestDTO?> GetTest(Guid testId, bool isCurator)
		{
			var test = await _testRepo.GetTestInfoById(testId);

			if (test == null || (test.IsActive == false && isCurator == false))
				return null;

			var questions = await _testRepo.GetTestQuestion(testId)
				.ToListAsync();

			var listGetQuestions = new List<BaseQuestionDto>();

			CreateListBaseQuestionDto(listGetQuestions, questions);

			return new SendTestDTO(test, listGetQuestions);
		}

		public async Task<TestForEditingDto> GetTestForEditing(Guid testId)
		{
			var test = await _testRepo.GetTestInfoById(testId);
			if (test == null && test.IsActive == true)
				return null;

			var questions = await _testRepo.GetTestQuestion(testId)
				.ToListAsync();

			var listGetQuestions = new List<BaseQuestionDto>();

			CreateListBaseQuestionDto(listGetQuestions, questions);

			return new TestForEditingDto(test, listGetQuestions);
		}

		private static void CreateListBaseQuestionDto(List<BaseQuestionDto> listGetQuestions, List<Question> questions)
		{
			foreach (var question in questions)
			{
				BaseQuestionDto questionDto;

				if (!string.IsNullOrEmpty(question.AnswerOptions))
				{
					questionDto = new QuestionWithOptionsDto(question, new AllAnswerDto(question.AnswerOptions.Split(' ')));
				}

				else if (!string.IsNullOrEmpty(question.PairKey) && !string.IsNullOrEmpty(question.PairValue))
				{
					questionDto = new QuestionWithPairDto(question, 
						new AllAnswerDto(question.PairKey.Split(' '), question.PairValue.Split(' ')));
				}

				else
				{
					questionDto = new SimpleQuestionDto(question);
				}

				listGetQuestions.Add(questionDto);
			}
		}

		public async Task SetCorrectAnswersForQuestions(List<BaseQuestionDto> listQuestions)
		{
			foreach (var question in listQuestions)
			{
				var correctAnswer = await _testRepo.GetQuestionsCorrectAnswers(question.Id);
				if (correctAnswer != null)
				{
					if (correctAnswer != null && question is SimpleQuestionDto simpleQuestion)
					{
						var answer = new AllAnswerDto([correctAnswer.TextAnswer]);
						simpleQuestion.CorrectAnswers = answer;
					}
					else if (correctAnswer != null && question is QuestionWithOptionsDto questionWithOptions)
					{
						var multipleAnswer = await _testRepo.GetMultipleChoice(correctAnswer.MultipleAnswerId)
							.Select(x => x.Answer)
							.ToArrayAsync();
						var answers = new AllAnswerDto(multipleAnswer!);
						questionWithOptions.CorrectAnswers = answers;
					}
					else if (correctAnswer != null && question is QuestionWithPairDto questionWithPair)
					{
						var pairs = await _testRepo.GetMatchingPair(correctAnswer.PairId)
							.ToArrayAsync();
						var pairKey = pairs.Select(x => x.PairKey).ToArray();
						var pairValue = pairs.Select(x => x.PairValue).ToArray();
						var answers = new AllAnswerDto(pairKey, pairValue);
						questionWithPair.CorrectAnswers = answers;
					}
				}
			}
		}

		public string CalculateTimer(int timerInSeconds, string startTimer)
		{
			var passedTime = TimeSpan.Parse(DateTime.Now.ToLongTimeString()).Subtract(TimeSpan.Parse(startTimer!));
			var convertedTimer = TimeSpan.FromSeconds(Convert.ToDouble(timerInSeconds));
			var remainingTime = convertedTimer.Subtract(passedTime).ToString();
			var remainingTimeSeconds = TimeSpan.Parse(remainingTime).TotalSeconds.ToString();

			return remainingTimeSeconds;
		}

		public async Task<bool> CreateTest(CreateTestDto createTestData, string curatorId)
		{
			if (createTestData.Title == null)
				return false;

			var isActive = false;
			if (createTestData.StartAt <= DateTime.Now)
				isActive = true;

			var newTest = new Test(createTestData, isActive, false, curatorId!);
			await _testRepo.CreateTestInfo(newTest);
			await AddQuestions(createTestData.Questions!, newTest);
			await _testRepo.SaveChecnhesAsync();

			return true;
		}

		private async Task AddQuestions(List<CreateQuestionDTO> questions, Test test)
		{
			var order = 1;
			decimal questionMark = 1;
			foreach (var question in questions)
			{
				var newQuestion = new Question(test.Id, question, order, questionMark);
				CheckForManualCheck(test, question.Type);

				await _testRepo.AddQuestion(newQuestion);
				await AddAnswer(newQuestion.Id, question.CreateAnswer, test.Id);
				order++;
			}
		}

		private static void CheckForManualCheck(Test test, QuestionType questionType)
		{
			if (questionType == QuestionType.DetailedAnswer)
				test.ManualCheck = true;
		}

		private async Task AddAnswer(Guid questionId, AnswerDTO answer, Guid testId)
		{
			if (!string.IsNullOrEmpty(answer.TextAnswer))
			{
				var newAnswer = new Answer(questionId, Guid.Empty, Guid.Empty, answer.TextAnswer, testId);
				await _testRepo.AddAnswer(newAnswer);
			}

			else if (answer.MultipleAnswer.Count > 0)
			{
				await AddMultipleAnswer(answer.MultipleAnswer, testId, questionId);
			}

			else if (answer.MatchingPairs.Count > 0)
			{
				await AddPairAnswer(answer.MatchingPairs, testId, questionId);
			}
		}

		private async Task AddMultipleAnswer(List<string> multipleAnswers, Guid testId, Guid questionId)
		{
			var guid = Guid.NewGuid();
			var newAnswer = new Answer(questionId, guid, Guid.Empty, string.Empty, testId);
			await _testRepo.AddAnswer(newAnswer);

			foreach (var singleAnswer in multipleAnswers)
			{
				var newSingleAnswer = new MultipleChoice(guid, singleAnswer);
				await _testRepo.AddSingleSolutionForMultipleAnswer(newSingleAnswer);
			}
		}

		private async Task AddPairAnswer(Dictionary<string, string> pairAnswers, Guid testId, Guid questionId)
		{
			var guid = Guid.NewGuid();
			var newAnswer = new Answer(questionId, Guid.Empty, guid, string.Empty, testId);
			await _testRepo.AddAnswer(newAnswer);

			foreach (var answer in pairAnswers)
			{
				var pairAnswer = new MatchingPair(guid, answer.Key, answer.Value);
				await _testRepo.AddPairAnswer(pairAnswer);
			}
		}
	}
}
