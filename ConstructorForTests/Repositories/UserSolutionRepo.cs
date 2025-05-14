using ConstructorForTests.Database;
using ConstructorForTests.Dtos;
using ConstructorForTests.Models;
using Microsoft.EntityFrameworkCore;

namespace ConstructorForTests.Repositories
{
	public class UserSolutionRepo : IUserSolutionRepo
	{
		private readonly AppDbContext _context;

		public UserSolutionRepo( AppDbContext context )
		{
			_context = context;
		}

		public async Task<decimal> CheckUserAnswers(List<UserAnswersDto> userSolution, Guid testId)
		{
			decimal score = 0;
			var correctAnswers = await _context.Answers
				.Where(x => x.TestId == testId)
				.ToListAsync();

			var questions = await _context.Questions
				.Where(x => x.TestId == testId)
				.ToListAsync();
			
			foreach (var userAnswer in userSolution)
			{
				var correctAnswer = correctAnswers
					.FirstOrDefault(x => x.QuestionId == userAnswer.QuestionId);
				
				if (!string.IsNullOrEmpty(correctAnswer.TextAnswer))
				{
					if (correctAnswer.TextAnswer == userAnswer.TextAnswer)
					{
						score += questions.FirstOrDefault(x => x.Id == correctAnswer.QuestionId).Mark;
					}
				}

				else if (correctAnswer.MultipleAnswerId != Guid.Empty)
				{
					score += await CheckMultipleAnswer(correctAnswer, userAnswer, questions);
				}

				else if (correctAnswer.PairId != Guid.Empty)
				{
					score += await CheckPairAnswer(correctAnswer, userAnswer, questions);
				}
			}

			return score;
		}

		private async Task<decimal> CheckMultipleAnswer(Answer correctAnswer, UserAnswersDto userAnswer, 
			List<Question> questions)
		{
			decimal score = 0;

			var correctMultipleAnswer = await _context.MultipleChoices
						.Where(x => x.MultipleAnswerId == correctAnswer.MultipleAnswerId)
						.Select(x => x.Answer)
						.ToListAsync();
			if (correctMultipleAnswer.Count == userAnswer.MultipleAnswer.Count)
			{
				var flag = true;
				foreach (var userSingleAnswer in userAnswer.MultipleAnswer)
				{
					if (!correctMultipleAnswer.Contains(userSingleAnswer))
					{
						flag = false;
						break;
					}
				}

				if (flag)
					score += questions.FirstOrDefault(x => x.Id == correctAnswer.QuestionId).Mark;
			}

			return score;
		}

		private async Task<decimal> CheckPairAnswer(Answer correctAnswer, UserAnswersDto userAnswer, 
			List<Question> questions)
		{
			decimal score = 0;

			var correctPairsAnswer = await _context.MatchingPairs
						.Where(x => x.PairId == correctAnswer.PairId)
						.ToListAsync();

			if (correctPairsAnswer.Count == userAnswer.MatchingPairs.Count)
			{
				var flag = true;
				foreach (var correctPair in correctPairsAnswer)
				{
					if (!userAnswer.MatchingPairs
						.Contains(new KeyValuePair<string, string>(correctPair.PairKey, correctPair.PairValue)))
					{
						flag = false;
						break;
					}
				}

				if (flag)
					score += questions
						.FirstOrDefault(x => x.Id == correctAnswer.QuestionId).Mark;
			}

			return score;
		}
	}
}
