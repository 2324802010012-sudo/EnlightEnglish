using System.Diagnostics;
using EnlightEnglishCenter.Models;
using Microsoft.AspNetCore.Mvc;

namespace EnlightEnglishCenter.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Courses()
        {
            return View();
        }
        [HttpPost]
        public IActionResult DangKyHoTro(string HoTen, string Email, string SoDienThoai)
        {
            // TODO: Lưu dữ liệu vào database hoặc gửi mail
            TempData["Message"] = "Cảm ơn bạn! Chúng tôi sẽ liên hệ sớm nhất.";
            return RedirectToAction("Index");
        }


    }
}
