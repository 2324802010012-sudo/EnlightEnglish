using Microsoft.AspNetCore.Mvc;

namespace EnlightEnglishCenter.Controllers
{
    public class LeTanController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Trang lễ tân";
            return View();
        }
    }
}
