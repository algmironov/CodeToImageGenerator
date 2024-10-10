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
        private readonly IImageService _imageService;
        private readonly string _miniAppUrl;
        private readonly string _webHookUrl;
        private const string WelcomeMessage = "Привет! Для получения картинки с Вашим кодом откройте мини-приложение";

        public TelegramBotService(ILogger<TelegramBotService> logger, IImageService imageService, string botToken, string miniAppUrl, string webHookUrl)
        {
            ArgumentNullException.ThrowIfNull(botToken, nameof(botToken));
            ArgumentNullException.ThrowIfNull(miniAppUrl, nameof(miniAppUrl));
            ArgumentNullException.ThrowIfNull(webHookUrl, nameof(webHookUrl));

            _webHookUrl = webHookUrl;

            _miniAppUrl = miniAppUrl;

            _logger = logger;

            var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(1)
            };
            _imageService = imageService;

            _botClient = new TelegramBotClient(botToken, httpClient);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _botClient.SetWebhookAsync(
                url: _webHookUrl,
                cancellationToken: cancellationToken
                );

            var info = await _botClient.GetWebhookInfoAsync();
            

            _logger.LogInformation("WebHook info: {info.LastErrorMessage}, webhook url: {info.Url}", info.LastErrorMessage, info.Url);

            //_botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, cancellationToken: cancellationToken);

            //_botClient.StartReceiving(
            //    HandleUpdateAsync,
            //    HandleErrorAsync,
            //    new ReceiverOptions(),
            //    cancellationToken
            //);

            _logger.LogInformation("Bot started...");
        }

        public async Task SendImageFromCodeAsync(long chatId, string language, string code)
        {
            var image = await _imageService.GenerateImageFromCodeAsync(language, code);

            var fileName = _imageService.GenerateFileName(language);

            var inputFile = new InputFileStream(image, fileName);

            var keyboard = GetWebAppKeyboard(chatId);

            await _botClient.SendPhotoAsync(chatId: chatId,
                                            photo: inputFile,
                                            caption: $"Ваше изображение с кодом на языке {language}",
                                            replyMarkup: keyboard);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Bot stopped...");
            _botClient.DeleteWebhookAsync();
            return Task.CompletedTask;
        }

        public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
        {
            await HandleUpdateAsync(_botClient, update, cancellationToken);
        }

        public async Task HandleErrorAsync(Exception exception, CancellationToken cancellationToken)
        {
            await HandleErrorAsync(_botClient, exception, cancellationToken);
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

            _logger.LogError("Возникла ошибка: {errorMessage}", errorMessage);
            return Task.CompletedTask;
        }

        private async Task HandleMessageAsync(Message message)
        {
            if (message.Text == "/start")
            {
                var chatId = message.Chat.Id;

                var keyboard = GetWebAppKeyboard(chatId);
                try
                {
                    await _botClient.SendTextMessageAsync(chatId, 
                                                            WelcomeMessage, 
                                                            replyMarkup: keyboard);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    await _botClient.DeleteMessageAsync(chatId, messageId: message.MessageId);
                }
            }
        }

        private IReplyMarkup GetWebAppKeyboard(long chatId)
        {

            var webApp = new WebAppInfo
            {
                Url = $"{_miniAppUrl}"
            };

            return new InlineKeyboardMarkup(InlineKeyboardButton.WithWebApp("Открыть приложение", webApp));

        }
    }
}