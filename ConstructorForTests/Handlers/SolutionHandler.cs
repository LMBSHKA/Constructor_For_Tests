using ConstructorForTests.Dtos;
using ConstructorForTests.Handlers;
using ConstructorForTests.Models;
using Microsoft.EntityFrameworkCore;

namespace ConstructorForTests.UserSolutionHandler
{
	/// <summary>
	/// Содержится логика приложения, обработка ответов на тест от пользователя
	/// </summary>
	
	// Да не красиво не чисто написано, но рефакторинг начат, но могу не успеть его доделать
	//Переделываю под паттерн репозиторий и сервис
	public class SolutionHandler : ISolutionHandler
	{
		public async Task<decimal> CheckMultipleAnswer(UserAnswersDto userAnswer, 
			List<string> correctMultipleAnswer, decimal questionMark)
		{
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
					return questionMark;
			}

			return 0;
		}

		public async Task<decimal> CheckPairAnswer(UserAnswersDto userAnswer, 
			List<MatchingPair> correctPairsAnswer, decimal questionMark)
		{
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
					return questionMark;
			}

			return 0;
		}
	}
}
