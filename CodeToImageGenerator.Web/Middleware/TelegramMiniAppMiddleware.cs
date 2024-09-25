using System.Text.Json;
using System.Web;

using CodeToImageGenerator.Web.Models;
using CodeToImageGenerator.Web.Services;

namespace CodeToImageGenerator.Web.Middleware
{
    public class TelegramMiniAppMiddleware
    {
        private readonly RequestDelegate _next;

        private const string TelegramDataKey = "TelegramMiniApp_Data";
        private readonly ILogger<TelegramMiniAppMiddleware> _logger;

        public TelegramMiniAppMiddleware(RequestDelegate next, ILogger<TelegramMiniAppMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("TelegramMiniAppMiddleware invoked");

            var request = context.Request;
            var originalQueryString = request.QueryString.Value;

            _logger.LogInformation($"Original QueryString: {originalQueryString}");

            if (originalQueryString != null && originalQueryString.Contains("tgWebAppData="))
            {
                var queryDictionary = HttpUtility.ParseQueryString(originalQueryString);
                var tgWebAppData = queryDictionary["tgWebAppData"];

                _logger.LogInformation($"tgWebAppData: {tgWebAppData}");

                if (!string.IsNullOrEmpty(tgWebAppData))
                {
                    var decodedData = HttpUtility.UrlDecode(tgWebAppData);
                    _logger.LogInformation($"Decoded data: {decodedData}");

                    try
                    {
                        var telegramData = JsonSerializer.Deserialize<TelegramMiniAppData>(decodedData);

                        // Set Telegram data using the extension method
                        context.SetTelegramData(telegramData);
                        _logger.LogInformation($"TelegramData set: {telegramData != null}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error deserializing Telegram data: {ex.Message}");
                    }
                }
            }

            await _next(context);
        }

        public static TelegramMiniAppData GetTelegramData(HttpContext context)
        {
            if (context.Items.TryGetValue(TelegramDataKey, out var telegramDataObj) &&
                telegramDataObj is TelegramMiniAppData telegramData)
            {
                return telegramData;
            }
            return null;
        }
    }
}
