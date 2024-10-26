namespace CodeToImageGenerator.Web.Models
{
    public class StatisticEntry
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string? Source { get; set; }
        public string? ProgrammingLanguage { get; set; }
        public bool IsSuccessful { get; set; }
    }
}
