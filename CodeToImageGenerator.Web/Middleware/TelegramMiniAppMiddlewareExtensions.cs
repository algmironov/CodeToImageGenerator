namespace CodeToImageGenerator.Web.Middleware
{
    public static class TelegramMiniAppMiddlewareExtensions
    {
        public static IApplicationBuilder UseTelegramMiniAppMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TelegramMiniAppMiddleware>();
        }
    }
}
