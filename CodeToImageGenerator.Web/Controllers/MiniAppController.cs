using Microsoft.AspNetCore.Mvc;

namespace CodeToImageGenerator.Web.Controllers
{
    public class MiniAppController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
