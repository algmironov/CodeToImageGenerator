using System.Diagnostics;

using CodeToImageGenerator.Web.Models;
using CodeToImageGenerator.Web.Services;

using Microsoft.AspNetCore.Mvc;

namespace CodeToImageGenerator.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TelegramBotService _botService;

        public HomeController(ILogger<HomeController> logger, TelegramBotService telegramBotService)
        {
            _logger = logger;
            _botService = telegramBotService;
        }

        [HttpGet]
        public IActionResult Index(long? chatId) 
        {
            if (chatId != null)
            {
                var submission = new CodeSubmission() { ChatId = (long) chatId };
                return View(submission);
            }
            return View();
        }

        [HttpPost]
        [Route("/Home/SubmitCode")]
        public async Task<IActionResult> SubmitCode([FromForm] CodeSubmission model)
        {
            _logger.LogInformation("Received data: {model}", model);

            try
            {

                if (model != null &&  ModelState.IsValid)
                {
                    var programmingLanguage = model.ProgrammingLanguage;
                    var code = model.Code;
                    var chatId = model.ChatId;

                    await _botService.SendImageFromCodeAsync(chatId, programmingLanguage, code);

                    return RedirectToAction("Index", chatId);
                }
                _logger.LogWarning("Invalid model state: {ModelState}", ModelState);

                return BadRequest("Something went wrong!");
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error deserializing JSON");
                return BadRequest("Invalid JSON format");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Success()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
