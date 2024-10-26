using CodeToImageGenerator.Web.Models;
using CodeToImageGenerator.Web.Services;

using Microsoft.AspNetCore.Mvc;

namespace CodeToImageGenerator.Web.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var statistics = await _statisticsService.GetStatisticsAsync(startDate, endDate);
            var pageViewModel = new PageViewModel()
            {
                Statistics = statistics
            };
            return View(pageViewModel);
        }
    }
}
