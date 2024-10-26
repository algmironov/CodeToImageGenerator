using System.Diagnostics;
using System.Text.Json;

using CodeToImageGenerator.Web.Models;
using CodeToImageGenerator.Web.Services;

using Microsoft.AspNetCore.Mvc;

namespace CodeToImageGenerator.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TelegramBotService _botService;
        private readonly IImageService _imageService;
        private readonly IStatisticsService _statisticsService;

        public HomeController(ILogger<HomeController> logger,
                                TelegramBotService telegramBotService,
                                IImageService imageService,
                                IStatisticsService statisticsService)
        {
            _logger = logger;
            _botService = telegramBotService;
            _imageService = imageService;
            _statisticsService = statisticsService;
        }

        [HttpGet]
        public IActionResult Index(PageViewModel? viewModel)
        {

            if (IsValid(viewModel))
            {
                return View(viewModel);
            }

            try
            {
                var telegramDataJson = HttpContext.Session.GetString("TelegramData");
                if (!string.IsNullOrEmpty(telegramDataJson))
                {
                    var telegramData = JsonSerializer.Deserialize<TelegramMiniAppData>(telegramDataJson);

                    _logger.LogInformation("Received TelegramData: {telegramData}", telegramData);

                    viewModel = new PageViewModel
                    {
                        IsFromTelegram = true,
                        CodeSubmission = new CodeSubmission
                        {
                            ChatId = telegramData.User.Id,
                        }
                    };
                    return View(viewModel);
                }
                return View(new PageViewModel());
            }
            catch (Exception)
            {
                viewModel = new PageViewModel
                {
                    IsFromTelegram = false,
                    CodeSubmission = new CodeSubmission
                    {
                    }
                };
                return View(viewModel);
            }
        }


        [HttpPost]
        [Route("/Home/SubmitCode")]
        public async Task<IActionResult> SubmitCode([FromForm] PageViewModel viewModel)
        {
            var isSuccessful = false;
            var codeSubmission = viewModel.CodeSubmission;

            _logger.LogInformation("Received data: {viewModel}", viewModel);
            
            try
            {
                if (viewModel.IsFromTelegram && codeSubmission != null && ModelState.IsValid)
                {
                    var programmingLanguage = codeSubmission.ProgrammingLanguage;
                    var code = codeSubmission.Code;
                    var chatId = codeSubmission.ChatId;

                    await _botService.SendImageFromCodeAsync((long)chatId, programmingLanguage, code);
                    isSuccessful = true;
                }
                else if (ModelState.IsValid)
                {

                    var programmingLanguage = codeSubmission.ProgrammingLanguage;
                    var code = codeSubmission.Code;
                    isSuccessful = true;

                    return RedirectToAction("DownloadImage", new { programmingLanguage, code });
                }
            }
            catch (Exception ex)
            {
                isSuccessful = false;
                _logger.LogError(ex, "An Error occured during image generation for web user");
            }
            finally
            {
                await _statisticsService.LogGenerationAttemptAsync(
                                                viewModel.IsFromTelegram ? "Telegram" : "Web",
                                                codeSubmission?.ProgrammingLanguage,
                                                isSuccessful);
            }
            return View("Index", viewModel);
        }

        [HttpGet]
        public async Task<FileResult> DownloadImage(string programmingLanguage, string code)
        {
            var imageStream = await _imageService.GenerateImageFromCodeAsync(programmingLanguage, code);
            var filename = _imageService.GenerateFileName(programmingLanguage);

            return File(imageStream, "image/png", filename);
        }

        [HttpPost]
        public IActionResult CloseAlert(PageViewModel viewModel)
        {
            return RedirectToAction("Index", viewModel);
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

        private static bool IsValid(PageViewModel? viewModel)
        {
            return viewModel != null
                && viewModel.CodeSubmission != null
                && viewModel.CodeSubmission.ChatId.HasValue;
        }
    }
}
