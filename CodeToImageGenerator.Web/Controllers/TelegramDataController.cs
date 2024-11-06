using CodeToImageGenerator.Web.Models.DTO;
using CodeToImageGenerator.Web.Models;

using TelegramWebAppDataValidator;

using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Web;

namespace CodeToImageGenerator.Web.Controllers
{
    [ApiController]
    [Route("api/telegram-data")]
    public class TelegramDataController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] TelegramDataDto data)
        {
            Console.WriteLine("Received data in controller");
            if (string.IsNullOrEmpty(data.TgWebAppData))
            {
                return RedirectToAction("Index", "Home", new { isFromTelegram = false });
            }

            var IsValid = Validator.Validate(data.TgWebAppData, Environment.GetEnvironmentVariable("BOT_TOKEN"));

            var decodedData = HttpUtility.UrlDecode(data.TgWebAppData);
            Console.WriteLine($"Decoded data: {decodedData}");

            if (IsValid)
            {
                try
                {
                    var parsedData = ParseTelegramData(decodedData);
                    HttpContext.Session.SetString("TelegramData", JsonSerializer.Serialize(parsedData));

                    return RedirectToAction("Index", "Home", new { isFromTelegram = true });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing data: {ex.Message}");

                    return RedirectToAction("Index", "Home", new { isFromTelegram = false });
                }
            }

            return RedirectToAction("Index", "Home", new { isFromTelegram = false });

        }

        private TelegramMiniAppData ParseTelegramData(string data)
        {
            var pairs = data.Split('&');
            var result = new TelegramMiniAppData();
            var dict = new Dictionary<string, string>();

            try
            {
                foreach (var pair in pairs)
                {
                    var parts = pair.Split('=');
                    if (parts.Length == 2)
                    {
                        dict[parts[0]] = HttpUtility.UrlDecode(parts[1]);
                    }
                }

                result.QueryId = dict.GetValueOrDefault("query_id");
                result.AuthDate = long.TryParse(dict.GetValueOrDefault("auth_date"), out var authDate) ? authDate : 0;
                result.Hash = dict.GetValueOrDefault("hash");

                if (dict.TryGetValue("user", out var userJson))
                {
                    result.User = JsonSerializer.Deserialize<TelegramUser>(userJson);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Возникла ошибка: {ex}{Environment.NewLine}при  парсинге данных от Телеграм: {data}");
                return result;
            }
        }
    }

}
