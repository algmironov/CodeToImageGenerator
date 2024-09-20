using System.Diagnostics;
using System.Reflection;

using CodeToImageGenerator.Web.Models;
using CodeToImageGenerator.Web.Services;

using Microsoft.AspNetCore.Mvc;

using Telegram.Bot.Types;

namespace CodeToImageGenerator.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TelegramBotService _botService;
        private readonly IImageService _imageService;
        private const string TempFileName = "codepic_image.png";

        public HomeController(ILogger<HomeController> logger, TelegramBotService telegramBotService, IImageService imageService)
        {
            _logger = logger;
            _botService = telegramBotService;
            _imageService = imageService;
        }

        [HttpGet]
        public IActionResult Index(long? chatId)
        {
            var viewModel = new PageViewModel
            {
                IsFromTelegram = chatId.HasValue,
                CodeSubmission = new CodeSubmission
                {
                    ChatId = chatId
                }
            };

            ViewData["Title"] = "Отправка кода";
            return View(viewModel);
        }

        [HttpPost]
        [Route("/Home/SubmitCode")]
        public async Task<IActionResult> SubmitCode([FromForm] PageViewModel viewModel)
        {
            var codeSubmission = viewModel.CodeSubmission;

            _logger.LogInformation("Received data: {viewModel}", viewModel);

            if (viewModel.IsFromTelegram)
            {
                try
                {

                    if (codeSubmission != null && ModelState.IsValid)
                    {
                        var programmingLanguage = codeSubmission.ProgrammingLanguage;
                        var code = codeSubmission.Code;
                        var chatId = codeSubmission.ChatId;

                        await _botService.SendImageFromCodeAsync((long)chatId, programmingLanguage, code);

                        return RedirectToAction("Index", chatId);
                    }
                    _logger.LogWarning("Invalid model state: {ModelState}", ModelState);

                    ViewData["Title"] = "Отправка кода";
                    return View("Index", viewModel);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deserializing JSON");

                    ViewData["Title"] = "Отправка кода";
                    return View("Index", viewModel);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var programmingLanguage = codeSubmission.ProgrammingLanguage;
                    var code = codeSubmission.Code;

                    /*
                    var streamTask = _imageService.GenerateImageFromCodeAsync(programmingLanguage!, code);
                    var imageStream = await streamTask;

                    var filePath = Path.Combine(Path.GetTempPath(), TempFileName);

                    using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                    imageStream.CopyTo(fileStream);
                    */
                    return RedirectToAction("DownloadImage", new { programmingLanguage, code });

                }
                catch (Exception ex)
                {
                    _logger.LogError(message: "An Error occured during image generation for web user", ex);

                    ViewData["Title"] = "Отправка кода";
                    return View("Index", viewModel);
                }
            }

            ViewData["Title"] = "Отправка кода";
            return View("Index", viewModel);

        }

        [HttpGet]
        public async Task<FileResult> DownloadImage(string programmingLanguage, string code)
        {
            var imageStream = await _imageService.GenerateImageFromCodeAsync(programmingLanguage, code);

            return File(imageStream, "image/png", "generated_image.png");
        }

        /*
        public IActionResult DownloadImage(string filePath)
        {
            using var image = System.IO.File.OpenRead(filePath);
            var fileName = "codepic_image.png";
            var mimeType = "image/png";

            return File(image, mimeType, fileName);
        }
        */

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
