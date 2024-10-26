
using CodeToImageGenerator.Web.Db;
using CodeToImageGenerator.Web.Models;

namespace CodeToImageGenerator.Web.Services
{
    public class StatisticsService(IStatisticsRepository repository) : IStatisticsService
    {
        private readonly IStatisticsRepository _repository = repository;

        public async Task<StatisticsViewModel> GetOverallStatisticsAsync()
        {
            var statistics = await _repository.GetAll();

            return new StatisticsViewModel
            {
                TotalRequests = statistics.Count,
                SuccessfulRequests = statistics.Count(s => s.IsSuccessful),
                BySource = statistics.GroupBy(s => s.Source)
                    .Select(g => new SourceStatistics
                    {
                        Source = g.Key,
                        Count = g.Count()
                    }).ToList(),
                ByLanguage = statistics.GroupBy(s => s.ProgrammingLanguage)
                    .Select(g => new LanguageStatistics
                    {
                        Language = g.Key,
                        Count = g.Count()
                    }).ToList()
            };

        }

        public async Task<StatisticsViewModel> GetStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            
            var statistics = await _repository.GetByPeriod(startDate, endDate);

            return new StatisticsViewModel
            {
                TotalRequests = statistics.Count,
                SuccessfulRequests = statistics.Count(s => s.IsSuccessful),
                BySource = statistics.GroupBy(s => s.Source)
                    .Select(g => new SourceStatistics
                    {
                        Source = g.Key,
                        Count = g.Count()
                    }).ToList(),
                ByLanguage = statistics.GroupBy(s => s.ProgrammingLanguage)
                    .Select(g => new LanguageStatistics
                    {
                        Language = g.Key,
                        Count = g.Count()
                    }).ToList()
            };
        }

        public async Task LogGenerationAttemptAsync(string source, string programmingLanguage, bool isSuccessful)
        {
            var statistic = new StatisticEntry
            {
                Timestamp = DateTime.UtcNow,
                Source = source,
                ProgrammingLanguage = programmingLanguage,
                IsSuccessful = isSuccessful
            };

            await _repository.Save(statistic);
        }
    }
}
