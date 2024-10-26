
using CodeToImageGenerator.Web.Models;

using Microsoft.EntityFrameworkCore;

namespace CodeToImageGenerator.Web.Db
{
    public class StatisticsRepository(ApplicationDbContext context) : IStatisticsRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<bool> Delete(StatisticEntry entry)
        {
            try
            {
                _context.Statistics.Remove(entry);
                await _context.SaveChangesAsync();
                return await Task.FromResult(true);
            }
            catch (Exception)
            {
                return await Task.FromResult(false);
            }
        }

        public async Task<List<StatisticEntry>> GetAll()
        {
            return await _context.Statistics.ToListAsync();
        }

        public async Task<List<StatisticEntry>> GetByPeriod(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Statistics.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(s => s.Timestamp >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(s => s.Timestamp <= endDate.Value);

            var statistics = await query.ToListAsync();
            return statistics;
        }

        public async Task Save(StatisticEntry entry)
        {
            await _context.Statistics.AddAsync(entry);
            await _context.SaveChangesAsync();
        }
    }
}
