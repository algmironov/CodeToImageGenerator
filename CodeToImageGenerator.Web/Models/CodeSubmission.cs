namespace CodeToImageGenerator.Web.Models
{
    public class CodeSubmission
    {
        public string? ProgrammingLanguage { get; set; }
        public string? Code { get; set; }
        public long ChatId { get; set; }

        public override string ToString()
        {
            return $"{ProgrammingLanguage} {Code} {ChatId}";
        }
    }
}
