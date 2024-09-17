using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CodeToImageGenerator.Web.Services
{
    public class TelegramBotService : IHostedService
    {
        private readonly TelegramBotClient _botClient;
        private readonly ILogger<TelegramBotService> _logger;

        public TelegramBotService(ILogger<TelegramBotService> logger)
        {
            _logger = logger;
            var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(1)
            };
            _botClient = new TelegramBotClient("7427295338:AAGX9JBYNfR0Y_haYva5k-w10AC19osodmU", httpClient);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                new ReceiverOptions(),
                cancellationToken
            );

            Console.WriteLine("Bot started...");
            return Task.CompletedTask;
        }

        public async Task SendImageFromCodeAsync(long chatId, string language, string code)
        {
            var image = await GenerateImageFromCode(language, code);
            var inputFile = new InputFileStream(image);
            await _botClient.SendPhotoAsync(chatId, photo: inputFile, caption: "Вот ваш результат!");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Bot stopped...");
            return Task.CompletedTask;
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update.Message!.Text != null)
            {
                await HandleMessageAsync(update.Message);
            }
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }

        // Логика обработки команды /start и отправки формы
        private async Task HandleMessageAsync(Message message)
        {
            if (message.Text == "/start")
            {
                var chatId = message.Chat.Id;

                var webApp = new WebAppInfo
                {
                    Url = @$"https://codepic.algmironov.com/home/index?chatId={chatId}"
                };

                try
                {
                    await _botClient.SendTextMessageAsync(chatId, "Привет! Чтобы отправить код, откройте форму", replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithWebApp("Открыть форму", webApp)));
                }
                catch (Exception ex) 
                {
                    Console.WriteLine(ex);
                }
                
            }
        }

        private async Task<Stream> GenerateImageFromCode(string language, string code)
        {
            var img = await Common.ImageGenerator.GenerateStream(language, code);
            //await Task.Delay(5000);
            return img; 
        }
    }
}