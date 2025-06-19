using ConstructorForTests.API;
using ConstructorForTests.Database;
using ConstructorForTests.Dtos;
using ConstructorForTests.Handlers;
using ConstructorForTests.Models;
using Microsoft.EntityFrameworkCore;

namespace ConstructorForTests.Repositories
{
	public class UserRepo : IUserRepo
	{
		private readonly AppDbContext _context;
		private readonly ISolutionHandler _solutionHandler;
		private readonly IEmailSender _emailSender;

		public UserRepo(AppDbContext context, ISolutionHandler solutionHandler, IEmailSender emailSender)
		{
			_emailSender = emailSender;
			_solutionHandler = solutionHandler;
			_context = context;
		}

		public async Task<decimal> CheckUserAnswers(UserSolutionDto userSolution, Guid testId, Guid userId)
		{
			decimal score = 0;

			await SaveUserAnswer(userSolution.Answers, userId);

			var correctAnswers = await _context.Answers
				.Where(x => x.TestId == testId)
				.ToListAsync();

			var questions = await _context.Questions
				.Where(x => x.TestId == testId)
				.ToListAsync();

			var test = await _context.Tests.FirstOrDefaultAsync(x => x.Id == testId) ?? throw new Exception("Invalid test id");

			foreach (var userAnswer in userSolution.Answers)
			{
				var questionMark = questions.FirstOrDefault(x => x.Id == userAnswer.QuestionId)!.Mark;
				score += await CheckAnswer(correctAnswers, userAnswer, questionMark);
			}

			var isPassed = await CreateTestResult(test, userId, score);

			if (test.ManualCheck == false)
			{
				if (isPassed)
					await _emailSender.SendEmail(userSolution.Email, score, test.MessageAboutPassing);
				else
					await _emailSender.SendEmail(userSolution.Email, score, test.FailureMessage);
			}

			return score;
		}

		public async Task<Guid> CreateUser(User newUser)
		{
			await _context.Users.AddAsync(newUser);
			await _context.SaveChangesAsync();

			return newUser.Id;
		}

		private async Task<bool> CreateTestResult(Test test, Guid userId, decimal score)
		{
			var isPassed = CheckPassage(test, score);
			var testResult = new TestResult(test.Id, userId, score, isPassed);

			await _context.TestResults.AddAsync(testResult);
			await _context.SaveChangesAsync();

			return isPassed;
		}

		public bool CheckPassage(Test test, decimal score)
		{
			if (test.ScoreToPass <= score)
				return true;

			else
				return false;
		}

		private async Task<decimal> CheckAnswer(List<Answer> correctAnswers, UserAnswersDto userAnswer, decimal mark)
		{
			var correctAnswer = correctAnswers.FirstOrDefault(x => x.QuestionId == userAnswer.QuestionId);
			if (correctAnswer != null)
			{
				if (!string.IsNullOrEmpty(correctAnswer.TextAnswer))
				{
					if (correctAnswer.TextAnswer == userAnswer.TextAnswer)
					{
						return mark;
					}
				}

				else if (correctAnswer.MultipleAnswerId != Guid.Empty)
				{
					var correctMultipleAnswer = await _context.MultipleChoices
						.Where(x => x.MultipleAnswerId == correctAnswer.MultipleAnswerId)
						.Select(x => x.Answer)
						.ToListAsync();

					return await _solutionHandler.CheckMultipleAnswer(userAnswer, correctMultipleAnswer, mark);
				}

				else if (correctAnswer.PairId != Guid.Empty)
				{
					var correctPairsAnswer = await _context.MatchingPairs
						.Where(x => x.PairId == correctAnswer.PairId)
						.ToListAsync();

					return await _solutionHandler.CheckPairAnswer(userAnswer, correctPairsAnswer, mark);
				}
			}

			return 0;
		}

		private async Task SaveUserAnswer(List<UserAnswersDto> userAnswers, Guid userId)
		{
			foreach (var userAnswer in userAnswers)
			{
				if (!string.IsNullOrEmpty(userAnswer.TextAnswer))
				{
					await AddTextAnswer(userAnswer, userId);
				}

				else if (userAnswer.MultipleAnswer.Count > 0)
				{
					var guid = Guid.NewGuid();
					await SaveMultipleAnswer(userAnswer.MultipleAnswer, guid);
					var answer = new UserAnswer(userId, userAnswer.QuestionId, guid, Guid.Empty, string.Empty, false);
					await _context.UserAnswers.AddAsync(answer);
				}

				else if (userAnswer.MatchingPairs.Count > 0)
				{
					var guid = Guid.NewGuid();
					await SavePairAnswer(userAnswer.MatchingPairs, guid);
					var answer = new UserAnswer(userId, userAnswer.QuestionId, Guid.Empty, guid, string.Empty, false);
					await _context.UserAnswers.AddAsync(answer);
				}
			}
			await _context.SaveChangesAsync();
		}

		private async Task AddTextAnswer(UserAnswersDto userAnswer, Guid userId)
		{
			var question = await _context.Questions
				.FirstOrDefaultAsync(x => x.Id == userAnswer.QuestionId);
			if (question.Type == QuestionType.DetailedAnswer)
			{
				var answer = new UserAnswer(userId, userAnswer.QuestionId,
					Guid.Empty, Guid.Empty, userAnswer.TextAnswer, true);
				await _context.UserAnswers.AddAsync(answer);
			}
			else
			{
				var answer = new UserAnswer(userId, userAnswer.QuestionId,
					Guid.Empty, Guid.Empty, userAnswer.TextAnswer, false);
				await _context.UserAnswers.AddAsync(answer);

			}
		}
		private async Task SaveMultipleAnswer(List<string> multipleAnswers, Guid guid)
		{
			foreach (var multipleAnswer in multipleAnswers)
			{
				var answer = new UserMultipleChoice { MultipleAnswerId = guid, Answer =  multipleAnswer };
				await _context.UserMultipleChoices.AddAsync(answer);
			}
		}

		private async Task SavePairAnswer(Dictionary<string, string> pairAnswers, Guid guid)
		{
			foreach (var pairAnswer in pairAnswers) 
			{
				var answer = new UserMatchingPair { PairId = guid, PairKey = pairAnswer.Key, PairValue = pairAnswer.Value };
				await _context.UserMatchingPairs.AddAsync(answer);
			}
		}
	}
}