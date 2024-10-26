using CodeToImageGenerator.Web.Models;

namespace CodeToImageGenerator.Web.Db
{
    public interface IStatisticsRepository
    {
        Task Save(StatisticEntry entry);
        Task<bool> Delete(StatisticEntry entry);
        Task<List<StatisticEntry>> GetAll();
        Task<List<StatisticEntry>> GetByPeriod(DateTime? startDate = null, DateTime? endDate = null);
    }
}
