namespace CodeToImageGenerator.Web.Models
{
    public class PageViewModel
    {
        public bool IsFromTelegram { get; set; } = false;

        private CodeSubmission? _codeSubmission;
        public CodeSubmission? CodeSubmission 
        {
            get => _codeSubmission; 
            set
            {
                if (value?.ChatId.HasValue == true) 
                {
                    IsFromTelegram = true;
                }
                _codeSubmission = value;
            } 
        }

        public override string ToString()
        {
            return $"Is from Telegram: {IsFromTelegram} {Environment.NewLine} {CodeSubmission}";
        }
    }
}
