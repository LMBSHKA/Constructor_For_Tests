using ConstructorForTests.Dtos;
using ConstructorForTests.Models;

namespace ConstructorForTests.Handlers
{
	public interface ISolutionHandler
	{
		Task<decimal> CheckMultipleAnswer(Answer correctAnswer, UserAnswersDto userAnswer,
			List<Question> questions, List<string> correctMultipleAnswer);
		Task<decimal> CheckPairAnswer(Answer correctAnswer, UserAnswersDto userAnswer,
			List<Question> questions, List<MatchingPair> correctPairsAnswer);
	}
}
