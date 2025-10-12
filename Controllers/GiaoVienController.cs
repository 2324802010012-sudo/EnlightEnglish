using Microsoft.AspNetCore.Mvc;

namespace EnlightEnglishCenter.Controllers
{
    public class GiaoVienController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Trang giảng viên";
            return View();
        }
    }
}
