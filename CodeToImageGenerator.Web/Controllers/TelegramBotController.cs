using CodeToImageGenerator.Web.Models;

using Microsoft.AspNetCore.Mvc;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CodeToImageGenerator.Web.Controllers
{
    /// <summary>
    ////Этот контроллер в будущих итерациях будет использоваться для работу бота через WebHook
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TelegramBotController : ControllerBase
    {
        private readonly ITelegramBotClient _botClient;

        public TelegramBotController(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] Update update)
        {
            if (update.Type == UpdateType.Message && update.Message?.Text != null)
            {
                await HandleMessage(update.Message);
            }
            else if (update.Type == UpdateType.MyChatMember)
            {
                // Обработка изменений статуса бота в чате
            }
            return Ok();
        }

        private async Task HandleMessage(Message message)
        {
            if (message.Text.StartsWith("/start"))
            {
                var keyboard = new InlineKeyboardMarkup(
                    InlineKeyboardButton.WithWebApp("Открыть форму",
                        new WebAppInfo { Url = "https://your-web-app-url.com" }));

                await _botClient.SendTextMessageAsync(message.Chat.Id,
                    "Привет! Нажми на кнопку, чтобы открыть форму для отправки кода.",
                    replyMarkup: keyboard);
            }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] Update update)
        {
            if (update.Type == UpdateType.Message && update.Message?.WebAppData != null)
            {
                var data = System.Text.Json.JsonSerializer.Deserialize<CodeSubmission>(update.Message.WebAppData.Data);

                // Здесь можно обработать полученные данные
                await _botClient.SendTextMessageAsync(update.Message.Chat.Id,
                    $"Получен код на языке {data.ProgrammingLanguage}:\n\n{data.Code}");
            }
            return Ok();
        }
    }
}
