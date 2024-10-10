
using Telegram.Bot;

namespace CodeToImageGenerator.Web.Services
{
    public class WebHookConfigurator : IHostedService
    {
        private readonly ILogger _logger;
        private readonly ITelegramBotClient _client;


        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
