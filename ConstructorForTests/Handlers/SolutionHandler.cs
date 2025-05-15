using ConstructorForTests.Dtos;
using ConstructorForTests.Handlers;
using ConstructorForTests.Models;
using Microsoft.EntityFrameworkCore;

namespace ConstructorForTests.UserSolutionHandler
{
	public class SolutionHandler : ISolutionHandler
	{
		public async Task<decimal> CheckMultipleAnswer(Answer correctAnswer, UserAnswersDto userAnswer,
			List<Question> questions, List<string> correctMultipleAnswer)
		{
			decimal score = 0;

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

		public async Task<decimal> CheckPairAnswer(Answer correctAnswer, UserAnswersDto userAnswer,
			List<Question> questions, List<MatchingPair> correctPairsAnswer)
		{
			decimal score = 0;

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
