using LMP.Data;
using LMP.Models.KnowledgeTesting;

namespace Application.Infrastructure.TestQuestionPassingManagement
{
    public class TestQuestionPassingService : ITestQuestionPassingService
    {
        public void SaveTestQuestionPassResults(TestQuestionPassResults item)
        {
            using var repositoriesContainer = new LmPlatformRepositoriesContainer();

            if (item is { })
            {
                repositoriesContainer.TestQuestionPassResultsRepository.Save(item);
            }
            repositoriesContainer.ApplyChanges();
        }
    }
}
