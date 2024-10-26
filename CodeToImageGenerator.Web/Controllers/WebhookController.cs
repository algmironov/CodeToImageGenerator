using CodeToImageGenerator.Web.Services;

using Microsoft.AspNetCore.Mvc;

using Telegram.Bot.Types;

namespace CodeToImageGenerator.Web.Controllers
{
    /// <summary>
    ////Этот контроллер используется для работы бота через WebHook
    /// </summary>
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly TelegramBotService _botService;
        private readonly ILogger _logger;

        public WebhookController(TelegramBotService botService, ILogger<WebhookController> logger)
        {
            _botService = botService;
            _logger = logger;
        }

        [HttpPost("/webhook/update")]
        public async Task<IActionResult> Update([FromBody] Update update)
        {

            try
            {
                await _botService.HandleUpdateAsync(update, cancellationToken: CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при обработке обновления: {ex}", ex);
                await _botService.HandleErrorAsync(ex, cancellationToken: CancellationToken.None);
            }
            
            return Ok();
        }
    }
}
