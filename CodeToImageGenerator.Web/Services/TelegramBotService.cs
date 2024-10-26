using Telegram.Bot;
using Telegram.Bot.Exceptions;
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

            var info = await _botClient.GetWebhookInfoAsync(cancellationToken: cancellationToken);
            

            _logger.LogInformation("WebHook info: {info.LastErrorMessage}, webhook url: {info.Url}", info.LastErrorMessage, info.Url);

            _logger.LogInformation("Bot started...");
        }

        public async Task SendImageFromCodeAsync(long chatId, string language, string code)
        {
            var image = await _imageService.GenerateImageFromCodeAsync(language, code);

            var fileName = _imageService.GenerateFileName(language);

            var inputFile = new InputFileStream(image, fileName);

            var keyboard = GetWebAppKeyboard();

            await _botClient.SendPhotoAsync(chatId: chatId,
                                            photo: inputFile,
                                            replyMarkup: keyboard);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Bot stopped...");
            await  _botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
        }

        public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update.Message!.Text != null)
            {
                await HandleMessageAsync(update.Message);
            }
        }

        public async Task HandleErrorAsync(Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            _logger.LogError("Возникла ошибка: {errorMessage}", errorMessage);
            
        }

        private async Task HandleMessageAsync(Message message)
        {
            if (message.Text == "/start")
            {
                var chatId = message.Chat.Id;

                var keyboard = GetWebAppKeyboard();
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

        private InlineKeyboardMarkup GetWebAppKeyboard()
        {

            var webApp = new WebAppInfo
            {
                Url = $"{_miniAppUrl}"
            };

            return new InlineKeyboardMarkup(InlineKeyboardButton.WithWebApp("Открыть приложение", webApp));

        }
    }
}