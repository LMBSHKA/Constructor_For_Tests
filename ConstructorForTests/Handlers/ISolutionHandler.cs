using ConstructorForTests.Dtos;
using ConstructorForTests.Models;

namespace ConstructorForTests.Handlers
{
	public interface ISolutionHandler
	{
		Task<decimal> CheckMultipleAnswer(UserAnswersDto userAnswer, List<string> correctMultipleAnswer);
		Task<decimal> CheckPairAnswer(UserAnswersDto userAnswer, List<MatchingPair> correctPairsAnswer);
	}
}
