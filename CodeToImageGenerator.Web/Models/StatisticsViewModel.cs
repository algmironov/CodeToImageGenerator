namespace CodeToImageGenerator.Web.Models
{
    public class StatisticsViewModel
    {
        public int TotalRequests { get; set; }
        public int SuccessfulRequests { get; set; }
        public List<SourceStatistics> BySource { get; set; }
        public List<LanguageStatistics> ByLanguage { get; set; }
    }
}
