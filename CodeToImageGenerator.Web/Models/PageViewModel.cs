namespace CodeToImageGenerator.Web.Models
{
    public class PageViewModel
    {
        public bool IsFromTelegram { get; set; }
        public CodeSubmission CodeSubmission { get; set; }

        public override string ToString()
        {
            return $"Is from Telegram: {IsFromTelegram} {Environment.NewLine} {CodeSubmission}";
        }
    }
}
