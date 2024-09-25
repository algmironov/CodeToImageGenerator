using CodeToImageGenerator.Web.Models;

namespace CodeToImageGenerator.Web.Services
{
    public static class HttpContextTelegramExtensions
    {
        private static readonly string TelegramDataKey = "TelegramMiniApp_Data";

        public static void SetTelegramData(this HttpContext context, TelegramMiniAppData data)
        {
            context.Items[TelegramDataKey] = data;
        }

        public static TelegramMiniAppData GetTelegramData(this HttpContext context)
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
