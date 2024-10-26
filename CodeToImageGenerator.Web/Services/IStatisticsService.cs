using CodeToImageGenerator.Web.Models;

namespace CodeToImageGenerator.Web.Services
{
    public interface IStatisticsService
    {
        Task LogGenerationAttemptAsync(string source, string programmingLanguage, bool isSuccessful);
        Task<StatisticsViewModel> GetStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<StatisticsViewModel> GetOverallStatisticsAsync();
    }
}
