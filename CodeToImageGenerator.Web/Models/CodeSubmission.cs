namespace CodeToImageGenerator.Web.Models
{
    public class CodeSubmission
    {
        public string? ProgrammingLanguage { get; set; }
        public string? Code { get; set; }
        public long? ChatId { get; set; }

        public override string ToString()
        {
            if (ChatId != null) 
            {
                return $"language: {ProgrammingLanguage}{Environment.NewLine}code: {Code}{Environment.NewLine}User from Telegram, chatId: {ChatId}";
            }
            return $"language: {ProgrammingLanguage}{Environment.NewLine}code: {Code}{Environment.NewLine}User from Browser";
        }
    }
}
