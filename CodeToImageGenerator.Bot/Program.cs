
using SimpleLogger;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace CodeToImageGenerator.Bot
{
    class Program
    {
        private static readonly Logger _logger = new();
        static async Task Main(string[] args)
        {
            var telegramBotClient = new TelegramBotClient("7427295338:AAGX9JBYNfR0Y_haYva5k-w10AC19osodmU");
            var me = await telegramBotClient.GetMeAsync();

            _logger.Info($"Bot has started at {DateTime.Now} with ID = {me.Id}");

            try
            {
                telegramBotClient.StartReceiving(updateHandler: HandleUpdates, pollingErrorHandler: HandleErrors);
            }
            catch (Exception ex)
            {
                _logger.Error("Ошибка во время работы бота", ex);
            }

            Console.ReadLine();
        }

        private static async Task HandleErrors(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            var help = exception.HelpLink;
            var consoleMessage = $"Возникла ошибка: {exception.Message}\n{help}";
            _logger.Error($"{consoleMessage}", exception);
        }

        private static async Task HandleUpdates(ITelegramBotClient client, Update update, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
