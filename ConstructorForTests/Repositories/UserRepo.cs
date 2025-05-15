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

		public async Task<decimal> CheckUserAnswers(UserSolutionDto userSolution, Guid testId)
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

			return score;
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