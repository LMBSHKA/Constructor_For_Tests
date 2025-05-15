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

		public UserRepo(AppDbContext context, ISolutionHandler solutionHandler)
		{
			_solutionHandler = solutionHandler;
			_context = context;
		}

		public async Task<decimal> CheckUserAnswers(UserSolutionDto userSolution, Guid testId, Guid userId)
		{
			decimal score = 0;

			var correctAnswers = await _context.Answers
				.Where(x => x.TestId == testId)
				.ToListAsync();

			var questions = await _context.Questions
				.Where(x => x.TestId == testId)
				.ToListAsync();
			
			foreach (var userAnswer in userSolution.Answers)
			{
				score += await CheckAnswer(correctAnswers, questions, userAnswer);
			}

			await CreateTestResult(testId, userId, score);

			return score;
		}

		public async Task<Guid> CreateUser(User newUser)
		{
			await _context.Users.AddAsync(newUser);
			await _context.SaveChangesAsync();

			return newUser.Id;
		}

		private async Task CreateTestResult(Guid testId, Guid userId, decimal score)
		{
			var isPassed = await CheckPassage(testId, score);
			var testResult = new TestResult(testId, userId, score, isPassed);

			await _context.TestResults.AddAsync(testResult);
			await _context.SaveChangesAsync();
		}

		private async Task<bool> CheckPassage(Guid testId, decimal score)
		{
			var test = await _context.Tests.FirstOrDefaultAsync(x => x.Id == testId) ?? throw new Exception("Invalid test id");

			if (test.ScoreToPass <= score)
				return true;

			else
				return false;
		}

		private async Task<decimal> CheckAnswer(List<Answer> correctAnswers, List<Question> questions, UserAnswersDto userAnswer)
		{
			var correctAnswer = correctAnswers.FirstOrDefault(x => x.QuestionId == userAnswer.QuestionId);

			if (!string.IsNullOrEmpty(correctAnswer.TextAnswer))
			{
				if (correctAnswer.TextAnswer == userAnswer.TextAnswer)
				{
					return questions.FirstOrDefault(x => x.Id == correctAnswer.QuestionId).Mark;
				}
			}

			else if (correctAnswer.MultipleAnswerId != Guid.Empty)
			{
				var correctMultipleAnswer = await _context.MultipleChoices
					.Where(x => x.MultipleAnswerId == correctAnswer.MultipleAnswerId)
					.Select(x => x.Answer)
					.ToListAsync();

				return await _solutionHandler.CheckMultipleAnswer(correctAnswer, userAnswer, questions, correctMultipleAnswer);
			}

			else if (correctAnswer.PairId != Guid.Empty)
			{
				var correctPairsAnswer = await _context.MatchingPairs
					.Where(x => x.PairId == correctAnswer.PairId)
					.ToListAsync();

				return await _solutionHandler.CheckPairAnswer(correctAnswer, userAnswer, questions, correctPairsAnswer);
			}

			return 0;
		}
	}
}