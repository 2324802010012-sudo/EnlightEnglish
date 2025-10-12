using Microsoft.AspNetCore.Mvc;

namespace EnlightEnglishCenter.Controllers
{
    public class KeToanController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Trang kế toán";
            return View();
        }
    }
}

