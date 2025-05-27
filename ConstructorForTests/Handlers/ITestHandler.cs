using ConstructorForTests.Dtos;
using ConstructorForTests.Models;

namespace ConstructorForTests.Handlers
{
	public interface ITestHandler
	{
		void GetTestById(List<BaseQuestionDto> listGetQuestions, List<Question> questions);
	}
}
