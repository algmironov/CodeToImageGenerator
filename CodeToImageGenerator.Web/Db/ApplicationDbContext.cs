using CodeToImageGenerator.Web.Models;

using Microsoft.EntityFrameworkCore;

namespace CodeToImageGenerator.Web.Db
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<StatisticEntry> Statistics { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
