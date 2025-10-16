using EnlightEnglishCenter.Data;
using EnlightEnglishCenter.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EnlightEnglishCenter.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
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
            if (string.IsNullOrWhiteSpace(HoTen) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(SoDienThoai))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin!";
                return RedirectToAction("Index");
            }

            var dk = new DangKyTuVan
            {
                HoTen = HoTen.Trim(),
                Email = Email.Trim(),
                SoDienThoai = SoDienThoai.Trim(),
                TrangThai = "Chưa liên hệ"
            };

            _context.DangKyTuVan.Add(dk);
            _context.SaveChanges();

            TempData["Success"] = "Gửi đăng ký thành công! Chúng tôi sẽ liên hệ trong 24 giờ.";
            return RedirectToAction("Index");
        }
    }



}
